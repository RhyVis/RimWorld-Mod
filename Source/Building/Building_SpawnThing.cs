using AdaptiveStorage;

namespace Overclock;

public class Building_SpawnThing : ThingClass
{
    private bool _active = true;
    private ThingDef _spawnThingDef;
    private int _ticker = 1250;

    private int StackLimit => _spawnThingDef.stackLimit;

    protected override void OnSpawn(Map map, SpawnMode spawnMode)
    {
        base.OnSpawn(map, spawnMode);

        _spawnThingDef = def.building.fixedStorageSettings.filter.AllowedThingDefs.FirstOrDefault();

        if (_spawnThingDef is null)
        {
            Msg.Error($"Unable to find allowed thing def for {def.defName}");
            Destroy();
        }
        else if (def.building.maxItemsInCell > 1)
        {
            Msg.Error($"Only single item stacks are supported for {def.defName}");
            Destroy();
        }
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var gizmo in base.GetGizmos())
            yield return gizmo;
        yield return new Command_Action
        {
            defaultLabel = _spawnThingDef.label,
            defaultDesc = "Overclock_Building_SpawnThing_Gizmo1_Desc".Translate(
                _spawnThingDef.label
            ),
            icon = _spawnThingDef.uiIcon,
            action = delegate
            {
                _ticker = 1250;
                DoSpawn();
            },
        };
        yield return new Command_Toggle
        {
            defaultLabel = "Overclock_Building_SpawnThing_Gizmo2_Label".Translate(),
            defaultDesc = "Overclock_Building_SpawnThing_Gizmo2_Desc".Translate(),
            icon = TexCommand.DesirePower,
            isActive = () => _active,
            toggleAction = delegate
            {
                _active = !_active;
            },
        };
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref _active, "isActive");
    }

    public override void Tick()
    {
        base.Tick();
        TickMethod(1);
    }

    public override void TickRare()
    {
        base.TickRare();
        TickMethod(250);
    }

    public override void TickLong()
    {
        base.TickLong();
        TickMethod(2500);
    }

    private void TickMethod(int t)
    {
        if (!_active || !Spawned)
            return;

        _ticker -= t;

        if (_ticker > 0)
            return;

        DoSpawn();
        _ticker = 1250;
    }

    private void DoSpawn()
    {
        var slot = GetSlotGroup();
        var t = slot.HeldThings.FirstOrDefault();
        if (t is null)
        {
            var adder = ThingMaker.MakeThing(_spawnThingDef);
            adder.stackCount = StackLimit;
            GenPlace.TryPlaceThing(adder, Position, Map, ThingPlaceMode.Direct);
            this.ThrowMote("Overclock_Building_SpawnThing_Mote2".Translate());
            return;
        }

        if (t.def.defName != _spawnThingDef.defName)
        {
            this.ThrowMote(
                "Overclock_Building_SpawnThing_Mote3".Translate(_spawnThingDef.label, t.Label)
            );
            _active = false;
            return;
        }

        if (t.stackCount >= StackLimit)
            return; // No need to spawn more

        this.ThrowMote("Overclock_Building_SpawnThing_Mote1".Translate(StackLimit - t.stackCount));
        t.stackCount = StackLimit;
    }
}
