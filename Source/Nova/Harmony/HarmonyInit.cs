using System.Reflection;
using HarmonyLib;
using Verse;

namespace Nova;

[StaticConstructorOnStartup]
public static class HarmonyInit
{
  static HarmonyInit()
  {
    new Harmony("Rhynia.Works.Nova.Harmony").PatchAll(Assembly.GetExecutingAssembly());
  }
}
