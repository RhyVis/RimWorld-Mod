namespace Overclock.Utility.Extension;

public static class SkillExtension
{
    public static bool DepleteSkillLevel(this SkillRecord skillRecord, float xp)
    {
        if (skillRecord.levelInt <= 0 || skillRecord.TotallyDisabled || xp <= 0)
            return false;

        while (xp > skillRecord.xpSinceLastLevel)
        {
            --skillRecord.levelInt;
            skillRecord.xpSinceLastLevel += skillRecord.XpRequiredForLevelUp;

            if (skillRecord.levelInt > 0)
                continue;

            skillRecord.levelInt = 0;
            skillRecord.xpSinceLastLevel = 0f;
            return true;
        }

        if (skillRecord.xpSinceLastLevel >= xp)
            skillRecord.xpSinceLastLevel -= xp;

        skillRecord.xpSinceLastLevel = 0;
        return true;
    }
}
