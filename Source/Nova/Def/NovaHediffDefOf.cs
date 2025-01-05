using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace Nova;

[DefOf, UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.WithMembers)]
public static class NovaHediffDefOf
{
  // Hediff_Misc
  public static HediffDef Nova_BodyPartWorking;
  public static HediffDef Nova_SlowHediff;

  static NovaHediffDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(NovaHediffDefOf));
}
