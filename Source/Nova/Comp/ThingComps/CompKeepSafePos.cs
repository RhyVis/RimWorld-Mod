using System.Collections.Generic;
using Verse;

namespace Nova;

public class CompProperties_KeepSafePos : CompProperties
{
  public CompProperties_KeepSafePos()
  {
    compClass = typeof(CompKeepSafePos);
  }
}

public class CompKeepSafePos : ThingComp
{
  public static readonly Dictionary<Map, HashSet<IntVec3>> SafePos = new();

  public override void PostSpawnSetup(bool respawningAfterLoad)
  {
    base.PostSpawnSetup(respawningAfterLoad);

    if (SafePos.TryGetValue(parent.Map, out var set))
      set.Add(parent.Position);
    else
      SafePos[parent.Map] = [parent.Position];
  }

  public override void PostDeSpawn(Map map)
  {
    base.PostDeSpawn(map);

    if (SafePos.TryGetValue(map, out var set))
      set.Remove(parent.Position);
  }
}
