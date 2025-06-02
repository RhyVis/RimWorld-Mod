namespace Overclock;

public class HediffCompProperties_FixWorstCycle : HediffCompProperties
{
    public int checkInterval = 1250;

    public HediffCompProperties_FixWorstCycle()
    {
        compClass = typeof(HediffComp_FixWorstCycle);
    }
}

public class HediffComp_FixWorstCycle : HediffComp
{
    private int _ticker = -1;
    private HediffCompProperties_FixWorstCycle Props => (HediffCompProperties_FixWorstCycle)props;

    public override void CompExposeData()
    {
        base.CompExposeData();
        Scribe_Values.Look(ref _ticker, "ticker", 0);
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        base.CompPostTick(ref severityAdjustment);
        if (_ticker < 0)
            _ticker = Props.checkInterval;

        if (_ticker > 0)
        {
            _ticker--;
            return;
        }

        var fixText = HealthUtility.FixWorstHealthCondition(parent.pawn);
        if (fixText.NullOrEmpty())
            return;
        _ticker = Props.checkInterval;
        if (!PawnUtility.ShouldSendNotificationAbout(parent.pawn))
            return;
        Messages.Message(fixText, parent.pawn, MessageTypeDefOf.PositiveEvent);
    }
}
