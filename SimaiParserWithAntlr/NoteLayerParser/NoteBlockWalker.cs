using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using SimaiParserWithAntlr.DataModels;
using SimaiParserWithAntlr.I18nModule;
using SimaiParserWithAntlr.NoteLayerParser.DataModels;
using SimaiParserWithAntlr.NoteLayerParser.Enums;
using SimaiParserWithAntlr.NoteLayerParser.Notes;

namespace SimaiParserWithAntlr.NoteLayerParser;

public class NoteBlockWalker : NoteBlockParserBaseListener
{
    public TextPosition Offset { get; private set; }

    public List<NoteGroup> NoteGroupList { get; } = new();
    public List<WarningInfo> WarningList { get; } = new();
    public List<ErrorInfo> ErrorList { get; } = new();

    public NoteBlockWalker(TextPosition offset)
    {
        Offset = offset;
    }

    public NoteBlockWalker() : this(TextPosition.EMPTY)
    {
    }

    /**
     * Process note groups.
     */
    public override void EnterNote_group(NoteBlockParser.Note_groupContext context)
    {
        TextPositionRange groupRange = new(context, Offset);
        NoteGroup noteGroup = new(groupRange);

        if (context.each_tap() is { } eachTap)
        {
            // each tap group like `15`
            TextPositionRange range1 = new(eachTap.pos1, eachTap.pos1, Offset);
            TextPositionRange range2 = new(eachTap.pos2, eachTap.pos2, Offset);

            if (ButtonEnumExt.TryParse(eachTap.pos1.Text, out var btn1) && ButtonEnumExt.TryParse(eachTap.pos2.Text, out var btn2))
            {
                noteGroup.AddEach(NoteGroup.BuildEach()
                    .Add(new TapNote(range1, btn1))
                    .Add(new TapNote(range2, btn2))
                    .Build());
            }
            else
            {
                ThrowError(groupRange, I18nKeyEnum.FailToParseButton, context.GetText());
            }
        }
        else if (context.each_group() is { } eachGroup)
        {
            // normal each group
            foreach (var group in eachGroup)
            {
                noteGroup.AddEach(ParseEachGroup(group));
            }
        }

        NoteGroupList.Add(noteGroup);

        base.EnterNote_group(context);
    }

    /**
     * Process each groups.
     */
    private List<NoteBase> ParseEachGroup(NoteBlockParser.Each_groupContext context)
    {
        List<NoteBase> result = new();

        if (context.note() is { } notes)
        {
            foreach (var noteCtx in notes)
            {
                TextPositionRange range = new(noteCtx, Offset);

                // parse note by note type
                if (noteCtx.tap() is { } tapCtx)
                {
                    if (ParseTap(tapCtx) is { } tap)
                    {
                        result.Add(tap);
                    }
                }
                else if (noteCtx.hold() is { } holdCtx)
                {
                    if (ParseHold(holdCtx) is { } hold)
                    {
                        result.Add(hold);
                    }
                }
                else if (noteCtx.touch() is { } touchCtx)
                {
                    if (ParseTouch(touchCtx) is { } touch)
                    {
                        result.Add(touch);
                    }
                }
                else if (noteCtx.touch_hold() is { } touchHoldCtx)
                {
                    if (ParseTouchHold(touchHoldCtx) is { } touchHold)
                    {
                        result.Add(touchHold);
                    }
                }
                else if (noteCtx.slide() is { } slideCtx)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    ThrowError(range, I18nKeyEnum.NotAState, "every sub-rule in `note` is null");
                }
            }
        }

        return result;
    }

    /**
     * Process taps. If an error occurs that prevents parsing, returns null.
     */
    private TapNote? ParseTap(NoteBlockParser.TapContext context)
    {
        TextPositionRange range = new(context, Offset);

        bool isBreak = false;
        bool isEx = false;

        if (!ButtonEnumExt.TryParse(context.pos.Text, out var button))
        {
            ThrowError(range, I18nKeyEnum.FailToParseButton, context.pos.Text);
            return null;
        }

        var tapMarks = context.tap_mark();
        // break
        if (tapMarks.BREAK_MARK() is { } breakMarks)
        {
            if (breakMarks.Length != 0)
            {
                isBreak = true;

                // A warning is thrown when breakMarks contains more than one member.
                if (breakMarks.Length > 1)
                {
                    ThrowWarning(range, I18nKeyEnum.DuplicateNoteMarks, "break");
                }
            }
        }

        // ex
        if (tapMarks.EX_MARK() is { } exMarks)
        {
            if (exMarks.Length != 0)
            {
                isEx = true;

                if (exMarks.Length > 1)
                {
                    ThrowWarning(range, I18nKeyEnum.DuplicateNoteMarks, "ex");
                }
            }
        }

        return new TapNote(range, button, isBreak, isEx);
    }
    
    /**
     * Process holds. If an error occurs that prevents parsing, returns null.
     */
    private HoldNote? ParseHold(NoteBlockParser.HoldContext context)
    {
        TextPositionRange range = new(context, Offset);

        bool isBreak = false;
        bool isEx = false;

        if (!ButtonEnumExt.TryParse(context.pos.Text, out var button))
        {
            ThrowError(range, I18nKeyEnum.FailToParseButton, context.pos.Text);
            return null;
        }

        var holdMark = context.hold_mark();
        // break
        if (holdMark.BREAK_MARK() is { } breakMarks)
        {
            if (breakMarks.Length != 0)
            {
                isBreak = true;

                // A warning is thrown when breakMarks contains more than one member.
                if (breakMarks.Length > 1)
                {
                    ThrowWarning(range, I18nKeyEnum.DuplicateNoteMarks, "break");
                }
            }
        }

        // ex
        if (holdMark.EX_MARK() is { } exMarks)
        {
            if (exMarks.Length != 0)
            {
                isEx = true;

                if (exMarks.Length > 1)
                {
                    ThrowWarning(range, I18nKeyEnum.DuplicateNoteMarks, "ex");
                }
            }
        }

        if (holdMark.HOLD_MARK() is { Length: > 1 })
        {
            ThrowWarning(range, I18nKeyEnum.DuplicateNoteMarks, "hold");
        }
        
        // If the hold mark doesn't appear at the last position, we throw a warning.
        if (holdMark.Stop.Type != NoteBlockParser.HOLD_MARK)
        {
            ThrowWarning(range, I18nKeyEnum.HoldMarkNotAtEnd);
        }

        var duration = ParseDuration(context.duration());
        if (duration.Type is not (DurationTypeEnum.Empty or DurationTypeEnum.Fraction or DurationTypeEnum.Time
            or DurationTypeEnum.BpmFraction))
        {
            ThrowWarning(range, I18nKeyEnum.UnsupportedDurationType, "hold");
        }

        return new HoldNote(range, button, isBreak, isEx, duration);
    }
    
    /**
     * Process touch. If an error occurs that prevents parsing, returns null.
     */
    private TouchNote? ParseTouch(NoteBlockParser.TouchContext context)
    {
        TextPositionRange range = new(context, Offset);

        bool isFirework = false;

        if (!AreaEnumExt.TryParse(context.pos.Text, out var area))
        {
            ThrowError(range, I18nKeyEnum.FailToParseArea, context.pos.Text);
            return null;
        }

        var touchMarks = context.touch_mark();

        if (touchMarks.FIREWORK_MARK() is { } fireworkMarks)
        {
            if (fireworkMarks.Length != 0)
            {
                isFirework = true;

                if (fireworkMarks.Length > 1)
                {
                    ThrowWarning(range, I18nKeyEnum.DuplicateNoteMarks, "firework");
                }
            }
        }

        return new TouchNote(range, area, isFirework);
    }
    
    /**
     * Process touch hold. If an error occurs that prevents parsing, returns null.
     */
    private TouchHoldNote? ParseTouchHold(NoteBlockParser.Touch_holdContext context)
    {
        TextPositionRange range = new(context, Offset);

        bool isFirework = false;

        if (!AreaEnumExt.TryParse(context.pos.Text, out var area))
        {
            ThrowError(range, I18nKeyEnum.FailToParseArea, context.pos.Text);
            return null;
        }

        var touchHoldMarks = context.touch_hold_mark();

        if (touchHoldMarks.FIREWORK_MARK() is { } fireworkMarks)
        {
            if (fireworkMarks.Length != 0)
            {
                isFirework = true;

                if (fireworkMarks.Length > 1)
                {
                    ThrowWarning(range, I18nKeyEnum.DuplicateNoteMarks, "firework");
                }
            }
        }
        
        if (touchHoldMarks.HOLD_MARK() is { Length: > 1 })
        {
            ThrowWarning(range, I18nKeyEnum.DuplicateNoteMarks, "hold");
        }

        // If the hold mark doesn't appear at the last position, we throw a warning.
        if (touchHoldMarks.Stop.Type != NoteBlockParser.HOLD_MARK)
        {
            ThrowWarning(range, I18nKeyEnum.HoldMarkNotAtEnd);
        }

        var duration = ParseDuration(context.duration());
        if (duration.Type is not (DurationTypeEnum.Empty or DurationTypeEnum.Fraction or DurationTypeEnum.Time
            or DurationTypeEnum.BpmFraction))
        {
            ThrowWarning(range, I18nKeyEnum.UnsupportedDurationType, "hold");
        }
        
        return new TouchHoldNote(range, area, isFirework, duration);
    }

    /**
     * Process duration.
     * When errors occur during processing, partial generation is attempted based on successfully parsed data as much as possible.
     * When parsing is not possible, returns an empty duration.
     */
    private NoteDuration ParseDuration(NoteBlockParser.DurationContext? context)
    {
        if (context == null)
        {
            return NoteDuration.Empty();
        }
        
        TextPositionRange range = new(context, Offset);
        if (context.frac_duration() is { } fracCtx)
        {
            if (!int.TryParse(fracCtx.den.Text, out var denominator))
            {
                ThrowError(range, I18nKeyEnum.FailToParseNumber, fracCtx.den.Text, "int");
                return NoteDuration.Empty();
            }

            if (!int.TryParse(fracCtx.num.Text, out var numerator))
            {
                ThrowError(range, I18nKeyEnum.FailToParseNumber, fracCtx.num.Text, "int");
                return NoteDuration.Empty();
            }

            return NoteDuration.FromFraction(denominator, numerator);
        }

        if (context.bpm_frac_duration() is { } bpmFracCtx)
        {
            if (!int.TryParse(bpmFracCtx.den.Text, out var denominator))
            {
                ThrowError(range, I18nKeyEnum.FailToParseNumber, bpmFracCtx.den.Text, "int");
                return NoteDuration.Empty();
            }

            if (!int.TryParse(bpmFracCtx.num.Text, out var numerator))
            {
                ThrowError(range, I18nKeyEnum.FailToParseNumber, bpmFracCtx.num.Text, "int");
                return NoteDuration.Empty();
            }
            if (!float.TryParse(bpmFracCtx.bpm.Text, out var bpm))
            {
                ThrowError(range, I18nKeyEnum.FailToParseNumber, bpmFracCtx.bpm.Text, "float");
                return NoteDuration.FromFraction(denominator, numerator);
            }

            return NoteDuration.FromBpmFraction(bpm, denominator, numerator);
        }
        
        if (context.time_duration() is { } timeCtx)
        {
            if (!float.TryParse(timeCtx.dur.Text, out var time))
            {
                ThrowError(range, I18nKeyEnum.FailToParseNumber, timeCtx.dur.Text, "float");
                return NoteDuration.Empty();
            }

            return NoteDuration.FromTime(time);
        }
        
        if (context.bpm_time_duration() is { } bpmTimeCtx)
        {
            if (!float.TryParse(bpmTimeCtx.dur.Text, out var time))
            {
                ThrowError(range, I18nKeyEnum.FailToParseNumber, bpmTimeCtx.dur.Text, "float");
                return NoteDuration.Empty();
            }

            if (!float.TryParse(bpmTimeCtx.bpm.Text, out var bpm))
            {
                ThrowError(range, I18nKeyEnum.FailToParseNumber, bpmTimeCtx.bpm.Text, "float");
                return NoteDuration.FromTime(time);
            }

            return NoteDuration.FromBpmTime(bpm, time);
        }
        
        if (context.delay_frac_duration() is { } delayFracCtx)
        {
            if (!int.TryParse(delayFracCtx.den.Text, out var denominator))
            {
                ThrowError(range, I18nKeyEnum.FailToParseNumber, delayFracCtx.den.Text, "int");
                return NoteDuration.Empty();
            }

            if (!int.TryParse(delayFracCtx.num.Text, out var numerator))
            {
                ThrowError(range, I18nKeyEnum.FailToParseNumber, delayFracCtx.num.Text, "int");
                return NoteDuration.Empty();
            }

            if (!float.TryParse(delayFracCtx.delay.Text, out var delay))
            {
                ThrowError(range, I18nKeyEnum.FailToParseNumber, delayFracCtx.delay.Text, "float");
                return NoteDuration.FromFraction(denominator, numerator);
            }

            return NoteDuration.FromDelayFraction(delay, denominator, numerator);
        }
        
        if (context.delay_time_duration() is { } delayTimeCtx)
        {
            if (!float.TryParse(delayTimeCtx.dur.Text, out var time))
            {
                ThrowError(range, I18nKeyEnum.FailToParseNumber, delayTimeCtx.dur.Text, "int");
                return NoteDuration.Empty();
            }

            if (!float.TryParse(delayTimeCtx.delay.Text, out var delay))
            {
                ThrowError(range, I18nKeyEnum.FailToParseNumber, delayTimeCtx.delay.Text, "float");
                return NoteDuration.FromTime(time);
            }

            return NoteDuration.FromDelayTime(delay, time);
        }
        
        if (context.delay_bpm_frac_duration() is { } delayBpmFracCtx)
        {
            if (!int.TryParse(delayBpmFracCtx.den.Text, out var denominator))
            {
                ThrowError(range, I18nKeyEnum.FailToParseNumber, delayBpmFracCtx.den.Text, "int");
                return NoteDuration.Empty();
            }

            if (!int.TryParse(delayBpmFracCtx.num.Text, out var numerator))
            {
                ThrowError(range, I18nKeyEnum.FailToParseNumber, delayBpmFracCtx.num.Text, "int");
                return NoteDuration.Empty();
            }

            if (!float.TryParse(delayBpmFracCtx.bpm.Text, out var bpm))
            {
                ThrowError(range, I18nKeyEnum.FailToParseNumber, delayBpmFracCtx.bpm.Text, "float");
                return NoteDuration.FromFraction(denominator, numerator);
            }

            if (!float.TryParse(delayBpmFracCtx.delay.Text, out var delay))
            {
                ThrowError(range, I18nKeyEnum.FailToParseNumber, delayBpmFracCtx.delay.Text, "float");
                return NoteDuration.FromBpmFraction(bpm, denominator, numerator);
            }

            return NoteDuration.FromDelayBpmFraction(delay, bpm, denominator, numerator);
        }

        return NoteDuration.Empty();
    }

    private void ThrowWarning(TextPositionRange range, I18nKeyEnum key, params object[] args)
    {
        WarningList.Add(new WarningInfo(range, key, args));
    }

    private void ThrowError(TextPositionRange range, I18nKeyEnum key, params object[] args)
    {
        ErrorList.Add(new ErrorInfo(range, key, args));
    }

    // TODO: Configurable error message level.

    public static NoteBlockWalker GenerateFromText(string text)
    {
        // Constructing the syntax tree
        AntlrInputStream charStream = new(text);
        ITokenSource lexer = new NoteBlockLexer(charStream);
        ITokenStream tokens = new CommonTokenStream(lexer);
        NoteBlockParser parser = new(tokens)
        {
            BuildParseTree = true
        };

        IParseTree tree = parser.note_block();

        // Visiting using the listener
        NoteBlockWalker walker = new();
        ParseTreeWalker.Default.Walk(walker, tree);

        return walker;
    }
}
