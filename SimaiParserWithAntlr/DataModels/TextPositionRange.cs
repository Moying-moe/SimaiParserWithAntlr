using Antlr4.Runtime;

namespace SimaiParserWithAntlr.DataModels;

public class TextPositionRange
{
    public static readonly TextPositionRange EMTPY = new TextPositionRange(TextPosition.EMPTY, TextPosition.EMPTY);
    
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

    public override string ToString()
    {
        return $"TextPositionRange<start={Start}, stop={Stop}>";
    }
}
