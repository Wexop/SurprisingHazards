using HarmonyLib;


namespace SurprisingHazards.Patches;

[HarmonyPatch(typeof(Turret))]
public class TurretPatch
{
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    public static void PatchStart(Turret __instance)
    {
        SurprisingHazardsPlugin.RegisterHazard(__instance);
    }
}