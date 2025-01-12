using Verse;

namespace Nova;

public static class Msg
{
  public static void Debug(string s)
  {
    if (Nova_ModSettings.Debug) Log.Message($"[Nova] {s}");
  }
  
  public static void Out(string s)
  {
    Log.Message($"[Nova] {s}");
  }

  public static void Error(string s)
  {
    Log.Error($"[Nova] {s}");
  }
}
