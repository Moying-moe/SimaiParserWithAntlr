using Antlr4.Runtime;
using SimaiParserWithAntlr.NoteLayerParser;

namespace SimaiParserWithAntlr.DataModels;

public class TextPositionRange
{
    public static readonly TextPositionRange EMPTY = new TextPositionRange(TextPosition.EMPTY, TextPosition.EMPTY);

    public TextPosition Start { get; set; }
    public TextPosition Stop { get; set; }

    public TextPositionRange(TextPosition start, TextPosition stop)
    {
        Start = start;
        Stop = stop;
    }

    public TextPositionRange(IToken startToken, IToken stopToken)
    {
        Start = new TextPosition(startToken);
        Stop = new TextPosition(stopToken.Line, stopToken.Column + stopToken.Text.Length);
    }

    public TextPositionRange(TextPosition start, TextPosition stop, TextPosition offset) : this(start, stop)
    {
        ApplyOffset(offset);
    }

    public TextPositionRange(IToken startToken, IToken stopToken, TextPosition offset) : this(startToken, stopToken)
    {
        ApplyOffset(offset);
    }

    public TextPositionRange(ParserRuleContext context) : this(context.Start, context.Stop)
    {
    }

    public TextPositionRange(ParserRuleContext context, TextPosition offset) : this(context)
    {
        ApplyOffset(offset);
    }

    public void ApplyOffset(TextPosition offset)
    {
        Start = offset + Start;
        Stop = offset + Stop;
    }

    public override string ToString()
    {
        return $"TextPositionRange<start={Start}, stop={Stop}>";
    }
}
