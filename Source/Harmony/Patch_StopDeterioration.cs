using HarmonyLib;

namespace Overclock;

[HarmonyPatch(typeof(SteadyEnvironmentEffects), "TryDoDeteriorate")]
internal static class Patch_NoDeteriorate
{
    [HarmonyPrefix]
    private static bool Prefix(Thing t) =>
        !(
            t?.Map is not null
            && CompStopDeterioration.Cache.TryGetValue(t.Map, out var pos)
            && pos.Contains(t.Position)
        );
}

[HarmonyPatch(typeof(CompRottable), "Active", MethodType.Getter)]
public static class Patch_Rot
{
    [HarmonyPrefix]
    public static bool Prefix(CompRottable __instance, ref bool __result)
    {
        if (
            !(
                __instance.parent?.Map is not null
                && CompStopDeterioration.Cache.TryGetValue(__instance.parent.Map, out var pos)
                && pos.Contains(__instance.parent.Position)
            )
        )
            return true;

        __result = false;
        return false;
    }
}
