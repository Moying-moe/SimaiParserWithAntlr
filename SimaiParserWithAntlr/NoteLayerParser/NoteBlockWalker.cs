using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using SimaiParserWithAntlr.DataModels;
using SimaiParserWithAntlr.I18nModule;
using SimaiParserWithAntlr.NoteLayerParser.Notes;

namespace SimaiParserWithAntlr.NoteLayerParser;

internal class NoteBlockWalker : NoteBlockParserBaseListener
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

    public override void EnterNote_group(NoteBlockParser.Note_groupContext context)
    {
        TextPositionRange groupRange = new(context, Offset);
        NoteGroup noteGroup = new(groupRange);

        if (context.each_tap() is { } eachTap)
        {
            // each tap group like `15`
            TextPositionRange range1 = new(eachTap.pos1, eachTap.pos1, Offset);
            TextPositionRange range2 = new(eachTap.pos2, eachTap.pos2, Offset);

            if (int.TryParse(eachTap.pos1.Text, out var btn1) && int.TryParse(eachTap.pos2.Text, out var btn2))
            {
                noteGroup.AddEach(NoteGroup.BuildEach()
                    .Add(new TapNote(range1, btn1))
                    .Add(new TapNote(range2, btn2))
                    .Build());
            }
            else
            {
                ThrowError(groupRange, I18nKeyEnum.FailToParseNumber, context.GetText(), "int");
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
                    throw new NotImplementedException();
                }
                else if (noteCtx.touch() is { } touchCtx)
                {
                    throw new NotImplementedException();
                }
                else if (noteCtx.touch_hold() is { } touchHoldCtx)
                {
                    throw new NotImplementedException();
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

    private TapNote? ParseTap(NoteBlockParser.TapContext context)
    {
        TextPositionRange range = new(context, Offset);

        int button;
        bool isBreak = false;
        bool isEx = false;

        if (!int.TryParse(context.pos.Text, out button))
        {
            ThrowError(range, I18nKeyEnum.FailToParseNumber, context.pos.Text, "int");
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
