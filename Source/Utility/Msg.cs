namespace Overclock.Utility;

static class Msg
{
    public static void Debug(string s)
    {
        Log.Message($"[Overclock] {s}");
    }

    public static void Out(string s)
    {
        Log.Message($"[Overclock] {s}");
    }

    public static void Error(string s)
    {
        Log.Error($"[Overclock] {s}");
    }
}
