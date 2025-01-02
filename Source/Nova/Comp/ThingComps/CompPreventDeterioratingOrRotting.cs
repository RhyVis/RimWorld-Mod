using System.Collections.Generic;
using Verse;

namespace Nova
{
  public class CompProperties_PreventDeterioratingOrRotting : CompProperties
  {
    public CompProperties_PreventDeterioratingOrRotting()
    {
      compClass = typeof(CompPreventDeterioratingOrRotting);
    }
  }

  public class CompPreventDeterioratingOrRotting : ThingComp
  {
    public static Dictionary<Map, HashSet<IntVec3>> NoDRPlaces = new Dictionary<Map, HashSet<IntVec3>>();

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
      base.PostSpawnSetup(respawningAfterLoad);
      if (NoDRPlaces.ContainsKey(parent.Map))
        NoDRPlaces[parent.Map].Add(parent.Position);
      else
        NoDRPlaces[parent.Map] = new HashSet<IntVec3> { parent.Position };
    }

    public override void PostDeSpawn(Map map)
    {
      base.PostDeSpawn(map);
      if (NoDRPlaces.ContainsKey(map)) NoDRPlaces[map].Remove(parent.Position);
    }
  }
}