using HarmonyLib;
using RhythmRift;
using RiftOfTheNecroManager;
using Shared.TrackData;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomPortRifts.Patches;


[HarmonyPatch(typeof(LocalTrackPortrait))]
public static class CustomVoicelineLoader {
    public static Dictionary<RRPerformanceLevel, List<string>> heroVoiceLines = [];
    public static Dictionary<RRPerformanceLevel, List<string>> counterpartVoiceLines = [];
    public static readonly HashSet<string> allowedExtensions = new (StringComparer.OrdinalIgnoreCase)
    {
        ".ogg",
        ".mp3",
        ".wav"
    };

    [HarmonyPatch(nameof(LocalTrackPortrait.TryLoadCustomPortrait))]
    public static void Postfix(string basePath) {
        var voiceDir = basePath + "/VoiceLines";
        try {
            if (FileUtils.IsDirectory(voiceDir)) {
                var voiceLineDict = Path.GetFileName(basePath) switch {
                    "Hero" => heroVoiceLines,
                    "Counterpart" => counterpartVoiceLines,
                    _ => null,
                };

                void AddFiles(RRPerformanceLevel performance, object folderName) {
                    if (FileUtils.IsDirectory(voiceDir + folderName))
                    {
                        var fileList = 
                            Directory.GetFiles(voiceDir + folderName)
                                .Where(file => allowedExtensions.Contains(Path.GetExtension(file)))
                            .ToList();
                        if (fileList.Count > 0) voiceLineDict?[performance] = fileList;
                    }

                }

                AddFiles(RRPerformanceLevel.GameOver, "/GameOver");
                AddFiles(RRPerformanceLevel.Ok, "/Good");
                AddFiles(RRPerformanceLevel.Poor, "/Bad");
                AddFiles(RRPerformanceLevel.Normal, "/Normal");
            }
        }
        catch (Exception arg) {
            Log.Error($"Failed to load voice lines from {basePath}: {arg}");
        }
    }
}