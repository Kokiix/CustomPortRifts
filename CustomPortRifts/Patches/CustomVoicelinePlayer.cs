using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using HarmonyLib;
using RhythmRift;
using RiftOfTheNecroManager;

namespace CustomPortRifts.Patches;

[HarmonyPatch(typeof(RRPortraitView))]
public static class CustomVoicelinePlayer {
    [HarmonyPatch(nameof(RRPortraitView.PerformanceLevelChange))]
    public static void Postfix(RRPortraitView __instance, RRPerformanceLevel performanceLevel, bool shouldPlaySoundReaction) {
        if (shouldPlaySoundReaction)
            switch (performanceLevel) {
                case RRPerformanceLevel.GameOver:
                    // game over
                    break;
                case RRPerformanceLevel.Terrible:
                case RRPerformanceLevel.Poor:
                    // bad
                    break;
                case RRPerformanceLevel.Ok:
                case RRPerformanceLevel.Well:
                case RRPerformanceLevel.Awesome:
                case RRPerformanceLevel.Amazing:
                case RRPerformanceLevel.VibePower:
                    // good
                    break;
                default:
                    // normal
                    break;
            }
    }
}
