using HarmonyLib;


namespace SurprisingHazards.Patches;

[HarmonyPatch(typeof(SpikeRoofTrap))]
public class SpikeTrapPatch
{
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    public static void PatchStart(SpikeRoofTrap __instance)
    {
        SurprisingHazardsPlugin.RegisterHazard(__instance);
    }
}