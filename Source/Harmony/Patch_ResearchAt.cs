using HarmonyLib;

namespace Overclock;

[HarmonyPatch(typeof(ResearchProjectDef), "CanBeResearchedAt")]
internal static class Patch_ResearchAt
{
    [HarmonyPostfix]
    internal static void Postfix(
        Building_ResearchBench bench,
        bool ignoreResearchBenchPowerStatus,
        ResearchProjectDef __instance,
        ref bool __result
    )
    {
        if (
            __result
            || __instance.requiredResearchBuilding != ThingDef.Named("HiTechResearchBench")
        )
            return;

        if (bench.def.GetModExtension<DefModExt_HiTechResearch>() is null)
            return;

        if (!ignoreResearchBenchPowerStatus)
        {
            var comp = bench.GetComp<CompPowerTrader>();
            if (comp?.PowerOn == false)
                return;
        }

        if (__instance.requiredResearchFacilities?.Any() == true)
        {
            var affectedByFacilities = bench.TryGetComp<CompAffectedByFacilities>();
            if (affectedByFacilities is null)
                return;

            var facilitiesListForReading = affectedByFacilities.LinkedFacilitiesListForReading;
            if (
                __instance.requiredResearchFacilities.Any(facility =>
                    facilitiesListForReading.Find(x =>
                        x.def == facility && affectedByFacilities.IsFacilityActive(x)
                    )
                        is null
                )
            )
                return;
        }

        __result = true;
    }
}
