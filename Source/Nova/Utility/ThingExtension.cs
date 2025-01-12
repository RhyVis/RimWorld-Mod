using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Nova;

public static class ThingExtension
{
  public static IEnumerable<Pawn> FindPawnsInRange(this ThingComp compInCenter, float range)
  {
    var rangeSquared = range * range;
    return compInCenter.parent.Map.mapPawns.AllPawnsSpawned
      .Where(pawn => pawn.Position.DistanceToSquared(compInCenter.parent.Position) < rangeSquared);
  }

  public static IEnumerable<Pawn> FindPawnsAliveInRange(this ThingComp compInCenter, float range)
  {
    return compInCenter.FindPawnsInRange(range).Where(pawn => !pawn.health.Dead);
  }

  private static bool ConsumePower(this ThingComp comp, float requiredVal)
  {
    var powerComp = comp.parent.TryGetComp<CompPowerTrader>();
    if (!(powerComp?.PowerOn ?? false)) return false;

    Msg.Debug("ConsumePower: Start power check.");
    var powerSum = powerComp.PowerNet.CurrentStoredEnergy();

    if (powerSum < requiredVal)
    {
      Msg.Debug("ConsumePower: Insufficient power.");
      return false;
    }

    var toConsume = requiredVal;
    foreach (var battery in powerComp.PowerNet.batteryComps)
    {
      if (battery.StoredEnergy > toConsume)
      {
        battery.SetStoredEnergyPct((battery.StoredEnergy - toConsume) / battery.Props.storedEnergyMax);
        Msg.Debug("ConsumePower: Clear.");
        return true;
      }

      toConsume -= battery.StoredEnergy;
      battery.SetStoredEnergyPct(0f);
      Msg.Debug("ConsumePower: Cost " + battery.StoredEnergy);

      if (toConsume > 0f)
      {
        Msg.Debug("ConsumePower: Next battery.");
        continue;
      }

      Msg.Debug("ConsumePower: Clear.");
      return true;
    }

    Msg.Debug("ConsumePower: Unexpected failure.");
    return false;
  }

  public static void CompSpawnThingWithPowerCost(this ThingComp comp, string defName, int minCount, int refillCount,
    int eachItemCost)
  {
    if (!(comp.parent is IHaulDestination))
    {
      Msg.Error("CompSpawnThingWithPowerCost: Parent is not IHaulDestination.");
      return;
    }

    var slotGroup = comp.parent.GetSlotGroup();
    if (slotGroup is null)
    {
      Msg.Error("CompSpawnThingWithPowerCost: SlotGroup is null.");
      return;
    }

    var presentCount = slotGroup.HeldThings
      .Where(thing => thing.def.defName == defName)
      .Sum(thing => thing.stackCount);

    if (presentCount >= minCount)
    {
      Msg.Debug("CompSpawnThingWithPowerCost: Sufficient count.");
      return;
    }

    var targetDef = ThingDef.Named(defName);
    if (targetDef == null)
    {
      Msg.Error($"CompSpawnThingWithPowerCost: There's no thing named {defName} at {comp.parent.Position} setting.");
      return;
    }

    var addUpStack = ThingMaker.MakeThing(targetDef);
    var addUpVal = refillCount <= targetDef.stackLimit
      ? refillCount - presentCount
      : targetDef.stackLimit - presentCount;

    if (!comp.ConsumePower(addUpVal * eachItemCost))
    {
      Msg.Debug("CompSpawnThingWithPowerCost: Insufficient power.");
      return;
    }

    addUpStack.stackCount = addUpVal;
    GenPlace.TryPlaceThing(addUpStack, comp.parent.Position, comp.parent.Map, ThingPlaceMode.Near);
  }

  public static void ThrowMote(this Thing thing, string s)
  {
    MoteMaker.ThrowText(thing.TrueCenter(), thing.Map, s);
  }

  public static void ThrowMote(this ThingComp comp, string s)
  {
    ThrowMote(comp.parent, s);
  }
}
