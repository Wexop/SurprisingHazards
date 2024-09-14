using HarmonyLib;


namespace SurprisingHazards.Patches;

[HarmonyPatch(typeof(RoundManager))]
public class RoundManagerPatch
{
    [HarmonyPatch(nameof(RoundManager.GenerateNewLevelClientRpc))]
    [HarmonyPrefix]
    public static void PatchLoadLevel(RoundManager __instance)
    {
        SurprisingHazardsPlugin.instance.RegisteredHazards.Clear();
    }
}