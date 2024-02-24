namespace SimaiParserWithAntlr.DataModels;

public class TextPosition
{
    public TextPosition(int line, int column)
    {
        Line = line;
        Column = column;
    }

    public TextPosition()
    {
        Line = 0;
        Column = 0;
    }

    public int Line { get; }
    public int Column { get; }

    public override string ToString()
    {
        return $"TextPosition<line={Line}, column={Column}>";
    }
}
