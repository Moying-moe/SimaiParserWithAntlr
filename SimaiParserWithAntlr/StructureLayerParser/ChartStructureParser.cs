using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using SimaiParserWithAntlr.DataModels;
using SimaiParserWithAntlr.I18nModule;
using SimaiParserWithAntlr.StructureLayerParser.Structures;

namespace SimaiParserWithAntlr.StructureLayerParser;

public class ChartStructureParser : StructureParserBaseListener
{
    public string RawText { get; private set; }
    public TextPosition Offset { get; private set; }

    public List<ElementBase> ElementList { get; } = new();
    // TODO: MAML support
    public List<WarningInfo> WarningList { get; } = new();
    public List<ErrorInfo> ErrorList { get; } = new();

    public ChartStructureParser(string rawText, TextPosition offset)
    {
        RawText = rawText;
        Offset = offset;
    }

    public ChartStructureParser(string rawText) : this(rawText, TextPosition.EMPTY)
    {
    }

    public override void EnterElement(StructureParser.ElementContext context)
    {
        TextPositionRange elRange = new(context, Offset);

        if (context.bpm() is { } bpmCtx)
        {
            if (ParseBpm(bpmCtx) is { } bpm)
            {
                ElementList.Add(bpm);
            }
        }
        else if (context.resolution() is { } resolutionCtx)
        {
            if (ParseResolution(resolutionCtx) is { } resolution)
            {
                ElementList.Add(resolution);
            }
        }
        else if (context.h_speed() is { } hiSpeedCtx)
        {
            if (ParseHiSpeed(hiSpeedCtx) is { } hiSpeed)
            {
                ElementList.Add(hiSpeed);
            }
        }
        else if (context.note_block() is { } noteBlockCtx)
        {
            if (ParseNoteBlock(noteBlockCtx) is { } noteBlock)
            {
                ElementList.Add(noteBlock);
            }
        }
        else if (context.comment() is { } commentCtx)
        {
            if (ParseComment(commentCtx) is { } comment)
            {
                ElementList.Add(comment);
            }
        }
        else
        {
            // unknown structure block
            ThrowError(elRange, I18nKeyEnum.UnknownStructureBlock, context.GetText());
        }

        base.EnterElement(context);
    }

    private BpmElement? ParseBpm(StructureParser.BpmContext context)
    {
        TextPositionRange range = new(context, Offset);

        if (!double.TryParse(context.value.Text, out var bpm))
        {
            ThrowError(range, I18nKeyEnum.FailToParseNumber, context.value.Text, "double");
            return null;
        }

        if (bpm <= 1e-5)
        {
            ThrowError(range, I18nKeyEnum.InvalidBpm, context.value.Text);
            return null;
        }

        return new BpmElement(context.GetText(), range, bpm);
    }
    
    private ResolutionElement? ParseResolution(StructureParser.ResolutionContext context)
    {
        TextPositionRange range = new(context, Offset);

        if (!int.TryParse(context.value.Text, out var resolution))
        {
            ThrowError(range, I18nKeyEnum.FailToParseNumber, context.value.Text, "int");
            return null;
        }

        if (resolution <= 0)
        {
            ThrowError(range, I18nKeyEnum.InvalidResolution, context.value.Text);
            return null;
        }

        return new ResolutionElement(context.GetText(), range, resolution);
    }
    
    private HiSpeedElement? ParseHiSpeed(StructureParser.H_speedContext context)
    {
        TextPositionRange range = new(context, Offset);

        if (!double.TryParse(context.rate.Text, out var hiSpeed))
        {
            ThrowError(range, I18nKeyEnum.FailToParseNumber, context.rate.Text, "double");
            return null;
        }

        if (hiSpeed <= 1e-5)
        {
            ThrowError(range, I18nKeyEnum.InvalidHiSpeed, context.rate.Text);
            return null;
        }

        return new HiSpeedElement(context.GetText(), range, hiSpeed);
    }
    
    private NoteBlockElement? ParseNoteBlock(StructureParser.Note_blockContext context)
    {
        TextPositionRange range = new(context, Offset);

        return new NoteBlockElement(context.GetText(), range);
    }
    
    private CommentElement? ParseComment(StructureParser.CommentContext context)
    {
        TextPositionRange range = new(context, Offset);

        string rawText = context.GetText();
        string content = rawText;
        if (content.StartsWith(Constants.COMMENT_SYMBOL))
        {
            content = content[Constants.COMMENT_SYMBOL.Length..];
        }

        if (!content.StartsWith(" "))
        {
            ThrowWarning(range, I18nKeyEnum.NoSpaceAfterCommentSymbol);
        }

        if (content.Length == 0)
        {
            ThrowWarning(range, I18nKeyEnum.EmptySymbol);
        }

        return new CommentElement(rawText, range, content);
    }

    private void ThrowWarning(TextPositionRange range, I18nKeyEnum key, params object[] args)
    {
        WarningList.Add(new WarningInfo(range, key, args));
    }

    private void ThrowError(TextPositionRange range, I18nKeyEnum key, params object[] args)
    {
        ErrorList.Add(new ErrorInfo(range, key, args));
    }

    public static ChartStructureParser GenerateFromText(string text, TextPosition? offset = null)
    {
        // Constructing the syntax tree
        AntlrInputStream charStream = new(text);
        ITokenSource lexer = new StructureLexer(charStream);
        ITokenStream tokens = new CommonTokenStream(lexer);
        StructureParser parser = new(tokens)
        {
            BuildParseTree = true
        };

        IParseTree tree = parser.note_block();

        // Visiting using the listener
        var walker = (offset == null) 
            ? new ChartStructureParser(text) 
            : new ChartStructureParser(text, offset);
        ParseTreeWalker.Default.Walk(walker, tree);

        return walker;
    }
}
