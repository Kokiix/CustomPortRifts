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
using UnityEngine;

namespace CustomPortRifts.Patches.CustomVoiceLines;

public enum VoiceLineCategory
{
    Good,
    Bad,
    GameOver,
    Recover
}

[HarmonyPatch(typeof(RRPortraitView), nameof(RRPortraitView.PerformanceLevelChange))]
public static class CustomVoicelinePlayer {

    static System.Random _rng = new();
    static float _timeOfLastVoiceline = 0;

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
                
                _ => VoiceLineCategory.Recover
            };

            var voiceDict = _rng.Next(2) == 0 ? CustomVoicelineLoader.heroVoiceLines : CustomVoicelineLoader.counterpartVoiceLines;
            if (voiceDict.Count > 0) {
                __instance._audioManager.StopAudioEvent(__instance._activeVOInstance);
                if (voiceDict.ContainsKey(voicelineCategory) && (Time.time - _timeOfLastVoiceline >= 15 || voicelineCategory == VoiceLineCategory.GameOver)) {
                    var voiceLineList = voiceDict[voicelineCategory];
                    var soundToPlay = voiceLineList[_rng.Next(voiceLineList.Count)];
                    
                    FMODUnity.RuntimeManager.CoreSystem.playSound(soundToPlay, default, false, out Channel _);
                }
            }

            if (Time.time - _timeOfLastVoiceline >= 15)
                _timeOfLastVoiceline = Time.time;
        }
    }
}
