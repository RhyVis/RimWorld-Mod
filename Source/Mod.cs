namespace Overclock;

public class Overclock_Mod(ModContentPack content) : Mod(content)
{
    public override void DoSettingsWindowContents(Rect inRect)
    {
        base.DoSettingsWindowContents(inRect);
        var list = new Listing_Standard();
        list.Begin(inRect);
        list.CheckboxLabeled(
            "Overclock_SettingCat_DebugMsg_Label".Translate(),
            ref Overclock_Settings.Debug
        );
        list.End();
    }

    public override string SettingsCategory()
    {
        return "Overclock_SettingCat_DebugMsg".Translate();
    }
}

public class Overclock_Settings : ModSettings
{
    public static bool Debug;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref Debug, "bDebug", false);
    }
}

[StaticConstructorOnStartup]
public static class Overclock_DefInit
{
    private const string Prefix = "Overclock_MiniProduction_";

    static Overclock_DefInit()
    {
        Msg.Out("Initializing Overclock Mini Production");
        foreach (
            var miniWorkbench in DefDatabase<ThingDef>.AllDefsListForReading.Where(def =>
                def.defName.StartsWith(Prefix)
            )
        )
        {
            var refWorkbench = ThingDef.Named(miniWorkbench.defName.Replace(Prefix, ""));

            miniWorkbench.label =
                $"{"Overclock_MiniProduction_LabelPrefix".Translate()}{refWorkbench.label}";
            miniWorkbench.description = refWorkbench.description;

            miniWorkbench.recipes ??= [];
            miniWorkbench.recipes.AddRange(
                refWorkbench.AllRecipes.Where(r => !miniWorkbench.recipes.Contains(r))
            );

            DefDatabase<WorkGiverDef>
                .AllDefsListForReading.Where(workGiverDef =>
                    workGiverDef.fixedBillGiverDefs?.Contains(refWorkbench) == true
                )
                .ToList()
                .ForEach(workGiverDef => workGiverDef.fixedBillGiverDefs.Add(miniWorkbench));

            var cpfMiniWorkbench =
                miniWorkbench.GetCompProperties<CompProperties_AffectedByFacilities>();
            if (cpfMiniWorkbench is null)
                continue;

            var cpfRefWorkbench =
                refWorkbench.GetCompProperties<CompProperties_AffectedByFacilities>();
            if (cpfRefWorkbench is null)
                continue;

            cpfMiniWorkbench.linkableFacilities = cpfRefWorkbench.linkableFacilities;
        }

        Msg.Out("Initializing Overclock Research Terminal");
        var cpfResearchTerminal =
            OverclockThingDefOf.Overclock_ScienceTerminal.GetCompProperties<CompProperties_AffectedByFacilities>();
        if (cpfResearchTerminal is null)
            return;
        var cpfHiTechResearchTable = ThingDef
            .Named("HiTechResearchBench")
            .GetCompProperties<CompProperties_AffectedByFacilities>();
        if (cpfHiTechResearchTable is null)
            return;
        cpfResearchTerminal.linkableFacilities ??= [];
        cpfResearchTerminal.linkableFacilities.AddRange(
            cpfHiTechResearchTable.linkableFacilities.Where(f =>
                !cpfResearchTerminal.linkableFacilities.Contains(f)
                && f.defName != OverclockThingDefOf.Overclock_ScienceTerminal.defName
            )
        );

        DefDatabase<ThingDef>
            .AllDefsListForReading.Where(def => def.HasComp(typeof(CompFacility)))
            .ToList()
            .ForEach(parentDef =>
                parentDef.GetCompProperties<CompProperties_Facility>().ResolveReferences(parentDef)
            );
    }
}
