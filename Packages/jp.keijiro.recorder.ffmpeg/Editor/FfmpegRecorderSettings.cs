using UnityEngine;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using System.Collections.Generic;

namespace FFmpegOut.Recorder
{
    [RecorderSettings(typeof(FfmpegRecorder), "Ffmpeg", "movie_16")]
    sealed class FfmpegRecorderSettings : RecorderSettings
    {
        public FFmpegPreset preset = FFmpegPreset.H264Default;
        public bool frameRateConversion = false;
        public float outputFrameRate = 59.94f;

        [SerializeField] ImageInputSelector _imageInputSelector = new ImageInputSelector();

        public FfmpegRecorderSettings()
        {
            FileNameGenerator.FileName = "ffmpeg";
            _imageInputSelector.ForceEvenResolutionPublic(true);
        }

        public string FrameRateArgs {
            get {
                if (!frameRateConversion) return "";
                return "-r " + outputFrameRate;
            }
        }

        public override IEnumerable<RecorderInputSettings> InputsSettings
        {
            get { yield return _imageInputSelector.Selected; }
        } 

        protected override string Extension {
            get { return preset.GetSuffix().Substring(1); }
        }
    }
}
