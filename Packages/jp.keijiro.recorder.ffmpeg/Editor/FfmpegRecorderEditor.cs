using UnityEngine;
using System.Linq;
using FFmpegOut;

namespace UnityEditor.Recorder
{
    [CustomEditor(typeof(FfmpegRecorderSettings))]
    class FfmpegRecorderEditor : RecorderEditor
    {
        SerializedProperty _preset;

        GUIContent[] _presetLabels;
        int[] _presetOptions;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (target == null) return;

            _preset = serializedObject.FindProperty("preset");

            // Preset labels
            var presets = FFmpegPreset.GetValues(typeof(FFmpegPreset));
            _presetLabels = presets.Cast<FFmpegPreset>().
                Select(p => new GUIContent(p.GetDisplayName())).ToArray();
            _presetOptions = presets.Cast<int>().ToArray();
        }

        protected override void FileTypeAndFormatGUI()
        {
            EditorGUILayout.IntPopup(_preset, _presetLabels, _presetOptions);
        }
    }
}
