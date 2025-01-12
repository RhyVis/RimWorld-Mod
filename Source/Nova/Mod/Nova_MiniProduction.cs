using System.Linq;
using RimWorld;
using Verse;

namespace Nova;

[StaticConstructorOnStartup]
public static class Nova_MiniProduction
{
  private const string Prefix = "Nova_MiniProduction_";

  static Nova_MiniProduction()
  {
    Msg.Out("Initializing Nova Mini Production");
    foreach (var miniWorkbench in DefDatabase<ThingDef>.AllDefsListForReading.Where(def =>
               def.defName.StartsWith(Prefix)))
    {
      var refWorkbench = ThingDef.Named(miniWorkbench.defName.Replace(Prefix, ""));

      miniWorkbench.label = $"{"Nova_MiniProduction_LabelPrefix".Translate()}{refWorkbench.label}";
      miniWorkbench.description = refWorkbench.description;

      miniWorkbench.recipes ??= [];
      miniWorkbench.recipes.AddRange(refWorkbench.AllRecipes.Where(r => !miniWorkbench.recipes.Contains(r)));

      DefDatabase<WorkGiverDef>.AllDefsListForReading
        .Where(workGiverDef => workGiverDef.fixedBillGiverDefs?.Contains(refWorkbench) == true)
        .ToList()
        .ForEach(workGiverDef => workGiverDef.fixedBillGiverDefs.Add(miniWorkbench));

      var compPropFacility = miniWorkbench.GetCompProperties<CompProperties_AffectedByFacilities>();
      if (compPropFacility is null)
        continue;
      var compPropFacilityRef = miniWorkbench.GetCompProperties<CompProperties_AffectedByFacilities>();
      if (compPropFacilityRef is null)
        continue;
      compPropFacility.linkableFacilities = compPropFacilityRef.linkableFacilities;
    }

    DefDatabase<ThingDef>.AllDefsListForReading
      .Where(def => def.HasComp(typeof(CompFacility)))
      .ToList()
      .ForEach(parentDef => parentDef.GetCompProperties<CompProperties_Facility>().ResolveReferences(parentDef));
  }
}
