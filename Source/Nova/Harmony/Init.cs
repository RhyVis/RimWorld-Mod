using HarmonyLib;
using Verse;

namespace Nova;

[StaticConstructorOnStartup]
public static class Init
{
  static Init()
  {
    var harmony = new Harmony("Rhynia.Works.Nova");
    harmony.PatchAll();
  }
}
