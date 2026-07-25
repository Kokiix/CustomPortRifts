using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RhythmRift;
using RiftOfTheNecroManager;

namespace CustomPortRifts.Patches;

/*
    This patch rewrites a single switch statement in PerformanceLevelChange.
    Rewriting the original statement doesn't feel great, but I'm not aware of a better way to do this.
*/
[HarmonyPatch(typeof(RRPortraitView))]
public static class CustomVoicelinePatch
{
    [HarmonyPatch(nameof(RRPortraitView.PerformanceLevelChange))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> ReplacePerformanceLevelSwitch(IEnumerable<CodeInstruction> instructions)
    {
        var customAudioSwitch = AccessTools.Method(typeof(CustomVoicelinePatch), nameof(CustomAudioSwitch));

        return new CodeMatcher(instructions)
        // if shouldPlaySoundReaction (param 2) is false, return 
        .MatchForward(
            useEnd: true,
            new CodeMatch(OpCodes.Ldarg_2),
            new CodeMatch(OpCodes.Brfalse))
        .Advance(1)
        // else, call the custom switch and return
        .Insert(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldarg_1),
            new CodeInstruction(OpCodes.Call, customAudioSwitch),
            new CodeInstruction(OpCodes.Ret))
        .InstructionEnumeration();
    }

    public static void CustomAudioSwitch(RRPortraitView instance, RRPerformanceLevel performanceLevel)
    {
        Log.Error("custom audio triggered!");
        switch (performanceLevel)
        {
            case RRPerformanceLevel.GameOver:
                if (!instance._gameOverReactionEventRef.IsNull)
                {
                    instance._activeVOInstance = instance._audioManager.PlayAudioEvent(instance._gameOverReactionEventRef);
                }
                break;
            case RRPerformanceLevel.Terrible:
            case RRPerformanceLevel.Poor:
                if (!instance._doingPoorlyReactionEventRef.IsNull)
                {
                    instance._activeVOInstance = instance._audioManager.PlayAudioEvent(instance._doingPoorlyReactionEventRef);
                }
                break;
            case RRPerformanceLevel.Ok:
            case RRPerformanceLevel.Well:
            case RRPerformanceLevel.Awesome:
            case RRPerformanceLevel.Amazing:
            case RRPerformanceLevel.VibePower:
                if (!instance._doingWellReactionEventRef.IsNull)
                {
                    instance._activeVOInstance = instance._audioManager.PlayAudioEvent(instance._doingWellReactionEventRef);
                }
                break;
            default:
                if (!instance._normalReactionEventRef.IsNull)
                {
                    instance._activeVOInstance = instance._audioManager.PlayAudioEvent(instance._normalReactionEventRef);
                }
                break;
        }
    }
}
