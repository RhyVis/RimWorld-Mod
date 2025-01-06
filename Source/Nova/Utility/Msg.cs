using Verse;

namespace Nova;

public static class Msg
{
  public static void D(string s)
  {
    if (Nova_ModSettings.Debug) Log.Message($"[Nova] {s}");
  }

  public static void E(string s)
  {
    Log.Error($"[Nova] {s}");
  }
}
