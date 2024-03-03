using SimaiParserWithAntlr.DataModels;
using SimaiParserWithAntlr.Enums;
using SimaiParserWithAntlr.NoteLayerParser.DataModels;

namespace SimaiParserWithAntlr.NoteLayerParser.Notes;

public class ParserHoldNote : ParserNoteBase
{
    public ParserHoldNote(string rawText, TextPositionRange range, ButtonEnum button, bool isBreak, bool isEx, NoteDuration duration) : base(rawText, range)
    {
        Button = button;
        IsBreak = isBreak;
        IsEx = isEx;
        Duration = duration;
    }

    public ButtonEnum Button { get; set; }
    public bool IsBreak { get; set; }
    public bool IsEx { get; set; }
    public NoteDuration Duration { get; set; }

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

        result += Constants.HOLD_MARK;
        result += Duration.GetFormattedString();

        return result;
    }
}
