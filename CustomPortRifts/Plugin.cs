using BepInEx;
using CustomPortRifts.Patches;
using RiftOfTheNecroManager;
using UnityEngine;

namespace CustomPortRifts;


[BepInPlugin(GUID, NAME, VERSION)]
[NecroManagerInfo(menuNameOverride: "Custom PortRifts")]
public class Plugin : RiftPlugin {
    public const string GUID = "com.lalabuff.necrodancer.customportrifts";
    public const string NAME = "CustomPortRifts";
    public const string VERSION = "2.1.0";

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            CustomVoicelinePatch.PlayTestSound();
        }
    }
}
