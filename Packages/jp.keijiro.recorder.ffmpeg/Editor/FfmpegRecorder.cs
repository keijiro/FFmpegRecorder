using UnityEngine;
using UnityEngine.Rendering;
using FFmpegOut;

namespace UnityEditor.Recorder
{
    sealed class FfmpegRecorder : BaseTextureRecorder<FfmpegRecorderSettings>
    {
        FFmpegPipe _pipe;

        protected override TextureFormat readbackTextureFormat
        {
            get { return  TextureFormat.RGBA32; }
        }

        public override bool BeginRecording(RecordingSession session)
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

            m_Settings.fileNameGenerator.CreateDirectory(session);

            var input = m_Inputs[0] as BaseRenderTextureInput;
            var args = 
                "-y -f rawvideo -vcodec rawvideo -pixel_format rgba"
                + " -colorspace bt709"
                + " -video_size " + input.outputWidth + "x" + input.outputHeight
                + " -framerate " + session.settings.frameRate
                + " -loglevel warning -i - " + m_Settings.preset.GetOptions()
                + " " + m_Settings.fileNameGenerator.BuildAbsolutePath(session);

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
