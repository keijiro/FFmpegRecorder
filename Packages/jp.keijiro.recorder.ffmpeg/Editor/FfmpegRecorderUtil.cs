using UnityEditor.Recorder;
using System.Reflection;

namespace FFmpegOut.Recorder
{
    //
    // Some important APIs are still protected in the UnityRecorder assembly.
    // This class provides public accessors using reflection.
    //

    static class BaseRenderTextureInputExtension
    {
        public static int GetOutputWidth(this BaseRenderTextureInput input)
        {
            var prop = typeof(BaseRenderTextureInput).GetProperty(
                "OutputWidth",
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            return (int)prop.GetValue(input);
        }

        public static int GetOutputHeight(this BaseRenderTextureInput input)
        {
            var prop = typeof(BaseRenderTextureInput).GetProperty(
                "OutputHeight",
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            return (int)prop.GetValue(input);
        }
    }

    static class ImageInputSelectorExtension
    {
        public static void ForceEvenResolutionPublic
            (this ImageInputSelector selector, bool value)
        {
            var method = typeof(ImageInputSelector).GetMethod(
                "ForceEvenResolution",
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            method.Invoke(selector, new object [] { value });
        }
    }
}
