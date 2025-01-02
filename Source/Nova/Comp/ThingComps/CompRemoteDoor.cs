using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Nova;

public class CompProperties_RemoteDoor : CompProperties
{
  public CompProperties_RemoteDoor()
  {
    compClass = typeof(CompRemoteDoor);
  }
}

public class CompRemoteDoor : ThingComp
{
  private CompPowerTrader _powerTrader;

  public override void PostSpawnSetup(bool respawningAfterLoad)
  {
    base.PostSpawnSetup(respawningAfterLoad);
    _powerTrader = parent.TryGetComp<CompPowerTrader>();

    if (_powerTrader == null) Msg.E($"No CompPowerTrader set with CompRemoteDoor at {parent.Position}");
  }

  public override IEnumerable<Gizmo> CompGetGizmosExtra()
  {
    foreach (var gizmo in base.CompGetGizmosExtra()) yield return gizmo;
    yield return new Command_Toggle
    {
      defaultLabel = "Nova_CompRemoteDoor_Gizmo_Label".Translate(),
      defaultDesc = "Nova_CompRemoteDoor_Gizmo_Desc".Translate(),
      icon = TexCommand.HoldOpen,
      hotKey = KeyBindingDefOf.Misc6,
      isActive = () => parent is Building_Door door && door.Open,
      toggleAction = delegate
      {
        if (_powerTrader != null && _powerTrader.PowerOn)
        {
          if (parent is Building_Door door) SetDoorState(!door.Open);
        }
        else
        {
          MoteMaker.ThrowText(parent.TrueCenter() + new Vector3(0.5f, 0.5f, 0.5f), parent.Map, "未通电");
        }
      }
    };
  }

  private void SetDoorState(bool b)
  {
    if (parent is Building_Door door) door.SetPrivateField("openInt", b);
  }
}
