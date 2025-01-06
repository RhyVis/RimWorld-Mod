using RimWorld;
using Verse;

namespace Nova;

public class IngestionOutcomeDoer_AgeTweaker : IngestionOutcomeDoer
{
  private readonly int offsetDays = 0;

  protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
  {
    var d = (double)offsetDays;
    if (d == 0d) return;
    if (d > 0d)
    {
      if (pawn.RaceProps.Humanlike || pawn.RaceProps.baseBodySize <= 1f)
        pawn.ageTracker.AgeBiologicalTicks += (long)(d * 60000);
      else
        pawn.ageTracker.AgeBiologicalTicks += (long)(d / pawn.RaceProps.baseBodySize);
    }
    else
    {
      var offsetDaysAbs = -d;
      if (pawn.RaceProps.Humanlike || pawn.RaceProps.baseBodySize <= 1f)
      {
        if (pawn.ageTracker.AgeBiologicalTicks < offsetDaysAbs)
          pawn.ageTracker.AgeBiologicalTicks = 0L;
        else
          pawn.ageTracker.AgeBiologicalTicks -= (long)(offsetDaysAbs * 60000);
      }
      else
      {
        var offsetAdjusted = (long)(offsetDaysAbs / pawn.RaceProps.baseBodySize);
        if (pawn.ageTracker.AgeBiologicalTicks < offsetAdjusted)
          pawn.ageTracker.AgeBiologicalTicks = 0L;
        else
          pawn.ageTracker.AgeBiologicalTicks -= offsetAdjusted * 60000;
      }
    }
  }
}
