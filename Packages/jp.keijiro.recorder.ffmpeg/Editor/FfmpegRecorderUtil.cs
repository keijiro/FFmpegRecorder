using UnityEditor.Recorder;
using System.Reflection;

namespace FFmpegOut.Recorder
{
    //
    // Some important APIs are still protected in the UnityRecorder assembly.
    // This class provides public accessors using reflection.
    //

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
