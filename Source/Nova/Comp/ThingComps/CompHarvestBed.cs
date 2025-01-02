using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Nova;

public class CompProperties_HarvestBed : CompProperties
{
  public int checkInterval = 1250;
  public float harvestEyeCost = 2.0f;
  public float harvestFullCost = 8.0f;
  public float harvestLegCost = 2.0f;

  public CompProperties_HarvestBed()
  {
    compClass = typeof(CompHarvestBed);
  }
}

public class CompHarvestBed : ThingComp
{
  private CompRefuelable _fuelComp;
  private CompProperties_HarvestBed Props => (CompProperties_HarvestBed)props;

  public override void PostSpawnSetup(bool respawningAfterLoad)
  {
    base.PostSpawnSetup(respawningAfterLoad);
    _fuelComp = parent.TryGetComp<CompRefuelable>();
  }

  public override IEnumerable<Gizmo> CompGetGizmosExtra()
  {
    foreach (var gizmo in base.CompGetGizmosExtra()) yield return gizmo;
    yield return new Command_Action
    {
      defaultLabel = "Nova_Building_BodypartRemove_Gizmo1_Label".Translate(),
      defaultDesc = "Nova_Building_BodypartRemove_Gizmo1_Desc".Translate(),
      icon = ThingDefOf.MedicineUltratech.uiIcon,
      action = Harvest
    };
    yield return new Command_Action
    {
      defaultLabel = "Nova_Building_BodypartRemove_Gizmo2_Label".Translate(),
      defaultDesc = "Nova_Building_BodypartRemove_Gizmo2_Desc".Translate(),
      icon = ThingDefOf.MedicineIndustrial.uiIcon,
      action = HarvestLegs
    };
    yield return new Command_Action
    {
      defaultLabel = "Nova_Building_BodypartRemove_Gizmo3_Label".Translate(),
      defaultDesc = "Nova_Building_BodypartRemove_Gizmo3_Desc".Translate(),
      icon = ThingDefOf.MedicineIndustrial.uiIcon,
      action = HarvestEyes
    };
    yield return new Command_Action
    {
      defaultLabel = "Nova_Building_BodypartRemove_Gizmo4_Label".Translate(),
      defaultDesc = "Nova_Building_BodypartRemove_Gizmo4_Desc".Translate(),
      icon = ThingDefOf.MedicineHerbal.uiIcon,
      action = RandomDamage
    };
    yield return new Command_Action
    {
      defaultLabel = "Nova_Building_BodypartRemove_Gizmo5_Label".Translate(),
      defaultDesc = "Nova_Building_BodypartRemove_Gizmo5_Desc".Translate(),
      icon = ThingDefOf.MedicineHerbal.uiIcon,
      action = Execution
    };
  }

  private void Harvest()
  {
    if (parent is Building_Bed bed && bed.CurOccupants.FirstOrDefault() != null)
    {
      if (_fuelComp.Fuel < Props.harvestFullCost)
      {
        MoteMaker.ThrowText(bed.TrueCenter() + Utility.RightUp, bed.Map,
          "Nova_Building_BodypartRemove_Mote2".Translate());
        return;
      }

      var pawn = bed.CurOccupants.First();
      pawn.ApplyHediff(NovaHediffDefOf.Nova_BodyPartWorking);
      try
      {
        pawn.health.hediffSet.GetNotMissingParts()
          .Where(record => record.def.spawnThingOnRemoved != null)
          .ToList()
          .ForEach(record =>
          {
            var partToSpawn = ThingMaker.MakeThing(record.def.spawnThingOnRemoved);
            partToSpawn.stackCount = 1;
            GenPlace.TryPlaceThing(partToSpawn, parent.Position, parent.Map, ThingPlaceMode.Near);
            pawn.DamageBodyPart(record);
          });
        _fuelComp.ConsumeFuel(Props.harvestFullCost);
      }
      catch (Exception e)
      {
        Msg.E(e.ToString());
        bed.ThrowMote("Nova_Building_BodypartRemove_Mote3".Translate());
      }
      finally
      {
        pawn.RemoveHediff(NovaHediffDefOf.Nova_BodyPartWorking);
      }
    }
    else
    {
      MoteMaker.ThrowText(parent.TrueCenter() + Utility.RightUp, parent.Map,
        "Nova_Building_BodypartRemove_Mote1".Translate());
    }
  }

  private void HarvestLegs()
  {
    if (parent is Building_Bed bed && bed.CurOccupants.FirstOrDefault() != null)
    {
      if (_fuelComp.Fuel < Props.harvestLegCost)
      {
        bed.ThrowMote("Nova_Building_BodypartRemove_Mote2".Translate());
        return;
      }

      var pawn = bed.CurOccupants.First();
      pawn.ApplyHediff(NovaHediffDefOf.Nova_BodyPartWorking);
      try
      {
        pawn.health.hediffSet.GetNotMissingParts()
          .Where(record => record.def.defName.Contains("Leg"))
          .ToList()
          .ForEach(record =>
          {
            if (record.def.spawnThingOnRemoved != null)
            {
              var partToSpawn = ThingMaker.MakeThing(record.def.spawnThingOnRemoved);
              partToSpawn.stackCount = 1;
              GenPlace.TryPlaceThing(partToSpawn, parent.Position, parent.Map, ThingPlaceMode.Near);
            }

            pawn.DamageBodyPart(record);
          });
        _fuelComp.ConsumeFuel(Props.harvestLegCost);
      }
      catch (Exception e)
      {
        Msg.E(e.ToString());
        bed.ThrowMote("Nova_Building_BodypartRemove_Mote3".Translate());
      }
      finally
      {
        pawn.RemoveHediff(NovaHediffDefOf.Nova_BodyPartWorking);
      }
    }
    else
    {
      MoteMaker.ThrowText(parent.TrueCenter() + Utility.RightUp, parent.Map,
        "Nova_Building_BodypartRemove_Mote1".Translate());
    }
  }

  private void HarvestEyes()
  {
    if (parent is Building_Bed bed && bed.CurOccupants.FirstOrDefault() != null)
    {
      if (_fuelComp.Fuel < Props.harvestEyeCost)
      {
        bed.ThrowMote("Nova_Building_BodypartRemove_Mote2".Translate());
        return;
      }

      var pawn = bed.CurOccupants.First();
      pawn.ApplyHediff(NovaHediffDefOf.Nova_BodyPartWorking);
      try
      {
        pawn.health.hediffSet.GetNotMissingParts()
          .Where(record => record.def.defName.Contains("Eye"))
          .ToList()
          .ForEach(record =>
          {
            if (record.def.spawnThingOnRemoved != null)
            {
              var partToSpawn = ThingMaker.MakeThing(record.def.spawnThingOnRemoved);
              partToSpawn.stackCount = 1;
              GenPlace.TryPlaceThing(partToSpawn, parent.Position, parent.Map, ThingPlaceMode.Near);
            }

            pawn.DamageBodyPart(record);
          });
        _fuelComp.ConsumeFuel(Props.harvestEyeCost);
      }
      catch (Exception e)
      {
        Msg.E(e.ToString());
        bed.ThrowMote("Nova_Building_BodypartRemove_Mote3".Translate());
      }
      finally
      {
        pawn.RemoveHediff(NovaHediffDefOf.Nova_BodyPartWorking);
      }
    }
    else
    {
      parent.ThrowMote("Nova_Building_BodypartRemove_Mote1".Translate());
    }
  }

  private void RandomDamage()
  {
    if (parent is Building_Bed bed && bed.CurOccupants.FirstOrDefault() is not null)
    {
      var pawn = bed.CurOccupants.First();
      pawn.DamageRandomBodyPart();
    }
    else
    {
      parent.ThrowMote("Nova_Building_BodypartRemove_Mote1".Translate());
    }
  }

  private void Execution()
  {
    if (parent is Building_Bed bed && bed.CurOccupants.FirstOrDefault() != null)
    {
      var pawn = bed.CurOccupants.First();
      pawn.health.hediffSet.GetNotMissingParts()
        .Where(record => record.def.defName.Contains("Brain"))
        .ToList()
        .ForEach(record => pawn.DamageBodyPart(record));

      var makeThing = ThingMaker.MakeThing(ThingDefOf.Meat_Human);
      makeThing.stackCount = 5;

      GenPlace.TryPlaceThing(makeThing, parent.Position, parent.Map, ThingPlaceMode.Near);
    }
    else
    {
      parent.ThrowMote("Nova_Building_BodypartRemove_Mote1".Translate());
    }
  }
}
