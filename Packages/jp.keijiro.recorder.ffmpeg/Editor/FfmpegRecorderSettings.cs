using UnityEngine;
using UnityEditor.Recorder.Input;
using System.Collections.Generic;
using FFmpegOut;

namespace UnityEditor.Recorder
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
            fileNameGenerator.fileName = "ffmpeg";
            _imageInputSelector.ForceEvenResolution(true);
        }

        public string FrameRateArgs {
            get {
                if (!frameRateConversion) return "";
                return "-r " + outputFrameRate;
            }
        }

        public override IEnumerable<RecorderInputSettings> inputsSettings
        {
            get { yield return _imageInputSelector.selected; }
        } 

        public override string extension {
            get { return preset.GetSuffix().Substring(1); }
        }
    }
}
