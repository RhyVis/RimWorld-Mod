using HarmonyLib;
using RimWorld.Planet;

namespace Overclock;

[HarmonyPatch(typeof(Map), "ConstructComponents")]
internal static class Patch_ConstructComponents
{
    [HarmonyPostfix]
    private static void Postfix(Map __instance)
    {
        if (__instance.uniqueID is -1 || __instance.info is null)
            return;
        _ = new FridgeManager(__instance);
    }
}

[HarmonyPatch(typeof(Game), "DeinitAndRemoveMap")]
internal static class Patch_DeinitAndRemoveMap
{
    [HarmonyPostfix]
    private static void Postfix(Map map)
    {
        if (map is null || !FridgeManager.UtilityCache.Remove(map.uniqueID) || !Prefs.DevMode)
            return;
        Msg.Debug("[Fridge] Map removal detected.");
    }
}

[HarmonyPatch(typeof(GenTemperature), "GetTemperatureForCell")]
[HarmonyPriority(800)]
internal class Patch_GetTemperatureForCell
{
    [HarmonyPrefix]
    private static bool Prefix(Map map, IntVec3 c, ref float __result)
    {
        if (
            !FridgeManager.UtilityCache.TryGetValue(map?.uniqueID ?? -1, out var fridgeManager)
            || !fridgeManager.GetAdjustedTemperature(c)
        )
            return true;
        __result = -10f;
        return false;
    }
}

[HarmonyPatch(typeof(GoodwillSituationManager), "RecalculateAll")]
internal class Patch_GoodwillSituationManager_RecalculateAll
{
    [HarmonyPostfix]
    private static void Postfix()
    {
        foreach (var FridgeManager in FridgeManager.UtilityCache.Values)
            FridgeManager.Tick();
    }
}

[HarmonyPatch(typeof(CompPowerTrader), "PostDeSpawn")]
internal class Patch_PostDeSpawn
{
    [HarmonyPostfix]
    private static void Postfix(CompPowerTrader __instance, Map map)
    {
        if (
            map is null
            || !FridgeManager.UtilityCache.TryGetValue(map.uniqueID, out var fridgeManager)
            || !fridgeManager.FridgeCache.Contains(__instance)
        )
            return;
        Traverse.Create(__instance).Field("powerOnInt").SetValue(false);
        fridgeManager.UpdateFridgeGrid(__instance);
        fridgeManager.FridgeCache.Remove(__instance);
    }
}

[HarmonyPatch(typeof(CompPowerTrader), "PostSpawnSetup")]
internal class Patch_PostSpawnSetup
{
    [HarmonyPostfix]
    private static void Postfix(CompPowerTrader __instance)
    {
        var map = __instance.parent.Map;
        if (
            map == null
            || !__instance.parent.def.HasModExtension<DefModExt_Fridge>()
            || !FridgeManager.UtilityCache.TryGetValue(map.uniqueID, out var fridgeManager)
        )
            return;
        fridgeManager.FridgeCache.Add(__instance);
        fridgeManager.UpdateFridgeGrid(__instance);
    }
}

[HarmonyPatch(typeof(World), "FinalizeInit")]
internal class Patch_World_FinalizeInit
{
    [HarmonyPostfix]
    private static void Postfix()
    {
        FridgeManager.UtilityCache = [];
    }
}
