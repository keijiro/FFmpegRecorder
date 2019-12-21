using UnityEngine;
using UnityEditor;
using UnityEditor.Recorder;
using System.Linq;

namespace FFmpegOut.Recorder
{
    [CustomEditor(typeof(FfmpegRecorderSettings))]
    class FfmpegRecorderEditor : RecorderEditor
    {
        SerializedProperty _preset;
        SerializedProperty _frameRateConversion;
        SerializedProperty _outputFrameRate;

        static class Styles
        {
            public static readonly GUIContent fpsConversion = new GUIContent("FPS Conversion");
            public static readonly GUIContent outputRate = new GUIContent("Output Rate");
        }

        GUIContent[] _presetLabels;
        int[] _presetOptions;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (target == null) return;

            _preset = serializedObject.FindProperty("preset");
            _frameRateConversion = serializedObject.FindProperty("frameRateConversion");
            _outputFrameRate = serializedObject.FindProperty("outputFrameRate");

            // Preset labels
            var presets = FFmpegPreset.GetValues(typeof(FFmpegPreset));
            _presetLabels = presets.Cast<FFmpegPreset>().
                Select(p => new GUIContent(p.GetDisplayName())).ToArray();
            _presetOptions = presets.Cast<int>().ToArray();
        }

        protected override void FileTypeAndFormatGUI()
        {
            EditorGUILayout.IntPopup(_preset, _presetLabels, _presetOptions);

            var wide = EditorGUIUtility.labelWidth > 140;

            if (wide)
                EditorGUILayout.PropertyField(_frameRateConversion);
            else
                EditorGUILayout.PropertyField(_frameRateConversion, Styles.fpsConversion);

            if (_frameRateConversion.boolValue)
            {
                EditorGUI.indentLevel++;
                if (wide)
                    EditorGUILayout.PropertyField(_outputFrameRate);
                else
                    EditorGUILayout.PropertyField(_outputFrameRate, Styles.outputRate);
                EditorGUI.indentLevel--;
            }
        }
    }
}
