using System.Text;
using PipeSystem;

namespace Overclock;

public class CompProperties_SpawnIntoNet : CompProperties_Resource
{
    public int interval = 2500;
    public int spawnCount = 1;

    public CompProperties_SpawnIntoNet()
    {
        compClass = typeof(CompSpawnIntoNet);
    }
}

public class CompSpawnIntoNet : CompResource
{
    private CompPowerTrader _compPowerTrader;
    private CompResource _compResource;

    private int _ticker;
    private new CompProperties_SpawnIntoNet Props => (CompProperties_SpawnIntoNet)props;

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        _compPowerTrader = parent.TryGetComp<CompPowerTrader>();
        _compResource = parent.TryGetComp<CompResource>();
        if (!respawningAfterLoad)
            _ticker = Props.interval;
    }

    public override string CompInspectStringExtra()
    {
        if (!parent.Spawned)
            return null;

        var sb = new StringBuilder();
        sb.Append(base.CompInspectStringExtra());
        sb.AppendLineIfNotEmpty();
        sb.Append("Overclock_CompSpawnIntoNet_Inspect1".Translate());
        sb.Append(_ticker.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor));
        return sb.ToString();
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref _ticker, "spawnTicker");
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        foreach (var gizmo in base.CompGetGizmosExtra())
            yield return gizmo;

        if (Prefs.DevMode)
            yield return new Command_Action
            {
                action = delegate
                {
                    _ticker = 50;
                },
                defaultLabel = "Spawn now",
                defaultDesc = "Spawn now",
            };

        yield break;
    }

    public override void CompTick()
    {
        base.CompTick();
        TickUp(1);
    }

    public override void CompTickRare()
    {
        base.CompTickRare();
        TickUp(250);
    }

    private void TickUp(int t)
    {
        if (
            !parent.Spawned
            || parent.Position.Fogged(parent.Map)
            || !(_compPowerTrader?.PowerOn ?? false)
        )
            return;

        _ticker -= t;
        if (_ticker > 0)
            return;

        _ticker = Props.interval;
        DoSpawn();
    }

    private bool DoSpawn()
    {
        if (!parent.Spawned)
            return false;
        var net = _compResource.PipeNet;
        if (net.AvailableCapacity < Props.spawnCount)
            return false;
        net.DistributeAmongStorage(Props.spawnCount, out _);
        return true;
    }
}
