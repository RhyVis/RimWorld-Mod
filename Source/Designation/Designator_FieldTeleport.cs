namespace Overclock;

public class Designator_FieldTeleport : Designator
{
    public Designator_FieldTeleport()
    {
        defaultLabel = "Overclock_Designation_FieldTeleport_Label".Translate();
        defaultDesc = "Overclock_Designation_FieldTeleport_Desc".Translate();
        icon = ContentFinder<Texture2D>.Get("Overclock/UI/FieldTeleportD");
        useMouseIcon = true;
        soundSucceeded = SoundDefOf.Click;
    }

    protected override DesignationDef Designation => OverclockDefOf.Overclock_FieldTeleportD;

    public override int DraggableDimensions => 2;

    public override AcceptanceReport CanDesignateCell(IntVec3 loc)
    {
        if (!loc.InBounds(Map) || loc.Fogged(Map))
            return false;

        var pawnFirst = loc.GetFirstPawn(Map);
        if (pawnFirst is null)
            return "Overclock_Designation_FieldTeleport_Msg_MustPawn".Translate();

        var tryReport = CanDesignateThing(pawnFirst);
        return !tryReport.Accepted ? tryReport : true;
    }

    public override AcceptanceReport CanDesignateThing(Thing t)
    {
        if (Map.designationManager.DesignationOn(t, Designation) is not null)
            return false;
        return t is Pawn p && p.Faction != Faction.OfPlayer && !p.InBed() && !p.IsPrisonerOfColony;
    }

    public override void DesignateSingleCell(IntVec3 c)
    {
        c.GetThingList(Map)
            .ForEach(t =>
            {
                if (t is Pawn p)
                    DesignateThing(p);
            });
    }

    public override void DesignateThing(Thing t)
    {
        Map.designationManager.RemoveAllDesignationsOn(t);
        Map.designationManager.AddDesignation(new Designation(t, Designation));
    }
}
