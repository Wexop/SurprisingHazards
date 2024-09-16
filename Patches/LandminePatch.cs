using HarmonyLib;


namespace SurprisingHazards.Patches;

[HarmonyPatch(typeof(Landmine))]
public class LandminePatch
{
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    public static void PatchStart(Landmine __instance)
    {
        SurprisingHazardsPlugin.RegisterHazard(__instance);
    }
}