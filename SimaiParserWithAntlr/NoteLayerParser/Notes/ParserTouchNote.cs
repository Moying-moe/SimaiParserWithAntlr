using SimaiParserWithAntlr.DataModels;
using SimaiParserWithAntlr.Enums;

namespace SimaiParserWithAntlr.NoteLayerParser.Notes;

public class ParserTouchNote : ParserNoteBase
{
    public ParserTouchNote(string rawText, TextPositionRange range, AreaCodeEnum areaCode, int areaNumber,
        bool isFirework) : base(rawText, range)
    {
        AreaCode = areaCode;
        AreaNumber = areaNumber;
        IsFirework = isFirework;
    }

    public AreaCodeEnum AreaCode { get; set; }
    public int AreaNumber { get; set; }
    public bool IsFirework { get; set; }

    public override string GetFormattedString()
    {
        var result = $"{AreaCode}{AreaNumber}";

        if (IsFirework)
        {
            result += Constants.FIREWORK_MARK;
        }

        return result;
    }
}
