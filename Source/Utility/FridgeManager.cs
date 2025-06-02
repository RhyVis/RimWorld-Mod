using System.Collections.Generic;

namespace Overclock.Utility;

public class FridgeManager
{
    public static Dictionary<int, FridgeManager> UtilityCache = [];

    private static readonly SimpleCurve _powerCurve =
    [
        new CurvePoint(0.0f, -0.1f),
        new CurvePoint(15f, -1f),
    ];

    public List<CompPowerTrader> FridgeCache = [];

    private readonly bool[] _fridgeGrid;
    private readonly Map _map;

    public FridgeManager(Map map)
    {
        _map = map;
        if (!UtilityCache.ContainsKey(map.uniqueID))
            UtilityCache.Add(map.uniqueID, this);
        else
            Msg.Error($"[Fridge] Tried to register a map that already exists: {map.uniqueID}");
        _fridgeGrid = new bool[map.info.NumCells];
    }

    public bool GetAdjustedTemperature(IntVec3 c)
    {
        var index = c.z * _map.info.Size.x + c.x;
        return index > -1 && index < _fridgeGrid.Length && _fridgeGrid[index];
    }

    public void UpdateFridgeGrid(CompPowerTrader thing)
    {
        foreach (
            var intVec3 in GenAdj.OccupiedRect(
                thing.parent.Position,
                thing.parent.Rotation,
                thing.parent.def.size
            )
        )
            _fridgeGrid[intVec3.z * _map.info.Size.x + intVec3.x] = thing.PowerOn;
    }

    public void Tick()
    {
        var count = FridgeCache.Count;
        while (count-- > 0)
        {
            var comp = FridgeCache[count];
            if (_map != comp.parent.Map)
            {
                FridgeCache.Remove(comp);
                Msg.Error("[Fridge] Fridge had invalid map assigned.");
            }
            else
            {
                comp.powerOutputInt =
                    comp.Props.PowerConsumption
                    * _powerCurve.Evaluate(comp.parent.GetRoom().Temperature);
                UpdateFridgeGrid(comp);
            }
        }
    }
}
