namespace Overclock.Utility.Extension;

public static class TranslationExtension
{
    public static TaggedString BoolTranslate(this bool value)
    {
        return value ? "Overclock_Enable".Translate() : "Overclock_Disable".Translate();
    }
}
