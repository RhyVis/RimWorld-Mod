namespace Overclock.Utility.Extension;

public static class PawnExtension
{
    public static void ApplyHediff(
        [CanBeNull] this Pawn pawn,
        HediffDef hediff,
        float severityAdjust = 1.0f
    )
    {
        if (pawn is null)
            return;

        var target = pawn.health.hediffSet.GetFirstHediffOfDef(hediff);
        if (target is null)
        {
            target = HediffMaker.MakeHediff(hediff, pawn);
            target.Severity = severityAdjust;
            pawn.health.AddHediff(target);
        }

        target.Severity += severityAdjust;
    }

    public static void ApplyHediffWithStat(
        [CanBeNull] this Pawn pawn,
        HediffDef hediff,
        List<StatDef> stats = null,
        float severityAdjust = 1.0f
    )
    {
        if (pawn == null)
            return;
        if (!(stats?.NullOrEmpty() ?? true))
            stats.ForEach(stat => severityAdjust *= pawn.GetStatValue(stat));
        var target = pawn.health.hediffSet.GetFirstHediffOfDef(hediff);
        if (target == null)
        {
            target = HediffMaker.MakeHediff(hediff, pawn);
            target.Severity = severityAdjust;
            pawn.health.AddHediff(target);
        }
        else
        {
            target.Severity += severityAdjust;
        }
    }

    public static void RemoveHediff([CanBeNull] this Pawn pawn, HediffDef hediff)
    {
        var target = pawn?.health.hediffSet.GetFirstHediffOfDef(hediff);
        if (target == null)
            return;
        pawn.health.RemoveHediff(target);
    }

    public static bool HasHediff([CanBeNull] this Pawn pawn, HediffDef hediff)
    {
        return pawn?.health.hediffSet.GetFirstHediffOfDef(hediff) != null;
    }

    public static void DamageBodyPart(
        [CanBeNull] this Pawn pawn,
        BodyPartRecord bodyPart,
        DamageDef def = null,
        float amount = 9999f,
        float armorPenetration = 999f
    )
    {
        if (pawn == null || bodyPart == null)
            return;
        Msg.Debug("Doing damage to " + bodyPart.def.label);
        pawn.TakeDamage(
            new DamageInfo(
                def ?? DamageDefOf.SurgicalCut,
                amount,
                armorPenetration,
                -1f,
                null,
                bodyPart,
                null,
                DamageInfo.SourceCategory.ThingOrUnknown,
                null,
                true,
                true,
                QualityCategory.Normal,
                false
            )
        );
    }

    public static void DamageRandomBodyPart([CanBeNull] this Pawn pawn, float amount = 1f)
    {
        var target = pawn?.health.hediffSet.GetNotMissingParts().RandomElement();
        if (target == null)
            return;
        Msg.Debug("Doing damage to " + target.def.label);
        pawn.TakeDamage(
            new DamageInfo(
                DamageDefOf.SurgicalCut,
                amount,
                999f,
                -1f,
                null,
                target,
                null,
                DamageInfo.SourceCategory.ThingOrUnknown,
                null,
                true,
                true,
                QualityCategory.Normal,
                false
            )
        );
    }

    public static void CrossMapTransfer([CanBeNull] this Pawn pawn, Map map, IntVec3? pos)
    {
        if (pawn is null || map is null)
            return;
        if (pawn.Map == map)
            return;
        pawn.DeSpawnOrDeselect();
        GenSpawn.Spawn(pawn, pos ?? map.Center, map);
    }

    public static bool IsNormalColonist(this Pawn pawn)
    {
        return pawn.IsColonist
            || pawn.IsSlaveOfColony
            || pawn.IsPrisonerOfColony
            || pawn.IsColonyMutantPlayerControlled;
    }
}
