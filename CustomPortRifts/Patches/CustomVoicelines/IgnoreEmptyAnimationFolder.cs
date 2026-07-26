using HarmonyLib;
using RiftOfTheNecroManager;
using Shared.TrackData;

namespace CustomPortRifts.Patches.CustomVoiceLines;


[HarmonyPatch(typeof(LocalTrackPortrait), nameof(LocalTrackPortrait.TryLoadCustomPortrait))]
public static class IgnoreEmptyAnimationFolder {
    public static void Postfix(ref LocalTrackPortrait? __result) {
        if (__result.CustomAnimations == null)
            __result = null;
    }
}
