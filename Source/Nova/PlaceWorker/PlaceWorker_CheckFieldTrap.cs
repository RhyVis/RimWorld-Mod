using Verse;

namespace Nova;

public class PlaceWorker_CheckFieldTrap : PlaceWorker
{
  public override AcceptanceReport AllowsPlacing(
    BuildableDef checkingDef,
    IntVec3 loc, Rot4 rot,
    Map map,
    Thing thingToIgnore = null,
    Thing thing = null)
  {
    var hasFieldTrap = map.spawnedThings.Contains(NovaThingDefOf.Nova_FieldTrap_Extreme);
    return base.AllowsPlacing(checkingDef, loc, rot, map, thingToIgnore, thing) && !hasFieldTrap
      ? true
      : "Nova_PlaceWorker_CheckFieldTrap".Translate();
  }
}
