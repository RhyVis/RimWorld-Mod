using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace Nova;

[DefOf, UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.WithMembers)]
public static class NovaHediffDefOf
{
  public static HediffDef Nova_BodyPartWorking;

  // Hediff_Misc Bad
  public static HediffDef Nova_BerserkHediff;
  public static HediffDef Nova_HungerHediff;
  public static HediffDef Nova_VomitHediff;
  public static HediffDef Nova_UpsetHediff;
  public static HediffDef Nova_SuppressedHediff;
  public static HediffDef Nova_SlowHediff;
  public static HediffDef Nova_DeathHediff;
}
