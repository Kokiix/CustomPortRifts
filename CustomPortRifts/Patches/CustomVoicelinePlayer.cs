using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using HarmonyLib;
using RhythmRift;
using RiftOfTheNecroManager;
using Shared.Audio;

namespace CustomPortRifts.Patches.CustomVoiceLines;

public enum VoiceLineCategory
{
    Good,
    Bad,
    GameOver,
    Normal
}

[HarmonyPatch(typeof(RRPortraitView))]
public static class CustomVoicelinePlayer {

    static Random _rng = new();

    [HarmonyPatch(nameof(RRPortraitView.PerformanceLevelChange))]
    public static void Postfix(RRPortraitView __instance, RRPerformanceLevel performanceLevel, bool shouldPlaySoundReaction) {
        if (shouldPlaySoundReaction)
        {
            var voicelineCategory = performanceLevel switch {
                RRPerformanceLevel.GameOver => VoiceLineCategory.GameOver,

                RRPerformanceLevel.Terrible or
                RRPerformanceLevel.Poor => VoiceLineCategory.Bad,

                RRPerformanceLevel.Ok or
                RRPerformanceLevel.Well or
                RRPerformanceLevel.Awesome or
                RRPerformanceLevel.Amazing or
                RRPerformanceLevel.VibePower => VoiceLineCategory.Good,
                
                _ => VoiceLineCategory.Normal
            };

            var voiceDict = _rng.Next(2) == 0 ? CustomVoicelineLoader.heroVoiceLines : CustomVoicelineLoader.counterpartVoiceLines;
            if (voiceDict.Count > 0) {
                __instance._audioManager.StopAudioEvent(__instance._activeVOInstance);
                if (voiceDict.ContainsKey(voicelineCategory)) {
                    var voiceLineList = voiceDict[voicelineCategory];
                    var soundToPlay = voiceLineList[_rng.Next(voiceLineList.Count)];
                    
                    FMODUnity.RuntimeManager.CoreSystem.playSound(soundToPlay, default, false, out Channel _);
                }
            }
        }
    }
}
