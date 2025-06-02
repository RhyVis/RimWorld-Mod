namespace Overclock;

public class CompProperties_HediffGiver : CompProperties
{
    public int checkInterval = 600;
    public HediffDef hediffDef;
    public float radius = 2.9f;
    public float severityAdjust = 1.0f;
    public List<StatDef> stats;

    public CompProperties_HediffGiver()
    {
        compClass = typeof(CompHediffGiver);
    }
}

public class CompHediffGiver : ThingComp
{
    // 1: All, 2: Hostile, 3: Everyone except Player, 4: Prisoner
    private int _factionType;
    private CompProperties_HediffGiver Props => (CompProperties_HediffGiver)props;

    private TaggedString GizmoLabel =>
        _factionType switch
        {
            0 => "Overclock_CompHediffGiver_Gizmo_LabelA".Translate(),
            1 => "Overclock_CompHediffGiver_Gizmo_LabelB".Translate(),
            2 => "Overclock_CompHediffGiver_Gizmo_LabelC".Translate(),
            3 => "Overclock_CompHediffGiver_Gizmo_LabelD".Translate(),
            4 => "Overclock_CompHediffGiver_Gizmo_LabelE".Translate(),
            _ => "ERR",
        };

    private Texture2D GizmoIcon =>
        _factionType switch
        {
            0 => TexCommand.ClearPrioritizedWork,
            1 => TexCommand.ForbidOff,
            2 => TexCommand.Attack,
            3 => TexCommand.FireAtWill,
            4 => ContentFinder<Texture2D>.Get("UI/Commands/ForPrisoners"),
            _ => TexCommand.CannotShoot,
        };

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        foreach (var gizmo in base.CompGetGizmosExtra())
            yield return gizmo;
        yield return new Command_Action
        {
            defaultLabel = GizmoLabel,
            defaultDesc = "Overclock_CompHediffGiver_Gizmo_Desc".Translate(),
            icon = GizmoIcon,
            action = SwitchFactionType,
        };
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref _factionType, "factionType");
    }

    public override void CompTick()
    {
        base.CompTick();
        if (_factionType == 0 || !parent.IsHashIntervalTick(Props.checkInterval) || !parent.Spawned)
            return;
        GiveHediff();
    }

    private void GiveHediff()
    {
        switch (_factionType)
        {
            case 0:
                break;
            case 1:
            {
                this.FindPawnsAliveInRange(Props.radius)
                    .ToList()
                    .ForEach(pawn =>
                        pawn.ApplyHediffWithStat(Props.hediffDef, Props.stats, Props.severityAdjust)
                    );
                break;
            }
            case 2:
            {
                this.FindPawnsAliveInRange(Props.radius)
                    .Where(pawn =>
                        (pawn.Faction != null && pawn.Faction.HostileTo(Faction.OfPlayer))
                        || (pawn.AnimalOrWildMan() && pawn.InAggroMentalState)
                    )
                    .ToList()
                    .ForEach(pawn =>
                        pawn.ApplyHediffWithStat(Props.hediffDef, Props.stats, Props.severityAdjust)
                    );
                break;
            }
            case 3:
            {
                this.FindPawnsAliveInRange(Props.radius)
                    .Where(pawn => !pawn.Faction?.IsPlayer ?? true)
                    .ToList()
                    .ForEach(pawn =>
                        pawn.ApplyHediffWithStat(Props.hediffDef, Props.stats, Props.severityAdjust)
                    );
                break;
            }
            case 4:
            {
                this.FindPawnsAliveInRange(Props.radius)
                    .Where(pawn => pawn.IsPrisoner)
                    .ToList()
                    .ForEach(pawn =>
                        pawn.ApplyHediffWithStat(Props.hediffDef, Props.stats, Props.severityAdjust)
                    );
                break;
            }
            default:
            {
                Msg.Error($"Unexpected Type of {_factionType}");
                this.ThrowMote("Unexpected Type");
                _factionType = 0;
                break;
            }
        }
    }

    private void SwitchFactionType()
    {
        _factionType = (_factionType + 1) % 5;
    }
}
