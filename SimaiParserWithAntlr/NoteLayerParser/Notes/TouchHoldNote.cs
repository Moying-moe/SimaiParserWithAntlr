using SimaiParserWithAntlr.DataModels;
using SimaiParserWithAntlr.NoteLayerParser.DataModels;
using SimaiParserWithAntlr.NoteLayerParser.Enums;

namespace SimaiParserWithAntlr.NoteLayerParser.Notes;

public class TouchHoldNote : NoteBase
{
    public TouchHoldNote(TextPositionRange range, AreaEnum area, bool isFirework, NoteDuration duration) : base(range)
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
