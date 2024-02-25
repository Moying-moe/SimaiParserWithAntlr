using SimaiParserWithAntlr.DataModels;
using SimaiParserWithAntlr.NoteLayerParser.Enums;

namespace SimaiParserWithAntlr.NoteLayerParser.Notes;

public class TapNote : NoteBase
{
    public TapNote(TextPositionRange range, ButtonEnum button, bool isBreak, bool isEx) : base(range)
    {
        Button = button;
        IsBreak = isBreak;
        IsEx = isEx;
    }

    public TapNote(TextPositionRange range, ButtonEnum button) : base(range)
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
