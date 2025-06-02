using System.Reflection;
using HarmonyLib;

namespace Overclock;

[StaticConstructorOnStartup]
public static class HarmonyInit
{
    static HarmonyInit() =>
        new Harmony("Rhynia.Mod.Overclock").PatchAll(Assembly.GetExecutingAssembly());
}
