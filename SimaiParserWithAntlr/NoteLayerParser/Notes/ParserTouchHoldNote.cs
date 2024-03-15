using SimaiParserWithAntlr.DataModels;
using SimaiParserWithAntlr.Enums;
using SimaiParserWithAntlr.NoteLayerParser.DataModels;

namespace SimaiParserWithAntlr.NoteLayerParser.Notes
{

    public class ParserTouchHoldNote : ParserNoteBase
    {
        public ParserTouchHoldNote(string rawText, TextPositionRange range, AreaCodeEnum areaCode, int areaNumber,
            bool isFirework, NoteDuration duration) : base(rawText, range)
        {
            AreaCode = areaCode;
            AreaNumber = areaNumber;
            IsFirework = isFirework;
            Duration = duration;
        }

        public AreaCodeEnum AreaCode { get; set; }
        public int AreaNumber { get; set; }
        public bool IsFirework { get; set; }
        public NoteDuration Duration { get; set; }

        public override string GetFormattedString()
        {
            var result = $"{AreaCode}{AreaNumber}";

            if (IsFirework)
            {
                result += Constants.FIREWORK_MARK;
            }

            result += Constants.HOLD_MARK;
            result += Duration.GetFormattedString();

            return result;
        }
    }
}