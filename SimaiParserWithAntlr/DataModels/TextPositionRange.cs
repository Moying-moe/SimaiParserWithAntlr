using Antlr4.Runtime;

namespace SimaiParserWithAntlr.DataModels;

public class TextPositionRange
{
    public static readonly TextPositionRange EMPTY = new(TextPosition.EMPTY, TextPosition.EMPTY);

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

    public TextPosition Start { get; set; }
    public TextPosition Stop { get; set; }

    public void ApplyOffset(TextPosition offset)
    {
        Start = offset + Start;
        Stop = offset + Stop;
    }

    public override string ToString()
    {
        return $"TextPositionRange<start={Start}, stop={Stop}>";
    }

    private int FindNthIndex(string str, string value, int count)
    {
        if (count == 0)
        {
            return 0;
        }

        var index = -1;
        for (var i = 0; i < count; i++)
        {
            index = str.IndexOf(value, index + 1, StringComparison.Ordinal);
            if (index == -1)
            {
                return str.Length;
            }
        }

        return index;
    }

    public string GetPositionedString(string text, int prefixExtra = 0, int suffixExtra = 0, bool showEllipsis = true,
        bool highlightRange = false)
    {
        var startLine = Start.Line - 1;
        var stopLine = Stop.Line;

        // 取出 startLine 到 stopLine 之间的文本 并分割为string[]
        var lineStart = FindNthIndex(text, "\n", startLine) + 1;
        var lineEnd = FindNthIndex(text, "\n", stopLine);
        var positionedLines = text.Substring(lineStart, lineEnd - lineStart).Split("\n");
        var widths = positionedLines.Select(line => line.Length).ToArray();

        var startCol = Start.Column;
        var stopCol = Stop.Column;

        var prefixSpace = startCol;
        var suffixSpace = positionedLines.Last().Length - stopCol;

        var hasPrefixEllipsis = showEllipsis && prefixSpace > prefixExtra;
        var hasSuffixEllipsis = showEllipsis && suffixSpace > suffixExtra;

        startCol = Math.Max(0, startCol - prefixExtra);
        stopCol = Math.Min(positionedLines.Last().Length, stopCol + suffixExtra);

        if (positionedLines.Length == 1)
        {
            // 只有一行
            positionedLines[0] = positionedLines[0].Substring(startCol, stopCol - startCol);
        }
        else
        {
            // 存在多行
            positionedLines[0] = positionedLines[0][startCol..];
            positionedLines[^1] = positionedLines[^1][..stopCol];
        }

        if (hasPrefixEllipsis)
        {
            positionedLines[0] = "..." + positionedLines[0];

            for (var i = 1; i < positionedLines.Length; i++)
            {
                positionedLines[i] = "   " + positionedLines[i];
            }
        }

        if (hasSuffixEllipsis)
        {
            positionedLines[^1] += "...";
        }

        if (highlightRange)
        {
            if (positionedLines.Length == 1)
            {
                positionedLines[0] += "\n";
                if (hasPrefixEllipsis)
                {
                    positionedLines[0] += string.Join("", Enumerable.Repeat(" ", prefixExtra + 3));
                }
                else
                {
                    positionedLines[0] += string.Join("", Enumerable.Repeat(" ", Start.Column));
                }

                positionedLines[0] += string.Join("", Enumerable.Repeat("^", Stop.Column - Start.Column));
            }
            else
            {
                positionedLines[0] += "\n";
                if (hasPrefixEllipsis)
                {
                    positionedLines[0] += string.Join("", Enumerable.Repeat(" ", prefixExtra + 3));
                }
                else
                {
                    positionedLines[0] += string.Join("", Enumerable.Repeat(" ", Start.Column));
                }

                positionedLines[0] += string.Join("", Enumerable.Repeat("^", widths[0] - Start.Column));

                for (var i = 1; i < positionedLines.Length - 1; i++)
                {
                    positionedLines[i] += "\n";
                    if (hasPrefixEllipsis)
                    {
                        positionedLines[i] += string.Join("", Enumerable.Repeat(" ", 3));
                    }

                    positionedLines[i] += string.Join("", Enumerable.Repeat("^", widths[i]));
                }

                positionedLines[^1] += "\n";
                if (hasPrefixEllipsis)
                {
                    positionedLines[^1] += string.Join("", Enumerable.Repeat(" ", 3));
                }

                positionedLines[^1] += string.Join("", Enumerable.Repeat("^", Stop.Column + 1));
            }
        }

        return string.Join("\n", positionedLines);
    }
}
