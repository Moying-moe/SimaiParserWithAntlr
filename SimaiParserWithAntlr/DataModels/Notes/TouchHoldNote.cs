using SimaiParserWithAntlr.Enums;

namespace SimaiParserWithAntlr.DataModels.Notes
{

    public class TouchHoldNote : NoteBase
    {
        public TouchHoldNote(double hiSpeed, AreaCodeEnum areaCode, int areaNumber, bool isFirework, NoteTiming duration) :
            base(NoteTypeEnum.Touch,
                hiSpeed)
        {
            AreaCode = areaCode;
            AreaNumber = areaNumber;
            IsFirework = isFirework;
            Duration = duration;
        }

        public AreaCodeEnum AreaCode { get; set; }
        public int AreaNumber { get; set; }
        public bool IsFirework { get; set; }
        public NoteTiming Duration { get; set; }
    }
}