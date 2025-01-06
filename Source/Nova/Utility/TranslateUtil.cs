using Verse;

namespace Nova;

public static class TranslateUtil
{
  public static TaggedString BoolTranslate(this bool b)
  {
    return b ? "Nova_U_Enable".Translate() : "Nova_U_Disable".Translate();
  }
}
