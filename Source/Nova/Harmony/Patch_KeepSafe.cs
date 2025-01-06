using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace Nova;

[HarmonyPatch(typeof(SteadyEnvironmentEffects), "TryDoDeteriorate")]
[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.WithMembers)]
public static class DeterioratePatch
{
  [HarmonyPrefix]
  public static bool Prefix(Thing t)
  {
    return !(t?.Map is not null &&
             CompKeepSafePos.SafePos.TryGetValue(t.Map, out var pos) &&
             pos.Contains(t.Position));
  }
}

[HarmonyPatch(typeof(CompRottable), "Active", MethodType.Getter)]
[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.WithMembers)]
public static class RotPatch
{
  [HarmonyPrefix]
  public static bool Prefix(CompRottable __instance, ref bool __result)
  {
    var shouldPrevent = __instance.parent?.Map is not null &&
                        CompKeepSafePos.SafePos.TryGetValue(__instance.parent.Map, out var pos) &&
                        pos.Contains(__instance.parent.Position);
    if (!shouldPrevent)
      return true;

    __result = false;
    return false;
  }
}
