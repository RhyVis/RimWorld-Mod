namespace Overclock.Utility.Extension;

public static class ThingExtension
{
    public static IEnumerable<Pawn> FindPawnsInRange(this Thing thing, float range) =>
        thing.Map.mapPawns.AllPawnsSpawned.Where(pawn =>
            pawn.Position.DistanceToSquared(thing.Position) < range * range
        );

    public static IEnumerable<Pawn> FindPawnsAliveInRange(this Thing thing, float range) =>
        thing.FindPawnsInRange(range).Where(pawn => !pawn.health.Dead);

    public static IEnumerable<Pawn> FindPawnsInRange(this ThingComp comp, float range) =>
        comp.parent.FindPawnsInRange(range);

    public static IEnumerable<Pawn> FindPawnsAliveInRange(this ThingComp comp, float range) =>
        comp.parent.FindPawnsInRange(range).Where(pawn => !pawn.health.Dead);

    public static bool ConsumePower(this Thing thing, float requiredVal)
    {
        var powerComp = thing.TryGetComp<CompPowerTrader>();
        if (powerComp?.PowerOn != true)
            return false;

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
                battery.SetStoredEnergyPct(
                    (battery.StoredEnergy - toConsume) / battery.Props.storedEnergyMax
                );
                Msg.Debug("ConsumePower: Clear.");
                return true;
            }

            toConsume -= battery.StoredEnergy;
            battery.SetStoredEnergyPct(0f);
            Msg.Debug($"ConsumePower: Cost {battery.StoredEnergy}.");

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

    public static bool ConsumePower(this ThingComp comp, float requiredVal) =>
        comp.parent.ConsumePower(requiredVal);

    public static void SpawnThingWithPowerCost(
        this Thing thing,
        string defName,
        int minCount,
        int refillCount,
        int eachItemCost = 0
    )
    {
        if (thing is not IHaulDestination)
        {
            Msg.Error("SpawnThingWithPowerCost: Parent is not IHaulDestination.");
            return;
        }

        var slotGroup = thing.GetSlotGroup();
        if (slotGroup is null)
        {
            Msg.Error("SpawnThingWithPowerCost: SlotGroup is null.");
            return;
        }

        var presentCount = slotGroup
            .HeldThings.Where(thing => thing.def.defName == defName)
            .Sum(thing => thing.stackCount);

        if (presentCount >= minCount)
        {
            Msg.Debug("SpawnThingWithPowerCost: Sufficient count.");
            return;
        }

        var targetDef = ThingDef.Named(defName);
        if (targetDef == null)
        {
            Msg.Error(
                $"SpawnThingWithPowerCost: There's no thing named {defName} at {thing.Position} setting."
            );
            return;
        }

        var addUpStack = ThingMaker.MakeThing(targetDef);
        var addUpVal =
            refillCount <= targetDef.stackLimit
                ? refillCount - presentCount
                : targetDef.stackLimit - presentCount;

        if (eachItemCost <= 0 || !thing.ConsumePower(addUpVal * eachItemCost))
        {
            Msg.Debug("SpawnThingWithPowerCost: Insufficient power.");
            return;
        }

        addUpStack.stackCount = addUpVal;
        GenPlace.TryPlaceThing(addUpStack, thing.Position, thing.Map, ThingPlaceMode.Near);
    }

    public static void SpawnThingWithPowerCost(
        this ThingComp comp,
        string defName,
        int minCount,
        int refillCount,
        int eachItemCost = 0
    ) => comp.parent.SpawnThingWithPowerCost(defName, minCount, refillCount, eachItemCost);

    public static void SetStackCount(this Thing thing, int count)
    {
        if (count <= 0)
        {
            thing.Destroy();
            return;
        }

        thing.stackCount = count <= thing.def.stackLimit ? count : thing.def.stackLimit;
    }

    public static void ThrowMote(this Thing thing, string s) =>
        MoteMaker.ThrowText(thing.TrueCenter(), thing.Map, s);

    public static void ThrowMote(this ThingComp comp, string s) => ThrowMote(comp.parent, s);

    public static void SpawnAt(this ThingDef def, Map map, IntVec3 pos, int count = 1)
    {
        var thing = ThingMaker.MakeThing(def);
        thing.SetStackCount(count);
        GenPlace.TryPlaceThing(thing, pos, map, ThingPlaceMode.Near);
    }
}
