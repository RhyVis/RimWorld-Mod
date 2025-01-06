using System.Collections.Generic;
using AdaptiveStorage;
using Verse;

namespace Nova;

public class Building_WithoutZoneSettings : ThingClass
{
  public override IEnumerable<Gizmo> GetGizmos()
  {
    var copyStr = "CommandCopyZoneSettingsLabel".Translate();
    var pasteStr = "CommandPasteZoneSettingsLabel".Translate();

    foreach (var gizmo in base.GetGizmos())
    {
      if (gizmo is Command_Action act && (act.defaultLabel == copyStr || act.defaultLabel == pasteStr)) continue;
      yield return gizmo;
    }
  }
}
