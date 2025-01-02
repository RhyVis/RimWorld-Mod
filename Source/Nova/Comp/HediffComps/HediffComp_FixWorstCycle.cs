using RimWorld;
using Verse;

namespace Nova;

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
  private int _ticker;
  private HediffCompProperties_FixWorstCycle Props => (HediffCompProperties_FixWorstCycle)props;

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

    var fixText = HealthUtility.FixWorstHealthCondition(parent.pawn);
    if (fixText.NullOrEmpty()) return;
    _ticker = Props.checkInterval;
    if (!PawnUtility.ShouldSendNotificationAbout(parent.pawn)) return;
    Messages.Message(fixText, parent.pawn, MessageTypeDefOf.PositiveEvent);
  }
}
