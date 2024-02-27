using SimaiParserWithAntlr.I18nModule;
using SimaiParserWithAntlr.NoteLayerParser;
using SimaiParserWithAntlr.NoteLayerParser.DataModels;
using SimaiParserWithAntlr.NoteLayerParser.Enums;
using SimaiParserWithAntlr.NoteLayerParser.Notes;
using Xunit.Abstractions;

namespace ParserTest.NoteBlockTest;

public class NoteBlockTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const double EPS = 1e-5;

    private static readonly Dictionary<string, Func<NoteBase, bool>> BUTTON_CHECK_MAP = Enum
        .GetValues(typeof(ButtonEnum))
        .Cast<ButtonEnum>()
        .Where(button => button != ButtonEnum.Unknown)
        .ToDictionary(
            ButtonEnumExt.ToFormattedString,
            button => new Func<NoteBase, bool>(note =>
            {
                ButtonEnum btn;
                switch (note)
                {
                    case TapNote tap:
                        btn = tap.Button;
                        break;
                    case HoldNote hold:
                        btn = hold.Button;
                        break;
                    default:
                        return false;
                }

                return btn == button;
            })
        );

    private static readonly Dictionary<DurationTypeEnum, KeyValuePair<string, Func<NoteBase, bool>>>
        DURATION_CHECK_MAP = new()
        {
            {
                DurationTypeEnum.Fraction,
                CreateDurationCheck("[8:1]", DurationTypeEnum.Fraction, 8,
                    1)
            },
            {
                DurationTypeEnum.BpmFraction,
                CreateDurationCheck("[120#8:1]", DurationTypeEnum.BpmFraction, 8,
                    1, bpm: 120)
            },
            { DurationTypeEnum.Time, CreateDurationCheck("[#0.5]", DurationTypeEnum.Time, time: 0.5f) },
            { DurationTypeEnum.Empty, CreateDurationCheck("", DurationTypeEnum.Empty) },
            {
                DurationTypeEnum.BpmTime,
                CreateDurationCheck("[120#0.5]", DurationTypeEnum.BpmTime, time: 0.5f, bpm: 120)
            },
            {
                DurationTypeEnum.DelayFraction,
                CreateDurationCheck("[0.3##8:1]", DurationTypeEnum.DelayFraction, 8,
                    1, 0.3f)
            },
            {
                DurationTypeEnum.DelayTime,
                CreateDurationCheck("[0.3##1.5]", DurationTypeEnum.DelayTime, delay: 0.3f, time: 1.5f)
            },
            {
                DurationTypeEnum.DelayBpmFraction,
                CreateDurationCheck("[0.3##120#8:1]", DurationTypeEnum.DelayBpmFraction,
                    8, 1, 0.3f, bpm: 120)
            }
        };

    private static readonly Dictionary<string, Func<NoteBase, bool>> AREA_CHECK_MAP = Enum.GetValues(typeof(AreaEnum))
        .Cast<AreaEnum>()
        .Where(area => area != AreaEnum.Unknown)
        .ToDictionary(
            AreaEnumExt.ToFormattedString,
            area => new Func<NoteBase, bool>(note =>
            {
                AreaEnum noteArea;

                switch (note)
                {
                    case TouchNote touch:
                        noteArea = touch.Area;
                        break;
                    case TouchHoldNote touchHold:
                        noteArea = touchHold.Area;
                        break;
                    default:
                        return false;
                }

                return noteArea == area;
            })
        );

    public NoteBlockTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    private static KeyValuePair<string, Func<NoteBase, bool>> CreateDurationCheck(
        string durationString, DurationTypeEnum type, int? fracDenominator = null, int? fracNumerator = null,
        float? delay = null, float? time = null, float? bpm = null)
    {
        return KeyValuePair.Create(durationString, (Func<NoteBase, bool>)(note =>
        {
            NoteDuration duration;
            switch (note)
            {
                case HoldNote hold:
                    duration = hold.Duration;
                    break;
                case TouchHoldNote touchHold:
                    duration = touchHold.Duration;
                    break;
                default:
                    return false;
            }

            return duration.Type == type &&
                   (!fracDenominator.HasValue || duration.FracDenominator == fracDenominator.Value) &&
                   (!fracNumerator.HasValue || duration.FracNumerator == fracNumerator.Value) &&
                   (!delay.HasValue || FloatEquals(duration.Delay, delay.Value)) &&
                   (!time.HasValue || FloatEquals(duration.Time, time.Value)) &&
                   (!bpm.HasValue || FloatEquals(duration.Bpm, bpm.Value));
        }));
    }

    private static bool FloatEquals(float a, float b)
    {
        return Math.Abs(a - b) < EPS;
    }


    /**
     * Debug test entrypoint. Use it with debug breakpoint.
     */
    [Fact]
    public void DebugTemporaryTest()
    {
        var parser = NoteParser.GenerateFromText(@"1,2b,3bx,4x,
1h,2bh,3bxh,4hxb,5hhbbxx,6xxhb,
1h[8:1],2bh[#0.5],3bxh[120#8:1],4hxb[2.5##8:1],5hhbbxx[2.5##1.5],6xxhb[2.5##120#8:1],
A1,B2f,Cf,C1,C2f,D3ff,E4,
Ch,Cfh,Chf,C1fh,C2h,Chfhh,
Ch[8:1],Cfh[#0.5],Chf[120#8:1],C1fh[2.5##8:1],C2h[2.5##1.5],Chfhh[2.5##120#8:1],

1-5[8:1],2-5-1[#0.5],3bx-5[120#8:1],4xbx-5-1[2.5##8:1],5-2-5b[2.5##1.5],6-2-6[2.5##120#8:1]b,
1b-5b[8:1],1xb-5[8:1]-1[8:1]b,
1-5[8:1]*-6[8:1],1bx-5b[8:1]*-6[8:1]b,1xb-5[8:1]b*-6b[8:1],1-5-1b[8:1]*-6-1[8:1]b,1-5[8:1]*-6[2.5##120#8:1],

1/2,1/3/5,1`2,1/2`3/4,1`2/3`4,
");
        foreach (var noteGroup in parser.NoteGroupList)
        {
            _testOutputHelper.WriteLine($"{noteGroup.GetFormattedString()}");
        }
        _testOutputHelper.WriteLine("");

        foreach (var warn in parser.WarningList)
        {
            _testOutputHelper.WriteLine(warn.GetFormattedInfo(parser.RawText));
        }
        _testOutputHelper.WriteLine("");
        foreach (var err in parser.ErrorList)
        {
            _testOutputHelper.WriteLine(err.GetFormattedInfo(parser.RawText));
        }
    }

    /**
     * tap test 1.
     * correct format
     */
    [Fact]
    public void Tap_CorrectFormat()
    {
        Dictionary<string, Func<NoteBase, bool>> markCheckMap = new()
        {
            { "", note => note is TapNote { IsBreak: false, IsEx: false } },
            { "b", note => note is TapNote { IsBreak: true, IsEx: false } },
            { "x", note => note is TapNote { IsBreak: false, IsEx: true } },
            { "bx", note => note is TapNote { IsBreak: true, IsEx: true } }
        };

        new CheckChain().Iter(BUTTON_CHECK_MAP).Iter(markCheckMap).Check();
    }

    /**
     * tap test 2.
     * duplicate marks
     */
    [Fact]
    public void Tap_DuplicateNoteMarks()
    {
        Dictionary<string, Func<NoteBase, bool>> markCheckMap = new()
        {
            { "bb", note => note is TapNote { IsBreak: true, IsEx: false } },
            { "xx", note => note is TapNote { IsBreak: false, IsEx: true } },
            { "bxbbx", note => note is TapNote { IsBreak: true, IsEx: true } }
        };

        new CheckChain().Iter(BUTTON_CHECK_MAP).Iter(markCheckMap).RequireWarning(I18nKeyEnum.DuplicateNoteMarks)
            .Check();
    }

    /**
     * hold test 1.
     * correct format
     */
    [Fact]
    public void Hold_CorrectFormat()
    {
        Dictionary<string, Func<NoteBase, bool>> markCheckMap = new()
        {
            { "h", note => note is HoldNote { IsBreak: false, IsEx: false } },
            { "bh", note => note is HoldNote { IsBreak: true, IsEx: false } },
            { "xh", note => note is HoldNote { IsBreak: false, IsEx: true } },
            { "bxh", note => note is HoldNote { IsBreak: true, IsEx: true } }
        };
        Dictionary<string, Func<NoteBase, bool>> durCheckMap = new()
        {
            { DURATION_CHECK_MAP[DurationTypeEnum.Empty].Key, DURATION_CHECK_MAP[DurationTypeEnum.Empty].Value },
            { DURATION_CHECK_MAP[DurationTypeEnum.Fraction].Key, DURATION_CHECK_MAP[DurationTypeEnum.Fraction].Value },
            { DURATION_CHECK_MAP[DurationTypeEnum.Time].Key, DURATION_CHECK_MAP[DurationTypeEnum.Time].Value },
            {
                DURATION_CHECK_MAP[DurationTypeEnum.BpmFraction].Key,
                DURATION_CHECK_MAP[DurationTypeEnum.BpmFraction].Value
            }
        };

        new CheckChain().Iter(BUTTON_CHECK_MAP).Iter(markCheckMap).Iter(durCheckMap).Check();
    }

    /**
     * hold test 2.
     * unsupported duration type. `[120#0.5]`, `[1.5##8:1]` etc.
     */
    [Fact]
    public void Hold_UnsupportedDurationType()
    {
        Dictionary<string, Func<NoteBase, bool>> markCheckMap = new()
        {
            { "h", note => note is HoldNote { IsBreak: false, IsEx: false } },
            { "bh", note => note is HoldNote { IsBreak: true, IsEx: false } },
            { "xh", note => note is HoldNote { IsBreak: false, IsEx: true } },
            { "bxh", note => note is HoldNote { IsBreak: true, IsEx: true } }
        };
        Dictionary<string, Func<NoteBase, bool>> durCheckMap = new()
        {
            { DURATION_CHECK_MAP[DurationTypeEnum.BpmTime].Key, DURATION_CHECK_MAP[DurationTypeEnum.BpmTime].Value },
            {
                DURATION_CHECK_MAP[DurationTypeEnum.DelayFraction].Key,
                DURATION_CHECK_MAP[DurationTypeEnum.DelayFraction].Value
            },
            {
                DURATION_CHECK_MAP[DurationTypeEnum.DelayTime].Key, DURATION_CHECK_MAP[DurationTypeEnum.DelayTime].Value
            },
            {
                DURATION_CHECK_MAP[DurationTypeEnum.DelayBpmFraction].Key,
                DURATION_CHECK_MAP[DurationTypeEnum.DelayBpmFraction].Value
            }
        };

        new CheckChain().Iter(BUTTON_CHECK_MAP).Iter(markCheckMap).Iter(durCheckMap)
            .RequireWarning(I18nKeyEnum.UnsupportedDurationType).Check();
    }

    /**
     * hold test 3.
     * hold mark does not at the end of note.
     */
    [Fact]
    public void Hold_HoldMarkNotAtEnd()
    {
        Dictionary<string, Func<NoteBase, bool>> markCheckMap = new()
        {
            { "hb", note => note is HoldNote { IsBreak: true, IsEx: false } },
            { "hx", note => note is HoldNote { IsBreak: false, IsEx: true } },
            { "hbx", note => note is HoldNote { IsBreak: true, IsEx: true } },
            { "hxb", note => note is HoldNote { IsBreak: true, IsEx: true } },
            { "bhx", note => note is HoldNote { IsBreak: true, IsEx: true } },
            { "xhb", note => note is HoldNote { IsBreak: true, IsEx: true } }
        };
        Dictionary<string, Func<NoteBase, bool>> durCheckMap = new()
        {
            { DURATION_CHECK_MAP[DurationTypeEnum.Empty].Key, DURATION_CHECK_MAP[DurationTypeEnum.Empty].Value },
            { DURATION_CHECK_MAP[DurationTypeEnum.Fraction].Key, DURATION_CHECK_MAP[DurationTypeEnum.Fraction].Value },
            { DURATION_CHECK_MAP[DurationTypeEnum.Time].Key, DURATION_CHECK_MAP[DurationTypeEnum.Time].Value },
            {
                DURATION_CHECK_MAP[DurationTypeEnum.BpmFraction].Key,
                DURATION_CHECK_MAP[DurationTypeEnum.BpmFraction].Value
            }
        };

        new CheckChain().Iter(BUTTON_CHECK_MAP).Iter(markCheckMap).Iter(durCheckMap)
            .RequireWarning(I18nKeyEnum.HoldMarkNotAtEnd).Check();
    }
    
    /**
     * hold test 4.
     * duplicate marks.
     */
    [Fact]
    public void Hold_DuplicateNoteMarks()
    {
        Dictionary<string, Func<NoteBase, bool>> markCheckMap = new()
        {
            { "hh", note => note is HoldNote { IsBreak: false, IsEx: false } },
            { "bbh", note => note is HoldNote { IsBreak: true, IsEx: false } },
            { "bhb", note => note is HoldNote { IsBreak: true, IsEx: false } },
            { "hbb", note => note is HoldNote { IsBreak: true, IsEx: false } },
            { "xxh", note => note is HoldNote { IsBreak: false, IsEx: true } },
            { "xhx", note => note is HoldNote { IsBreak: false, IsEx: true } },
            { "hxx", note => note is HoldNote { IsBreak: false, IsEx: true } },
            { "xhh", note => note is HoldNote { IsBreak: false, IsEx: true } },
            { "hxh", note => note is HoldNote { IsBreak: false, IsEx: true } },
            { "hhx", note => note is HoldNote { IsBreak: false, IsEx: true } },
            { "xxbh", note => note is HoldNote { IsBreak: true, IsEx: true } },
            { "xxhb", note => note is HoldNote { IsBreak: true, IsEx: true } },
            { "hxxb", note => note is HoldNote { IsBreak: true, IsEx: true } }
        };
        Dictionary<string, Func<NoteBase, bool>> durCheckMap = new()
        {
            { DURATION_CHECK_MAP[DurationTypeEnum.Empty].Key, DURATION_CHECK_MAP[DurationTypeEnum.Empty].Value },
            { DURATION_CHECK_MAP[DurationTypeEnum.Fraction].Key, DURATION_CHECK_MAP[DurationTypeEnum.Fraction].Value },
            { DURATION_CHECK_MAP[DurationTypeEnum.Time].Key, DURATION_CHECK_MAP[DurationTypeEnum.Time].Value },
            {
                DURATION_CHECK_MAP[DurationTypeEnum.BpmFraction].Key,
                DURATION_CHECK_MAP[DurationTypeEnum.BpmFraction].Value
            }
        };

        new CheckChain().Iter(BUTTON_CHECK_MAP).Iter(markCheckMap).Iter(durCheckMap)
            .RequireWarning(I18nKeyEnum.DuplicateNoteMarks).Check();
    }

    /**
     * touch test 1.
     * correct format.
     */
    [Fact]
    public void Touch_CorrectFormat()
    {
        Dictionary<string, Func<NoteBase, bool>> markCheckMap = new()
        {
            { "", note => note is TouchNote { IsFirework: false } },
            { "f", note => note is TouchNote { IsFirework: true } }
        };

        new CheckChain().Iter(AREA_CHECK_MAP).Iter(markCheckMap).Check();
    }

    /**
     * touch test 2.
     * duplicate marks.
     */
    [Fact]
    public void Touch_DuplicateNoteMarks()
    {
        Dictionary<string, Func<NoteBase, bool>> markCheckMap = new()
        {
            { "ff", note => note is TouchNote { IsFirework: true } },
            { "fff", note => note is TouchNote { IsFirework: true } }
        };

        new CheckChain().Iter(AREA_CHECK_MAP).Iter(markCheckMap).RequireWarning(I18nKeyEnum.DuplicateNoteMarks).Check();
    }
    
    /**
     * touch hold test 1.
     * correct format
     */
    [Fact]
    public void TouchHold_CorrectFormat()
    {
        Dictionary<string, Func<NoteBase, bool>> markCheckMap = new()
        {
            { "h", note => note is TouchHoldNote { IsFirework: false } },
            { "fh", note => note is TouchHoldNote { IsFirework: true } },
        };
        Dictionary<string, Func<NoteBase, bool>> durCheckMap = new()
        {
            { DURATION_CHECK_MAP[DurationTypeEnum.Empty].Key, DURATION_CHECK_MAP[DurationTypeEnum.Empty].Value },
            { DURATION_CHECK_MAP[DurationTypeEnum.Fraction].Key, DURATION_CHECK_MAP[DurationTypeEnum.Fraction].Value },
            { DURATION_CHECK_MAP[DurationTypeEnum.Time].Key, DURATION_CHECK_MAP[DurationTypeEnum.Time].Value },
            {
                DURATION_CHECK_MAP[DurationTypeEnum.BpmFraction].Key,
                DURATION_CHECK_MAP[DurationTypeEnum.BpmFraction].Value
            }
        };

        new CheckChain().Iter(AREA_CHECK_MAP).Iter(markCheckMap).Iter(durCheckMap).Check();
    }

    /**
     * touch hold test 2.
     * unsupported duration type. `[120#0.5]`, `[1.5##8:1]` etc.
     */
    [Fact]
    public void TouchHold_UnsupportedDurationType()
    {
        Dictionary<string, Func<NoteBase, bool>> markCheckMap = new()
        {
            { "h", note => note is TouchHoldNote { IsFirework: false } },
            { "fh", note => note is TouchHoldNote { IsFirework: true } },
        };
        Dictionary<string, Func<NoteBase, bool>> durCheckMap = new()
        {
            { DURATION_CHECK_MAP[DurationTypeEnum.BpmTime].Key, DURATION_CHECK_MAP[DurationTypeEnum.BpmTime].Value },
            {
                DURATION_CHECK_MAP[DurationTypeEnum.DelayFraction].Key,
                DURATION_CHECK_MAP[DurationTypeEnum.DelayFraction].Value
            },
            {
                DURATION_CHECK_MAP[DurationTypeEnum.DelayTime].Key, DURATION_CHECK_MAP[DurationTypeEnum.DelayTime].Value
            },
            {
                DURATION_CHECK_MAP[DurationTypeEnum.DelayBpmFraction].Key,
                DURATION_CHECK_MAP[DurationTypeEnum.DelayBpmFraction].Value
            }
        };

        new CheckChain().Iter(AREA_CHECK_MAP).Iter(markCheckMap).Iter(durCheckMap)
            .RequireWarning(I18nKeyEnum.UnsupportedDurationType).Check();
    }

    /**
     * touch hold test 3.
     * hold mark does not at the end of note.
     */
    [Fact]
    public void TouchHold_HoldMarkNotAtEnd()
    {
        Dictionary<string, Func<NoteBase, bool>> markCheckMap = new()
        {
            { "hf", note => note is TouchHoldNote { IsFirework: true } },
        };
        Dictionary<string, Func<NoteBase, bool>> durCheckMap = new()
        {
            { DURATION_CHECK_MAP[DurationTypeEnum.Empty].Key, DURATION_CHECK_MAP[DurationTypeEnum.Empty].Value },
            { DURATION_CHECK_MAP[DurationTypeEnum.Fraction].Key, DURATION_CHECK_MAP[DurationTypeEnum.Fraction].Value },
            { DURATION_CHECK_MAP[DurationTypeEnum.Time].Key, DURATION_CHECK_MAP[DurationTypeEnum.Time].Value },
            {
                DURATION_CHECK_MAP[DurationTypeEnum.BpmFraction].Key,
                DURATION_CHECK_MAP[DurationTypeEnum.BpmFraction].Value
            }
        };

        new CheckChain().Iter(AREA_CHECK_MAP).Iter(markCheckMap).Iter(durCheckMap)
            .RequireWarning(I18nKeyEnum.HoldMarkNotAtEnd).Check();
    }
    
    /**
     * touch hold test 4.
     * duplicate marks.
     */
    [Fact]
    public void TouchHold_DuplicateNoteMarks()
    {
        Dictionary<string, Func<NoteBase, bool>> markCheckMap = new()
        {
            { "hh", note => note is TouchHoldNote { IsFirework: false } },
            { "fhf", note => note is TouchHoldNote { IsFirework: true } },
            { "ffh", note => note is TouchHoldNote { IsFirework: true } },
            { "hff", note => note is TouchHoldNote { IsFirework: true } },
            { "fhh", note => note is TouchHoldNote { IsFirework: true } },
            { "hhf", note => note is TouchHoldNote { IsFirework: true } },
            { "hfh", note => note is TouchHoldNote { IsFirework: true } },
        };
        Dictionary<string, Func<NoteBase, bool>> durCheckMap = new()
        {
            { DURATION_CHECK_MAP[DurationTypeEnum.Empty].Key, DURATION_CHECK_MAP[DurationTypeEnum.Empty].Value },
            { DURATION_CHECK_MAP[DurationTypeEnum.Fraction].Key, DURATION_CHECK_MAP[DurationTypeEnum.Fraction].Value },
            { DURATION_CHECK_MAP[DurationTypeEnum.Time].Key, DURATION_CHECK_MAP[DurationTypeEnum.Time].Value },
            {
                DURATION_CHECK_MAP[DurationTypeEnum.BpmFraction].Key,
                DURATION_CHECK_MAP[DurationTypeEnum.BpmFraction].Value
            }
        };

        new CheckChain().Iter(AREA_CHECK_MAP).Iter(markCheckMap).Iter(durCheckMap).RequireWarning(I18nKeyEnum.DuplicateNoteMarks).Check();
    }

    /**
     * slide test 1.
     * simple format like `1-5[8:1]`
     * TODO: more test case
     */
    [Fact]
    public void Slide_SimpleFormat()
    {
        Dictionary<string, Func<NoteBase, bool>> slideCheckMap = new()
        {
            {
                "1-5[8:1]", note =>
                {
                    if (note is not SlideNote slide)
                    {
                        return false;
                    }

                    return slide is { IsBreakTap: false, IsExTap: false, IsHeadless: false, SlideBodies.Count: 1 } &&
                           slide.SlideBodies[0] is
                           {
                               IsBreakSlide: false, IsChainDuration: false,
                               Duration: { Type: DurationTypeEnum.Fraction, FracDenominator: 8, FracNumerator: 1 },
                               SlideChain.Count: 1
                           } body && body.SlideChain[0] is
                               { Type: SlideTypeEnum.Straight, StopButton: ButtonEnum.Button5 };

                }
            },
            {
                "1b-5[8:1]", note =>
                {
                    if (note is not SlideNote slide)
                    {
                        return false;
                    }

                    return slide is { IsBreakTap: true, IsExTap: false, IsHeadless: false, SlideBodies.Count: 1 } &&
                           slide.SlideBodies[0] is
                           {
                               IsBreakSlide: false, IsChainDuration: false,
                               Duration: { Type: DurationTypeEnum.Fraction, FracDenominator: 8, FracNumerator: 1 },
                               SlideChain.Count: 1
                           } body && body.SlideChain[0] is
                               { Type: SlideTypeEnum.Straight, StopButton: ButtonEnum.Button5 };

                }
            },
            {
                "1x-5[8:1]", note =>
                {
                    if (note is not SlideNote slide)
                    {
                        return false;
                    }

                    return slide is { IsBreakTap: false, IsExTap: true, IsHeadless: false, SlideBodies.Count: 1 } &&
                           slide.SlideBodies[0] is
                           {
                               IsBreakSlide: false, IsChainDuration: false,
                               Duration: { Type: DurationTypeEnum.Fraction, FracDenominator: 8, FracNumerator: 1 },
                               SlideChain.Count: 1
                           } body && body.SlideChain[0] is
                               { Type: SlideTypeEnum.Straight, StopButton: ButtonEnum.Button5 };

                }
            },
            {
                "1bx-5[8:1]", note =>
                {
                    if (note is not SlideNote slide)
                    {
                        return false;
                    }

                    return slide is { IsBreakTap: true, IsExTap: true, IsHeadless: false, SlideBodies.Count: 1 } &&
                           slide.SlideBodies[0] is
                           {
                               IsBreakSlide: false, IsChainDuration: false,
                               Duration: { Type: DurationTypeEnum.Fraction, FracDenominator: 8, FracNumerator: 1 },
                               SlideChain.Count: 1
                           } body && body.SlideChain[0] is
                               { Type: SlideTypeEnum.Straight, StopButton: ButtonEnum.Button5 };

                }
            },
            {
                "1b-5b[8:1]", note =>
                {
                    if (note is not SlideNote slide)
                    {
                        return false;
                    }

                    return slide is { IsBreakTap: true, IsExTap: false, IsHeadless: false, SlideBodies.Count: 1 } &&
                           slide.SlideBodies[0] is
                           {
                               IsBreakSlide: true, IsChainDuration: false,
                               Duration: { Type: DurationTypeEnum.Fraction, FracDenominator: 8, FracNumerator: 1 },
                               SlideChain.Count: 1
                           } body && body.SlideChain[0] is
                               { Type: SlideTypeEnum.Straight, StopButton: ButtonEnum.Button5 };

                }
            },
            {
                "1b-5[8:1]b", note =>
                {
                    if (note is not SlideNote slide)
                    {
                        return false;
                    }

                    return slide is { IsBreakTap: true, IsExTap: false, IsHeadless: false, SlideBodies.Count: 1 } &&
                           slide.SlideBodies[0] is
                           {
                               IsBreakSlide: true, IsChainDuration: false,
                               Duration: { Type: DurationTypeEnum.Fraction, FracDenominator: 8, FracNumerator: 1 },
                               SlideChain.Count: 1
                           } body && body.SlideChain[0] is
                               { Type: SlideTypeEnum.Straight, StopButton: ButtonEnum.Button5 };

                }
            },
            {
                "1>5[8:1]", note =>
                {
                    if (note is not SlideNote slide)
                    {
                        return false;
                    }

                    return slide is { IsBreakTap: false, IsExTap: false, IsHeadless: false, SlideBodies.Count: 1 } &&
                           slide.SlideBodies[0] is
                           {
                               IsBreakSlide: false, IsChainDuration: false,
                               Duration: { Type: DurationTypeEnum.Fraction, FracDenominator: 8, FracNumerator: 1 },
                               SlideChain.Count: 1
                           } body && body.SlideChain[0] is
                               { Type: SlideTypeEnum.CircleRight, StopButton: ButtonEnum.Button5 };

                }
            },
            {
                "1b<5[8:1]", note =>
                {
                    if (note is not SlideNote slide)
                    {
                        return false;
                    }

                    return slide is { IsBreakTap: true, IsExTap: false, IsHeadless: false, SlideBodies.Count: 1 } &&
                           slide.SlideBodies[0] is
                           {
                               IsBreakSlide: false, IsChainDuration: false,
                               Duration: { Type: DurationTypeEnum.Fraction, FracDenominator: 8, FracNumerator: 1 },
                               SlideChain.Count: 1
                           } body && body.SlideChain[0] is
                               { Type: SlideTypeEnum.CircleLeft, StopButton: ButtonEnum.Button5 };

                }
            },
            {
                "1x^4[8:1]", note =>
                {
                    if (note is not SlideNote slide)
                    {
                        return false;
                    }

                    return slide is { IsBreakTap: false, IsExTap: true, IsHeadless: false, SlideBodies.Count: 1 } &&
                           slide.SlideBodies[0] is
                           {
                               IsBreakSlide: false, IsChainDuration: false,
                               Duration: { Type: DurationTypeEnum.Fraction, FracDenominator: 8, FracNumerator: 1 },
                               SlideChain.Count: 1
                           } body && body.SlideChain[0] is
                               { Type: SlideTypeEnum.CircleNear, StopButton: ButtonEnum.Button4 };

                }
            },
            {
                "1bxv4[8:1]", note =>
                {
                    if (note is not SlideNote slide)
                    {
                        return false;
                    }

                    return slide is { IsBreakTap: true, IsExTap: true, IsHeadless: false, SlideBodies.Count: 1 } &&
                           slide.SlideBodies[0] is
                           {
                               IsBreakSlide: false, IsChainDuration: false,
                               Duration: { Type: DurationTypeEnum.Fraction, FracDenominator: 8, FracNumerator: 1 },
                               SlideChain.Count: 1
                           } body && body.SlideChain[0] is
                               { Type: SlideTypeEnum.Fold, StopButton: ButtonEnum.Button4 };

                }
            },
            {
                "1bs5b[8:1]", note =>
                {
                    if (note is not SlideNote slide)
                    {
                        return false;
                    }

                    return slide is { IsBreakTap: true, IsExTap: false, IsHeadless: false, SlideBodies.Count: 1 } &&
                           slide.SlideBodies[0] is
                           {
                               IsBreakSlide: true, IsChainDuration: false,
                               Duration: { Type: DurationTypeEnum.Fraction, FracDenominator: 8, FracNumerator: 1 },
                               SlideChain.Count: 1
                           } body && body.SlideChain[0] is
                               { Type: SlideTypeEnum.Thunder, StopButton: ButtonEnum.Button5 };

                }
            },
            {
                "1bz5[8:1]b", note =>
                {
                    if (note is not SlideNote slide)
                    {
                        return false;
                    }

                    return slide is { IsBreakTap: true, IsExTap: false, IsHeadless: false, SlideBodies.Count: 1 } &&
                           slide.SlideBodies[0] is
                           {
                               IsBreakSlide: true, IsChainDuration: false,
                               Duration: { Type: DurationTypeEnum.Fraction, FracDenominator: 8, FracNumerator: 1 },
                               SlideChain.Count: 1
                           } body && body.SlideChain[0] is
                               { Type: SlideTypeEnum.ThunderMirror, StopButton: ButtonEnum.Button5 };

                }
            },
            {
                "1w5[8:1]", note =>
                {
                    if (note is not SlideNote slide)
                    {
                        return false;
                    }

                    return slide is { IsBreakTap: false, IsExTap: false, IsHeadless: false, SlideBodies.Count: 1 } &&
                           slide.SlideBodies[0] is
                           {
                               IsBreakSlide: false, IsChainDuration: false,
                               Duration: { Type: DurationTypeEnum.Fraction, FracDenominator: 8, FracNumerator: 1 },
                               SlideChain.Count: 1
                           } body && body.SlideChain[0] is
                               { Type: SlideTypeEnum.Fan, StopButton: ButtonEnum.Button5 };

                }
            },
            {
                "1bp5[8:1]", note =>
                {
                    if (note is not SlideNote slide)
                    {
                        return false;
                    }

                    return slide is { IsBreakTap: true, IsExTap: false, IsHeadless: false, SlideBodies.Count: 1 } &&
                           slide.SlideBodies[0] is
                           {
                               IsBreakSlide: false, IsChainDuration: false,
                               Duration: { Type: DurationTypeEnum.Fraction, FracDenominator: 8, FracNumerator: 1 },
                               SlideChain.Count: 1
                           } body && body.SlideChain[0] is
                               { Type: SlideTypeEnum.CurveCcw, StopButton: ButtonEnum.Button5 };

                }
            },
            {
                "1xq5[8:1]", note =>
                {
                    if (note is not SlideNote slide)
                    {
                        return false;
                    }

                    return slide is { IsBreakTap: false, IsExTap: true, IsHeadless: false, SlideBodies.Count: 1 } &&
                           slide.SlideBodies[0] is
                           {
                               IsBreakSlide: false, IsChainDuration: false,
                               Duration: { Type: DurationTypeEnum.Fraction, FracDenominator: 8, FracNumerator: 1 },
                               SlideChain.Count: 1
                           } body && body.SlideChain[0] is
                               { Type: SlideTypeEnum.CurveCw, StopButton: ButtonEnum.Button5 };

                }
            },
            {
                "1bxpp5[8:1]", note =>
                {
                    if (note is not SlideNote slide)
                    {
                        return false;
                    }

                    return slide is { IsBreakTap: true, IsExTap: true, IsHeadless: false, SlideBodies.Count: 1 } &&
                           slide.SlideBodies[0] is
                           {
                               IsBreakSlide: false, IsChainDuration: false,
                               Duration: { Type: DurationTypeEnum.Fraction, FracDenominator: 8, FracNumerator: 1 },
                               SlideChain.Count: 1
                           } body && body.SlideChain[0] is
                               { Type: SlideTypeEnum.TweakCurveCcw, StopButton: ButtonEnum.Button5 };

                }
            },
            {
                "1bqq5b[8:1]", note =>
                {
                    if (note is not SlideNote slide)
                    {
                        return false;
                    }

                    return slide is { IsBreakTap: true, IsExTap: false, IsHeadless: false, SlideBodies.Count: 1 } &&
                           slide.SlideBodies[0] is
                           {
                               IsBreakSlide: true, IsChainDuration: false,
                               Duration: { Type: DurationTypeEnum.Fraction, FracDenominator: 8, FracNumerator: 1 },
                               SlideChain.Count: 1
                           } body && body.SlideChain[0] is
                               { Type: SlideTypeEnum.TweakCurveCw, StopButton: ButtonEnum.Button5 };

                }
            },
            {
                "1bV35[8:1]b", note =>
                {
                    if (note is not SlideNote slide)
                    {
                        return false;
                    }

                    return slide is { IsBreakTap: true, IsExTap: false, IsHeadless: false, SlideBodies.Count: 1 } &&
                           slide.SlideBodies[0] is
                           {
                               IsBreakSlide: true, IsChainDuration: false,
                               Duration: { Type: DurationTypeEnum.Fraction, FracDenominator: 8, FracNumerator: 1 },
                               SlideChain.Count: 1
                           } body && body.SlideChain[0] is
                           {
                               Type: SlideTypeEnum.Turn, TurnButton: ButtonEnum.Button3, StopButton: ButtonEnum.Button5
                           };

                }
            },
        };
        
        new CheckChain().Iter(slideCheckMap).Check();
    }

    private class CheckChain
    {
        private readonly List<Dictionary<string, Func<NoteBase, bool>>> _checkList = new();
        private readonly HashSet<I18nKeyEnum> _errKeys = new();
        private readonly HashSet<I18nKeyEnum> _warnKeys = new();

        public CheckChain Iter(Dictionary<string, Func<NoteBase, bool>> checkMap)
        {
            _checkList.Add(checkMap);
            return this;
        }

        public CheckChain RequireWarning(I18nKeyEnum warnKey)
        {
            _warnKeys.Add(warnKey);
            return this;
        }

        public CheckChain RequireError(I18nKeyEnum errKey)
        {
            _errKeys.Add(errKey);
            return this;
        }

        public void Check()
        {
            SubCheck(0, "", new List<Func<NoteBase, bool>>());
        }

        private void SubCheck(int layer, string noteString, List<Func<NoteBase, bool>> checkFunc)
        {
            if (layer == _checkList.Count)
            {
                // last layer. Check it out, now! *scratch*
                noteString += Constants.NOTE_GROUP_SEPARATOR;

                var result = NoteBlockWalker.GenerateFromText(noteString);

                // check warnings and errors
                if (_warnKeys.Count == 0)
                {
                    Assert.Empty(result.WarningList);
                }
                else
                {
                    Assert.Contains(result.WarningList, warn => _warnKeys.Contains(warn.Key));
                }

                if (_errKeys.Count == 0)
                {
                    Assert.Empty(result.ErrorList);
                }
                else
                {
                    Assert.Contains(result.ErrorList, err => _errKeys.Contains(err.Key));
                }

                // check note
                Assert.Single(result.NoteGroupList);
                var noteGroup = result.NoteGroupList[0];
                Assert.Single(noteGroup.NoteList);
                Assert.Single(noteGroup.NoteList[0]);

                var note = noteGroup.NoteList[0][0];
                Assert.NotNull(note);
                checkFunc.ForEach(func => Assert.True(func(note)));
            }
            else
            {
                // iter layer. recur
                foreach (var each in _checkList[layer])
                {
                    var newCheckFunc = new List<Func<NoteBase, bool>>(checkFunc) { each.Value };
                    SubCheck(layer + 1, noteString + each.Key, newCheckFunc);
                }
            }
        }
    }
}
