using SimaiParserWithAntlr.DataModels;

namespace SimaiParserWithAntlr.NoteLayerParser.Notes;

public class ParserTapNote : ParserNoteBase
{
    public ParserTapNote(string rawText, TextPositionRange range, int button, bool isBreak, bool isEx) : base(rawText,
        range)
    {
        Button = button;
        IsBreak = isBreak;
        IsEx = isEx;
    }

    public ParserTapNote(string rawText, TextPositionRange range, int button) : base(rawText, range)
    {
        Button = button;
    }

    public int Button { get; set; }
    public bool IsBreak { get; set; }
    public bool IsEx { get; set; }

    public override string GetFormattedString()
    {
        var result = $"{Button}";

        if (IsBreak)
        {
            result += Constants.BREAK_MARK;
        }

        if (IsEx)
        {
            result += Constants.EX_MARK;
        }

        return result;
    }
}
