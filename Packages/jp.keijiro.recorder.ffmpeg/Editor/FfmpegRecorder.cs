using UnityEngine;
using UnityEditor.Recorder;
using UnityEngine.Rendering;
using Unity.Collections;

namespace FFmpegOut.Recorder
{
    sealed class FfmpegRecorder : BaseTextureRecorder<FfmpegRecorderSettings>
    {
        FFmpegPipe _pipe;

        protected override TextureFormat ReadbackTextureFormat
            => Settings.TextureFormat;

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
                "-y -f rawvideo -vcodec rawvideo"
                + " -pixel_format " + Settings.PixelFormatName
                + " -colorspace bt709"
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
            if (Settings.highBitDepth)
            {
                using (var img = HalfToUNorm16.Convert(texture.GetRawTextureData<ulong>()))
                {
                    _pipe.PushFrameData(img.Reinterpret<byte>(sizeof(ulong)));
                    _pipe.SyncFrameData();
                }
            }
            else
            {
                _pipe.PushFrameData(texture.GetRawTextureData<byte>());
                _pipe.SyncFrameData();
            }
        }

        protected override void WriteFrame(AsyncGPUReadbackRequest request)
        {
            if (Settings.highBitDepth)
            {
                using (var img = HalfToUNorm16.Convert(request.GetData<ulong>()))
                {
                    _pipe.PushFrameData(img.Reinterpret<byte>(sizeof(ulong)));
                    _pipe.SyncFrameData();
                }
            }
            else
            {
                _pipe.PushFrameData(request.GetData<byte>());
                _pipe.SyncFrameData();
            }
        }
    }
}
