using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace Nova;

public class Nova_Mod : Mod
{
  public Nova_Mod(ModContentPack content) : base(content)
  {
    Log.Message("[Nova] Hello RimWorld!");
  }

  public override void DoSettingsWindowContents(Rect inRect)
  {
    var listing = new Listing_Standard();
    listing.Begin(inRect);
    listing.CheckboxLabeled("Nova_SettingCat_DebugMsg_Label".Translate(), ref Nova_ModSettings.Debug);
    listing.End();
  }

  public override string SettingsCategory()
  {
    return "Nova_SettingCat_Label".Translate();
  }
}

[UsedImplicitly]
public class Nova_ModSettings : ModSettings
{
  public static bool Debug;

  public override void ExposeData()
  {
    Scribe_Values.Look(ref Debug, "bDebug");
  }
}
