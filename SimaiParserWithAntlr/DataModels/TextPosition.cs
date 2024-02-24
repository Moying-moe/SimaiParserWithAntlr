using Antlr4.Runtime;

namespace SimaiParserWithAntlr.DataModels;

public class TextPosition
{
    public static readonly TextPosition EMPTY = new();

    public TextPosition(int line, int column)
    {
        Line = line;
        Column = column;
    }

    public TextPosition(IToken token)
    {
        Line = token.Line;
        Column = token.Column;
    }

    public TextPosition()
    {
        Line = 1;
        Column = 0;
    }

    // Line index starts from 1
    public int Line { get; }

    // Column index starts from 0
    public int Column { get; }

    public override string ToString()
    {
        return $"TextPosition<line={Line}, column={Column}>";
    }

    /**
     * Addition operation produces a new object.
     * Please note that addition operation of TextPosition does not satisfy the commutative property.
     */
    public static TextPosition operator +(TextPosition a, TextPosition b)
    {
        var line = a.Line;
        var col = a.Column;

        if (b.Line == 1)
        {
            // If the line number of b is 1, the new position is on the same line as a, and the column numbers are added.
            col += b.Column;
            return new TextPosition(line, col);
        }

        // Otherwise, the line numbers need to be added, and the column numbers are recalculated starting from 0,
        // which equals the column number of b.
        // Note that line numbers start from 1, so subtracting 1 is necessary when adding.
        line += b.Line - 1;
        col = b.Column;
        return new TextPosition(line, col);
    }
}
