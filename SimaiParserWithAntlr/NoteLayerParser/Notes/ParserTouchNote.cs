using SimaiParserWithAntlr.DataModels;
using SimaiParserWithAntlr.Enums;

namespace SimaiParserWithAntlr.NoteLayerParser.Notes;

public class ParserTouchNote : ParserNoteBase
{
    public ParserTouchNote(string rawText, TextPositionRange range, AreaEnum area, bool isFirework) : base(rawText, range)
    {
        Area = area;
        IsFirework = isFirework;
    }

    public AreaEnum Area { get; set; }
    public bool IsFirework { get; set; }

    public override string GetFormattedString()
    {
        var result = $"{AreaEnumExt.ToFormattedString(Area)}";

        if (IsFirework)
        {
            result += Constants.FIREWORK_MARK;
        }

        return result;
    }
}
