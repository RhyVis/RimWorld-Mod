using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Nova;

public class Building_ThingGuardian : Building
{
  private const int Interval = 60;
  private bool _activated;
  private bool _allyOnly;
  private bool _clearGas;

  public override IEnumerable<Gizmo> GetGizmos()
  {
    foreach (var gizmo in base.GetGizmos()) yield return gizmo;
    yield return new Command_Toggle
    {
      defaultLabel = "Nova_CompThingGuardian_Gizmo_Label1".Translate(),
      defaultDesc = "Nova_CompThingGuardian_Gizmo_Desc1".Translate(),
      icon = _activated ? TexCommand.ForbidOff : TexCommand.ForbidOn,
      isActive = () => _activated,
      toggleAction = delegate { _activated = !_activated; }
    };
    yield return new Command_Toggle
    {
      defaultLabel = "Nova_CompThingGuardian_Gizmo_Label2".Translate(),
      defaultDesc = "Nova_CompThingGuardian_Gizmo_Desc2".Translate(),
      icon = TexCommand.PauseCaravan,
      isActive = () => _allyOnly,
      toggleAction = delegate { _allyOnly = !_allyOnly; }
    };
    yield return new Command_Toggle
    {
      defaultLabel = "Nova_CompThingGuardian_Gizmo_Label3".Translate(),
      defaultDesc = "Nova_CompThingGuardian_Gizmo_Desc3".Translate(),
      icon = TexCommand.ToggleVent,
      isActive = () => _clearGas,
      toggleAction = delegate { _clearGas = !_clearGas; }
    };
  }

  public override void ExposeData()
  {
    base.ExposeData();
    Scribe_Values.Look(ref _activated, "activated");
    Scribe_Values.Look(ref _allyOnly, "allyOnly");
    Scribe_Values.Look(ref _clearGas, "clearGas");
  }

  public override void Tick()
  {
    base.Tick();
    if (!_activated || !this.IsHashIntervalTick(Interval) || !Spawned)
      return;
    DoRepair();
    if (_clearGas)
      ClearGas();
  }

  private void DoRepair()
  {
    var things = this.GetRoom()?.ContainedAndAdjacentThings
      .Where(thing => thing.def.category.NotEqualToAllOf(ThingCategory.Pawn, ThingCategory.Gas, ThingCategory.Filth))
      .Where(thing => !_allyOnly || (thing.Faction?.IsPlayer ?? false))
      .ToList() ?? [];

    foreach (var thing in things)
    {
      if (thing.def.useHitPoints && thing.HitPoints < thing.MaxHitPoints)
        thing.HitPoints = thing.MaxHitPoints;

      var rotComp = thing.TryGetComp<CompRottable>();
      if (rotComp is null)
        continue;
      rotComp.RotProgress -= 2000f;
      if (rotComp.RotProgress < 0f)
        rotComp.RotProgress = 0f;
    }
  }

  private void ClearGas()
  {
    var density = Traverse.Create(MapHeld.gasGrid).Field("gasDensity").GetValue<uint[]>();
    Msg.Debug($"GasDensity Index: {density.Length}");
    foreach (var c in this.GetRoom().Cells)
      density[Map.cellIndices.CellToIndex(c)] = 0U;
    Map.mapDrawer.WholeMapChanged((ulong)MapMeshFlagDefOf.Gas);
  }
}
