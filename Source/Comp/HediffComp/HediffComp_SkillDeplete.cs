namespace Overclock;

public class HediffCompProperties_SkillDeplete : HediffCompProperties
{
    public int checkInterval = 10000;
    public int depleteAmount = 5000;

    public HediffCompProperties_SkillDeplete()
    {
        compClass = typeof(HediffComp_SkillDeplete);
    }
}

public class HediffComp_SkillDeplete : HediffComp
{
    private int _ticker;
    private HediffCompProperties_SkillDeplete Props => (HediffCompProperties_SkillDeplete)props;

    public override void CompPostMake()
    {
        base.CompPostMake();
        _ticker = Props.checkInterval;
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        base.CompPostTick(ref severityAdjustment);
        if (_ticker > 0)
        {
            _ticker--;
            return;
        }

        DoDeplete();
        _ticker = Props.checkInterval;
    }

    private void DoDeplete()
    {
        var skillRecord = parent.pawn.skills?.skills.RandomElement();
        if (skillRecord == null)
            return;
        if (!skillRecord.DepleteSkillLevel(Props.depleteAmount))
            return;

        MoteMaker.ThrowText(
            parent.pawn.TrueCenter() + new Vector3(0.5f, 0.5f, 0.5f),
            parent.pawn.Map,
            "Overclock_HediffComp_SkillDeplete_Mote".Translate(
                skillRecord.def.label,
                Props.depleteAmount
            )
        );
    }
}
