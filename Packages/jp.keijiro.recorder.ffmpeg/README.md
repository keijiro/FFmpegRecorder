FFmpegRecorder
==============

**FFmpegRecorder** is an extension for [Unity Recorder] that allows recording
videos in Unity Editor via [FFmpeg].

[Unity Recorder]: https://docs.unity3d.com/Packages/com.unity.recorder@latest
[FFmpeg]: https://ffmpeg.org/

System Requirements
-------------------

- Unity 2019.1 or later

CAVEAT
------

This package uses a dirty trick to access the internal API of Unity Recorder
-- It pretends to be [AOV Recorder] to circumvent the protection. This means
you can't use FFmpegRecorder and AOV Recorder at the same time. You only can
use one of these packages in a project.

[AOV Recorder]: https://docs.unity3d.com/Packages/com.unity.aovrecorder@latest/

How To Install
--------------

The FFmpegRecorder package uses the [scoped registry] feature to import
dependent packages. Please add the following sections to the package manifest
file (`Packages/manifest.json`).

To the `scopedRegistries` section:

```
{
  "name": "Keijiro",
  "url": "https://registry.npmjs.com",
  "scopes": [ "jp.keijiro" ]
}
```

To the `dependencies` section:

```
"jp.keijiro.recorder.ffmpeg": "0.0.6"
```

After changes, the manifest file should look like below:

```
{
  "scopedRegistries": [
    {
      "name": "Keijiro",
      "url": "https://registry.npmjs.com",
      "scopes": [ "jp.keijiro" ]
    }
  ],
  "dependencies": {
    "jp.keijiro.recorder.ffmpeg": "0.0.6",
    ...
```

[scoped registry]: https://docs.unity3d.com/Manual/upm-scoped.html

Frequently Asked Questions
--------------------------

#### Output video is vertically flipped

This issue will be fixed in a future version. At the moment, please manually
turn on the "Flip Vertical" option in the recorder settings when you find the
output is flipped.
