using FMOD;
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


[HarmonyPatch(typeof(LocalTrackPortrait), nameof(LocalTrackPortrait.TryLoadCustomPortrait))]
public static class CustomVoicelineLoader {
    public static Dictionary<VoiceLineCategory, List<Sound>> heroVoiceLines = [];
    public static Dictionary<VoiceLineCategory, List<Sound>> counterpartVoiceLines = [];
    public static readonly HashSet<string> allowedExtensions = new (StringComparer.OrdinalIgnoreCase)
    {
        ".ogg",
        ".mp3",
        ".wav"
    };

    public static void Postfix(string basePath) {
        var voiceDir = basePath + "/Voicelines";
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
                        var soundList = 
                            Directory.GetFiles(voiceDir + folderName)
                                .Where(file => allowedExtensions.Contains(Path.GetExtension(file)))
                                .Select(file => {
                                    FMODUnity.RuntimeManager.CoreSystem.createSound(file, FMOD.MODE.DEFAULT, out Sound sound);
                                    return sound;
                                })
                            .ToList();
                        if (soundList.Count > 0) voiceLineDict?[performance] = soundList;
                    }
                }

                AddFiles(VoiceLineCategory.GameOver, "/GameOver");
                AddFiles(VoiceLineCategory.Good, "/Good");
                AddFiles(VoiceLineCategory.Bad, "/Bad");
                AddFiles(VoiceLineCategory.Recover, "/Recover");
            }
        }
        catch (Exception arg) {
            Log.Error($"Failed to load voice lines from {basePath}: {arg}");
        }
    }
}