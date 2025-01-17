﻿using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace Nova;

[DefOf]
[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.WithMembers)]
public static class NovaThingDefOf
{
  public static ThingDef Nova_FieldTrap_Extreme;

  static NovaThingDefOf()
  {
    DefOfHelper.EnsureInitializedInCtor(typeof(NovaThingDefOf));
  }
}
