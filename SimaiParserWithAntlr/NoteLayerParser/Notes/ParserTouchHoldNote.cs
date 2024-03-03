using SimaiParserWithAntlr.DataModels;
using SimaiParserWithAntlr.Enums;
using SimaiParserWithAntlr.NoteLayerParser.DataModels;

namespace SimaiParserWithAntlr.NoteLayerParser.Notes;

public class ParserTouchHoldNote : ParserNoteBase
{
    public ParserTouchHoldNote(string rawText, TextPositionRange range, AreaEnum area, bool isFirework, NoteDuration duration) : base(rawText, range)
    {
        Area = area;
        IsFirework = isFirework;
        Duration = duration;
    }

    public AreaEnum Area { get; set; }
    public bool IsFirework { get; set; }
    public NoteDuration Duration { get; set; }

    public override string GetFormattedString()
    {
        var result = $"{AreaEnumExt.ToFormattedString(Area)}";
        
        if (IsFirework)
        {
            result += Constants.FIREWORK_MARK;
        }

        result += Constants.HOLD_MARK;
        result += Duration.GetFormattedString();

        return result;
    }
}
