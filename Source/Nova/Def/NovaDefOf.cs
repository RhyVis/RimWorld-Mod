using RimWorld;
using Verse;

namespace Nova;

[DefOf]
public static class NovaDefOf
{
  public static DesignationDef Nova_FieldTeleportD;

  static NovaDefOf()
  {
    DefOfHelper.EnsureInitializedInCtor(typeof(NovaDefOf));
  }
}
