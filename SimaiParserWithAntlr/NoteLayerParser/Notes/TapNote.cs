using SimaiParserWithAntlr.DataModels;
using SimaiParserWithAntlr.Enums;

namespace SimaiParserWithAntlr.NoteLayerParser.Notes;

public class TapNote : NoteBase
{
    public TapNote(string rawText, TextPositionRange range, ButtonEnum button, bool isBreak, bool isEx) : base(rawText, range)
    {
        Button = button;
        IsBreak = isBreak;
        IsEx = isEx;
    }

    public TapNote(string rawText, TextPositionRange range, ButtonEnum button) : base(rawText, range)
    {
        Button = button;
    }

    public ButtonEnum Button { get; set; }
    public bool IsBreak { get; set; }
    public bool IsEx { get; set; }

    public override string GetFormattedString()
    {
        var result = $"{ButtonEnumExt.ToFormattedString(Button)}";

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
