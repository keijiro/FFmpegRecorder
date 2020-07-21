using UnityEngine;
using UnityEditor.Recorder;
using UnityEngine.Rendering;

namespace FFmpegOut.Recorder
{
    sealed class FfmpegRecorder : BaseTextureRecorder<FfmpegRecorderSettings>
    {
        FFmpegPipe _pipe;

        protected override TextureFormat ReadbackTextureFormat
        {
            get { return  TextureFormat.RGBA32; }
        }

        protected override bool BeginRecording(RecordingSession session)
        {
            if (!base.BeginRecording(session)) return false;

            if (!FFmpegPipe.IsAvailable)
            {
                Debug.LogError(
                    "Failed to initialize an FFmpeg session due to missing " +
                    "executable file. Please check FFmpeg installation."
                );
                return false;
            }

            Settings.FileNameGenerator.CreateDirectory(session);

            var input = m_Inputs[0] as BaseRenderTextureInput;
            var args = 
                "-y -f rawvideo -vcodec rawvideo -pixel_format rgba"
                + " -colorspace bt709 -color_trc iec61966-2-1"
                + " -video_size " + input.OutputWidth + "x" + input.OutputHeight
                + " -framerate " + session.settings.FrameRate
                + " -loglevel error -i - " + Settings.preset.GetOptions()
                + " " + Settings.FrameRateArgs
                + " \"" + Settings.FileNameGenerator.BuildAbsolutePath(session) + "\"";

            _pipe = new FFmpegPipe(args);

            return true;
        }

        protected override void DisposeEncoder()
        {
            base.DisposeEncoder();

            if (_pipe != null)
            {
                var error = _pipe.CloseAndGetOutput();

                if (!string.IsNullOrEmpty(error))
                    Debug.LogWarning(
                        "FFmpeg returned with warning/error messages. " +
                        "See the following lines for details:\n" + error
                    );

                _pipe.Dispose();
                _pipe = null;
            }
        }

        protected override void WriteFrame(Texture2D texture)
        {
            _pipe.PushFrameData(texture.GetRawTextureData<byte>());
            _pipe.SyncFrameData();
        }

        protected override void WriteFrame(AsyncGPUReadbackRequest request)
        {
            _pipe.PushFrameData(request.GetData<byte>());
            _pipe.SyncFrameData();
        }
    }
}
