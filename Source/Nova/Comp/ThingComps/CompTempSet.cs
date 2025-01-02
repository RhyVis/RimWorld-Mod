using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Nova;

public class CompProperties_TempSet : CompProperties
{
  public CompProperties_TempSet()
  {
    compClass = typeof(CompTempSet);
  }
}

public class CompTempSet : ThingComp
{
  private static readonly int TargetTempIntDefault = 21;
  private bool _activated;
  private int _targetTempInt = 21;

  public override void CompTick()
  {
    base.CompTick();
    if (!_activated || !parent.Spawned) return;
    TweakTemperature();
  }

  public override void PostExposeData()
  {
    base.PostExposeData();
    Scribe_Values.Look(ref _activated, "activated");
    Scribe_Values.Look(ref _targetTempInt, "targetTempInt");
  }

  public override string CompInspectStringExtra()
  {
    var sb = new StringBuilder();
    sb.Append(base.CompInspectStringExtra());
    sb.AppendLineIfNotEmpty();
    sb.Append("Nova_CompTempSet_Mote2".Translate(_targetTempInt));
    return sb.ToString();
  }

  public override IEnumerable<Gizmo> CompGetGizmosExtra()
  {
    foreach (var gizmo in base.CompGetGizmosExtra()) yield return gizmo;

    yield return new Command_Toggle
    {
      defaultLabel = "Nova_CompTempSet_Gizmo_Act_Label".Translate(),
      defaultDesc = "Nova_CompTempSet_Gizmo_Act_Desc".Translate(),
      icon = TexCommand.ToggleVent,
      isActive = () => _activated,
      toggleAction = () => _activated = !_activated
    };

    yield return new Command_Action
    {
      defaultLabel = "-100",
      defaultDesc = "-100℃",
      icon = ContentFinder<Texture2D>.Get("UI/Commands/TempLower"),
      action = () => TweakTarget(-100)
    };

    yield return new Command_Action
    {
      defaultLabel = "-10",
      defaultDesc = "-10℃",
      icon = ContentFinder<Texture2D>.Get("UI/Commands/TempLower"),
      action = () => TweakTarget(-10)
    };

    yield return new Command_Action
    {
      defaultLabel = "-1",
      defaultDesc = "-1℃",
      icon = ContentFinder<Texture2D>.Get("UI/Commands/TempLower"),
      action = () => TweakTarget(-1)
    };

    yield return new Command_Action
    {
      defaultLabel = "Nova_CompTempSet_Gizmo_Reset_Label".Translate(),
      defaultDesc = "Nova_CompTempSet_Gizmo_Reset_Desc".Translate(),
      icon = ContentFinder<Texture2D>.Get("UI/Commands/TempReset"),
      action = () => _targetTempInt = TargetTempIntDefault
    };

    yield return new Command_Action
    {
      defaultLabel = "+1",
      defaultDesc = "+1℃",
      icon = ContentFinder<Texture2D>.Get("UI/Commands/TempRaise"),
      action = () => TweakTarget(1)
    };

    yield return new Command_Action
    {
      defaultLabel = "+10",
      defaultDesc = "+10℃",
      icon = ContentFinder<Texture2D>.Get("UI/Commands/TempRaise"),
      action = () => TweakTarget(10)
    };

    yield return new Command_Action
    {
      defaultLabel = "+100",
      defaultDesc = "+100℃",
      icon = ContentFinder<Texture2D>.Get("UI/Commands/TempRaise"),
      action = () => TweakTarget(100)
    };
  }

  private void TweakTarget(int offset)
  {
    SoundDefOf.DragSlider.PlayOneShotOnCamera();
    _targetTempInt += offset;
    _targetTempInt = Mathf.Clamp(_targetTempInt, -273, 1000);
    this.ThrowMote("Nova_CompTempSet_Mote2".Translate(_targetTempInt));
  }

  private void TweakTemperature()
  {
    var room = parent.GetRoom();
    if (room == null)
    {
      this.ThrowMote("Nova_CompTempSet_Mote1".Translate());
      _activated = false;
      return;
    }

    var target = (float)_targetTempInt;
    if (room.Temperature < target || room.Temperature > target) room.Temperature = target;
  }
}
