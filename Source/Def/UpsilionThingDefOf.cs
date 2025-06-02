namespace Overclock;

[DefOf]
public static class OverclockThingDefOf
{
    // public static ThingDef Overclock_FieldTrap_Extreme;

    // Building
    public static ThingDef Overclock_ScienceTerminal;

    static OverclockThingDefOf() =>
        DefOfHelper.EnsureInitializedInCtor(typeof(OverclockThingDefOf));
}
