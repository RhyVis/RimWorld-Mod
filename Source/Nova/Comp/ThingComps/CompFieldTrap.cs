using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Nova;

public class CompProperties_FieldTrap : CompProperties
{
  public int checkInterval = 120;
  public bool ignoreDistance = false;
  public float range = 1;
  public int stunTick = 180;

  public CompProperties_FieldTrap()
  {
    compClass = typeof(CompFieldTrap);
  }
}

public class CompFieldTrap : ThingComp
{
  private bool _activated;
  private bool _highSpeed;
  private bool _teleport;
  private CompProperties_FieldTrap Props => (CompProperties_FieldTrap)props;

  private int interval => _highSpeed ? Props.checkInterval / 2 : Props.checkInterval;

  public override void PostExposeData()
  {
    base.PostExposeData();
    Scribe_Values.Look(ref _activated, "emitActivated");
    Scribe_Values.Look(ref _teleport, "emitTeleport");
    Scribe_Values.Look(ref _highSpeed, "emitHighSpeed");
  }

  public override IEnumerable<Gizmo> CompGetGizmosExtra()
  {
    foreach (var gizmo in base.CompGetGizmosExtra()) yield return gizmo;
    yield return new Command_Toggle
    {
      defaultLabel = "Nova_CompFieldTrap_Gizmo_Label1".Translate(),
      defaultDesc = "Nova_CompFieldTrap_Gizmo_Desc1".Translate(),
      icon = _activated ? TexCommand.ForbidOff : TexCommand.ForbidOn,
      isActive = () => _activated,
      toggleAction = delegate { _activated = !_activated; }
    };
    yield return new Command_Toggle
    {
      defaultLabel = "Nova_CompFieldTrap_Gizmo_Label2".Translate(),
      defaultDesc = "Nova_CompFieldTrap_Gizmo_Desc2".Translate(),
      icon = TexCommand.FireAtWill,
      isActive = () => _teleport,
      toggleAction = delegate { _teleport = !_teleport; }
    };
    yield return new Command_Toggle
    {
      defaultLabel = "Nova_CompFieldTrap_Gizmo_Label3".Translate(),
      defaultDesc = "Nova_CompFieldTrap_Gizmo_Desc3".Translate(),
      icon = TexCommand.DesirePower,
      isActive = () => _highSpeed,
      toggleAction = delegate { _highSpeed = !_highSpeed; }
    };
  }

  public override void CompTick()
  {
    base.CompTick();
    if (!parent.IsHashIntervalTick(interval) || !_activated ||
        !parent.Spawned) return;
    DoStun();
  }

  private void DoStun()
  {
    var targetPos = parent.Position;
    List<Pawn> pawns;

    if (Props.ignoreDistance)
      pawns = parent.Map.mapPawns.AllPawnsSpawned
        .Where(pawn => !pawn.health.Dead)
        .Where(pawn => (pawn.Faction != null && pawn.Faction.HostileTo(Faction.OfPlayer)) ||
                       (pawn.AnimalOrWildMan() && pawn.InAggroMentalState))
        .Where(pawn => !pawn.IsPrisoner)
        .ToList();
    else
      pawns = this.FindPawnsAliveInRange(Props.range)
        .Where(pawn => (pawn.Faction != null && pawn.Faction.HostileTo(Faction.OfPlayer)) ||
                       (pawn.AnimalOrWildMan() && pawn.InAggroMentalState))
        .Where(pawn => !pawn.IsPrisoner)
        .ToList();

    if (pawns.Count == 0)
      return;

    //SoundDefOf.Psycast_Skip_Entry.PlayOneShot(new TargetInfo(parent.Position, parent.Map));

    if (_teleport)
      foreach (var pawn in pawns)
      {
        pawn.ApplyHediff(NovaHediffDefOf.Nova_SlowHediff);
        pawn.Position = targetPos;
        pawn.jobs.StopAll();
        pawn.stances.stunner.StunFor(Props.stunTick, parent, true, !pawn.stances.stunner.Stunned);
      }
    else
      foreach (var pawn in pawns)
      {
        pawn.ApplyHediff(NovaHediffDefOf.Nova_SlowHediff);
        pawn.jobs.StopAll();
        pawn.stances.stunner.StunFor(Props.stunTick, parent, true, !pawn.stances.stunner.Stunned);
      }
  }
}
