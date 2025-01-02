using HarmonyLib;
using RimWorld;
using Verse;

namespace Nova;

[HarmonyPatch(typeof(SteadyEnvironmentEffects), "TryDoDeteriorate")]
public static class Deteriorate_Patch
{
  public static bool Prefix(Thing t)
  {
    return !(t?.Map != null && CompPreventDeterioratingOrRotting.NoDRPlaces.TryGetValue(t.Map, out var pos) &&
             pos.Contains(t.Position));
  }
}

[HarmonyPatch(typeof(CompRottable), "Active", MethodType.Getter)]
public static class Rot_Patch
{
  public static bool Prefix(CompRottable __instance, ref bool __result)
  {
    if (__instance.parent?.Map != null &&
        CompPreventDeterioratingOrRotting.NoDRPlaces.TryGetValue(__instance.parent.Map, out var pos) &&
        pos.Contains(__instance.parent.Position))
    {
      __result = false;
      return false;
    }

    return true;
  }
}
