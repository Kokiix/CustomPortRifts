using HarmonyLib;
using RhythmRift;
using RiftOfTheNecroManager;
using Shared.TrackData;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomPortRifts.Patches.CustomVoiceLines;


[HarmonyPatch(typeof(LocalTrackPortrait))]
public static class CustomVoicelineLoader {
    public static Dictionary<VoiceLineCategory, List<string>> heroVoiceLines = [];
    public static Dictionary<VoiceLineCategory, List<string>> counterpartVoiceLines = [];
    public static readonly HashSet<string> allowedExtensions = new (StringComparer.OrdinalIgnoreCase)
    {
        ".ogg",
        ".mp3",
        ".wav"
    };

    [HarmonyPatch(nameof(LocalTrackPortrait.TryLoadCustomPortrait))]
    public static void Postfix(string basePath) {
        var voiceDir = Path.GetDirectoryName(basePath) + "/VoiceLines/" + Path.GetFileName(basePath);
        try {
            if (FileUtils.IsDirectory(voiceDir)) {
                var voiceLineDict = Path.GetFileName(basePath) switch {
                    "Hero" => heroVoiceLines,
                    "Counterpart" => counterpartVoiceLines,
                    _ => null,
                };

                void AddFiles(VoiceLineCategory performance, object folderName) {
                    if (FileUtils.IsDirectory(voiceDir + folderName))
                    {
                        var fileList = 
                            Directory.GetFiles(voiceDir + folderName)
                                .Where(file => allowedExtensions.Contains(Path.GetExtension(file)))
                            .ToList();
                        if (fileList.Count > 0) voiceLineDict?[performance] = fileList;
                    }

                }

                AddFiles(VoiceLineCategory.GameOver, "/GameOver");
                AddFiles(VoiceLineCategory.Good, "/Good");
                AddFiles(VoiceLineCategory.Bad, "/Bad");
                AddFiles(VoiceLineCategory.Normal, "/Normal");
            }
        }
        catch (Exception arg) {
            Log.Error($"Failed to load voice lines from {basePath}: {arg}");
        }
    }
}