namespace Overclock.Utility.Extension;

public static class RoomExtension
{
    public static IEnumerable<Thing> ThingGrid([CanBeNull] this Room room)
    {
        if (room == null)
            return [];

        return room.Cells.SelectMany(cell => cell.GetThingList(room.Map));
    }
}
