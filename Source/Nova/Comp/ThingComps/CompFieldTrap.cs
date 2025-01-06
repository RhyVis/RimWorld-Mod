using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Nova;

public class CompProperties_FieldTrap : CompProperties
{
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
  private bool _attack;
  private bool _highSpeed;
  private bool _teleport;
  private CompProperties_FieldTrap Props => (CompProperties_FieldTrap)props;

  private int Interval => _highSpeed ? 60 : 120;

  public override void PostExposeData()
  {
    base.PostExposeData();
    Scribe_Values.Look(ref _activated, "emitActivated");
    Scribe_Values.Look(ref _teleport, "emitTeleport");
    Scribe_Values.Look(ref _highSpeed, "emitHighSpeed");
    Scribe_Values.Look(ref _attack, "emitAttack");
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
    yield return new Command_Toggle
    {
      defaultLabel = "Nova_CompFieldTrap_Gizmo_Label4".Translate(),
      defaultDesc = "Nova_CompFieldTrap_Gizmo_Desc4".Translate(),
      icon = TexCommand.Attack,
      isActive = () => _attack,
      toggleAction = delegate { _attack = !_attack; }
    };
  }

  public override void CompTick()
  {
    base.CompTick();
    if (!parent.IsHashIntervalTick(Interval) || !_activated ||
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
        .Where(pawn => (pawn.Faction is not null && pawn.Faction.HostileTo(Faction.OfPlayer)) ||
                       (pawn.AnimalOrWildMan() && pawn.InAggroMentalState))
        .Where(pawn => !pawn.IsPrisoner)
        .ToList();
    else
      pawns = this.FindPawnsAliveInRange(Props.range)
        .Where(pawn => (pawn.Faction is not null && pawn.Faction.HostileTo(Faction.OfPlayer)) ||
                       (pawn.AnimalOrWildMan() && pawn.InAggroMentalState))
        .Where(pawn => !pawn.IsPrisoner)
        .ToList();

    var des = parent.Map.designationManager.SpawnedDesignationsOfDef(NovaDefOf.Nova_FieldTeleportD);
    foreach (var d in des)
    {
      var pawn = (Pawn)d.target.Thing;
      if (!pawns.Contains(pawn))
        pawns.Add(pawn);
    }

    if (pawns.Count == 0)
      return;

    if (_teleport)
      foreach (var pawn in pawns)
      {
        pawn.ApplyHediff(NovaHediffDefOf.Nova_SlowHediff);
        pawn.Position = targetPos;
        pawn.jobs.StopAll();
        pawn.stances.stunner.StunFor(Props.stunTick, parent, true, !pawn.stances.stunner.Stunned);
        if (pawn.Downed)
          parent.Map.designationManager.TryRemoveDesignationOn(pawn, NovaDefOf.Nova_FieldTeleportD);
      }
    else
      foreach (var pawn in pawns)
      {
        pawn.ApplyHediff(NovaHediffDefOf.Nova_SlowHediff);
        pawn.jobs.StopAll();
        pawn.stances.stunner.StunFor(Props.stunTick, parent, true, !pawn.stances.stunner.Stunned);
        if (pawn.Downed)
          parent.Map.designationManager.TryRemoveDesignationOn(pawn, NovaDefOf.Nova_FieldTeleportD);
      }

    if (!_attack)
      return;
    foreach (var pawn in pawns)
      pawn.health.hediffSet.GetNotMissingParts()
        .Where(record => record.def.defName.ContainsAnyOfIgnoreCase("brain", "heart", "processor"))
        .ToList()
        .ForEach(record => pawn.DamageBodyPart(record, DamageDefOf.Cut));
    _attack = false;
  }
}
