using System.Collections.Generic;

namespace Overclock;

public class CompProperties_StopDeterioration : CompProperties
{
    public CompProperties_StopDeterioration()
    {
        compClass = typeof(CompStopDeterioration);
    }
}

public class CompStopDeterioration : ThingComp
{
    public static readonly Dictionary<Map, HashSet<IntVec3>> Cache = [];

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);

        if (Cache.TryGetValue(parent.Map, out var set))
            set.Add(parent.Position);
        else
            Cache[parent.Map] = [parent.Position];
    }

    public override void PostDeSpawn(Map map)
    {
        base.PostDeSpawn(map);

        if (Cache.TryGetValue(map, out var set))
            set.Remove(parent.Position);
        else
            Msg.Error(
                $"[StopDeterioration] Tried to remove a position that doesn't exist: {parent.Position}"
            );
    }
}
