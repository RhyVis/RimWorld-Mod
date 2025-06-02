namespace Overclock;

public class CompProperties_RemoteDoor : CompProperties
{
    public CompProperties_RemoteDoor()
    {
        compClass = typeof(CompRemoteDoor);
    }
}

public class CompRemoteDoor : ThingComp
{
    private CompPowerTrader _powerTrader;

    private Building_Door Door => (Building_Door)parent;

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        if (parent is not Building_Door)
        {
            Msg.Error(
                $"CompRemoteDoor must be attached to a Building_Door, but was attached to {nameof(parent.def.thingClass)}"
            );
            parent.Destroy();
        }

        _powerTrader = parent.TryGetComp<CompPowerTrader>();

        if (_powerTrader == null)
            Msg.Debug($"No CompPowerTrader set with CompRemoteDoor at {parent.Position}");
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        foreach (var gizmo in base.CompGetGizmosExtra())
            yield return gizmo;
        yield return new Command_Toggle
        {
            defaultLabel = "Overclock_CompRemoteDoor_Gizmo_Label".Translate(),
            defaultDesc = "Overclock_CompRemoteDoor_Gizmo_Desc".Translate(),
            icon = TexCommand.HoldOpen,
            hotKey = KeyBindingDefOf.Misc6,
            isActive = () => Door.Open,
            toggleAction = delegate
            {
                if (_powerTrader is null || _powerTrader.PowerOn)
                    SetDoorState(!Door.Open);
                else
                    parent.ThrowMote("未通电");
            },
        };
    }

    private void SetDoorState(bool b)
    {
        if (parent is Building_Door door)
            door.SetPrivateField("openInt", b);
    }
}
