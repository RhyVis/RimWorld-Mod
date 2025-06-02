namespace Overclock.Utility.Extension;

public static class ObjectExtension
{
    public static bool NotEqualToAnyOf(this object obj, params object[] values) =>
        values.Any(value => obj != value);

    public static bool NotEqualToAllOf(this object obj, params object[] values) =>
        values.All(value => obj != value);
}
