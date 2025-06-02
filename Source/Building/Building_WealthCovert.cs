using System.Text;

namespace Overclock;

public class Building_WealthCovert : AdaptiveStorage.ThingClass
{
    private bool _autoConvert = true;
    private bool _autoPlace = true;
    private long _leftoverSilverValue;
    private int _mode; // 0: Convert to silver, 1: Convert to gold
    private int _ticks = 1250;

    private string ModeLabel => _mode == 0 ? ThingDefOf.Silver.label : ThingDefOf.Gold.label;

    private TaggedString AutoCovertString => _autoConvert.BoolTranslate();
    private TaggedString AutoPlaceString => _autoPlace.BoolTranslate();

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var gizmo in base.GetGizmos())
            yield return gizmo;
        yield return new Command_Toggle
        {
            defaultLabel = "Overclock_CompResourceCovert_GizmoA_Label".Translate(AutoCovertString),
            defaultDesc = "Overclock_CompResourceCovert_GizmoA_Desc".Translate(AutoCovertString),
            icon = TexCommand.DesirePower,
            isActive = () => _autoConvert,
            toggleAction = delegate
            {
                _autoConvert = !_autoConvert;
                _ticks = 1250;
            },
        };
        yield return new Command_Toggle
        {
            defaultLabel = "Overclock_CompResourceCovert_GizmoAP_Label".Translate(AutoPlaceString),
            defaultDesc = "Overclock_CompResourceCovert_GizmoAP_Desc".Translate(AutoPlaceString),
            icon = TexCommand.DesirePower,
            isActive = () => _autoPlace,
            toggleAction = delegate
            {
                _autoPlace = !_autoPlace;
                _ticks = 1250;
            },
        };
        yield return new Command_Action
        {
            defaultLabel = "Overclock_CompResourceCovert_Gizmo_Label".Translate(),
            defaultDesc = "Overclock_CompResourceCovert_Gizmo_Desc".Translate(),
            icon = TexCommand.ForbidOff,
            action = DoCovert,
        };
        yield return new Command_Action
        {
            defaultLabel = "Overclock_CompResourceCovert_GizmoP_Label".Translate(),
            defaultDesc = "Overclock_CompResourceCovert_GizmoP_Desc".Translate(),
            icon = TexCommand.ForbidOff,
            action = delegate
            {
                TryPlace(true);
            },
        };
        yield return new Command_Action
        {
            defaultLabel = "Overclock_CompResourceCovert_Gizmo1_Label".Translate(ModeLabel),
            defaultDesc = "Overclock_CompResourceCovert_Gizmo1_Desc".Translate(),
            icon = _mode == 0 ? ThingDefOf.Silver.uiIcon : ThingDefOf.Gold.uiIcon,
            action = delegate
            {
                _mode = _mode == 0 ? 1 : 0;
            },
        };
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref _mode, "cov_mode");
        Scribe_Values.Look(ref _autoConvert, "cov_auto");
        Scribe_Values.Look(ref _autoPlace, "cov_place");
        Scribe_Values.Look(ref _leftoverSilverValue, "cov_leftover");
    }

    public override string GetInspectString()
    {
        var sb = new StringBuilder();
        sb.Append(base.GetInspectString());
        sb.AppendLineIfNotEmpty();
        sb.Append("Overclock_CompResourceCovert_Info".Translate(_leftoverSilverValue));
        return sb.ToString();
    }

    public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
    {
        base.Destroy(mode);
        if (_leftoverSilverValue > 0)
            TryPlace();
    }

    public override void Tick()
    {
        base.Tick();
        TickDown(1);
    }

    public override void TickRare()
    {
        base.TickRare();
        TickDown(250);
    }

    public override void TickLong()
    {
        base.TickLong();
        TickDown(2500);
    }

    private void TickDown(int ticks)
    {
        if (!_autoConvert)
            return;

        if (_ticks <= 0)
            DoCovert();
        else
            _ticks -= ticks;
    }

    private void DoCovert()
    {
        var things = GetSlotGroup()
            .HeldThings.Where(t =>
                _mode == 0 ? t.def != ThingDefOf.Silver : t.def != ThingDefOf.Gold
            )
            .ToList();
        if (things.Count == 0)
            return;

        var silverValue = 0;
        foreach (var thing in things)
        {
            silverValue += (int)(thing.MarketValue * thing.stackCount);
            thing.Destroy();
        }

        MoteMaker.ThrowText(
            this.TrueCenter() + new Vector3(0.5f, 0.5f, 0.5f),
            Map,
            "Overclock_CompResourceCovert_Mote".Translate(silverValue)
        );

        _leftoverSilverValue += silverValue;

        TryPlace();

        _ticks = 1250;
    }

    private void TryPlace(bool calledByPlayer = false)
    {
        if (_leftoverSilverValue <= 0)
            return;
        if (!calledByPlayer && !_autoPlace)
            return;

        if (_mode == 0)
        {
            var silverLimit = ThingDefOf.Silver.stackLimit;

            while (_leftoverSilverValue > 0)
            {
                var stack = ThingMaker.MakeThing(ThingDefOf.Silver);
                if (_leftoverSilverValue > silverLimit)
                {
                    stack.stackCount = silverLimit;
                    if (GenPlace.TryPlaceThing(stack, Position, Map, ThingPlaceMode.Near))
                    {
                        _leftoverSilverValue -= silverLimit;
                    }
                    else
                    {
                        Msg.Error($"Failed to place silver {silverLimit} at {Position}");
                        this.ThrowMote(
                            "Overclock_CompResourceCovert_MoteFailure".Translate(
                                _leftoverSilverValue
                            )
                        );
                        _autoPlace = false;
                        break;
                    }
                }
                else
                {
                    stack.stackCount = (int)_leftoverSilverValue;
                    if (GenPlace.TryPlaceThing(stack, Position, Map, ThingPlaceMode.Near))
                    {
                        _leftoverSilverValue = 0;
                    }
                    else
                    {
                        Msg.Error($"Failed to place silver {_leftoverSilverValue} at {Position}");
                        this.ThrowMote(
                            "Overclock_CompResourceCovert_MoteFailure".Translate(
                                _leftoverSilverValue
                            )
                        );
                        _autoPlace = false;
                        break;
                    }
                }
            }
        }
        else
        {
            var goldLimit = ThingDefOf.Gold.stackLimit;

            while (_leftoverSilverValue > 0)
            {
                var goldLimitValue = (int)(goldLimit * ThingDefOf.Gold.BaseMarketValue);
                var stack = ThingMaker.MakeThing(ThingDefOf.Gold);
                if (_leftoverSilverValue > goldLimitValue)
                {
                    stack.stackCount = goldLimit;
                    if (GenPlace.TryPlaceThing(stack, Position, Map, ThingPlaceMode.Near))
                    {
                        _leftoverSilverValue -= goldLimitValue;
                    }
                    else
                    {
                        Msg.Error($"Failed to place gold {goldLimit} at {Position}");
                        this.ThrowMote(
                            "Overclock_CompResourceCovert_MoteFailure".Translate(
                                _leftoverSilverValue
                            )
                        );
                        _autoPlace = false;
                        break;
                    }
                }
                else
                {
                    var goldCount = (int)(_leftoverSilverValue / ThingDefOf.Gold.BaseMarketValue);
                    stack.stackCount = goldCount;
                    if (GenPlace.TryPlaceThing(stack, Position, Map, ThingPlaceMode.Near))
                    {
                        _leftoverSilverValue = 0;
                    }
                    else
                    {
                        Msg.Error($"Failed to place gold {goldCount} at {Position}");
                        this.ThrowMote(
                            "Overclock_CompResourceCovert_MoteFailure".Translate(
                                _leftoverSilverValue
                            )
                        );
                        _autoPlace = false;
                        break;
                    }
                }
            }
        }
    }
}
