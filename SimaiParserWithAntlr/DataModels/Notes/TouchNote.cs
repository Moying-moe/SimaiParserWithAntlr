using SimaiParserWithAntlr.Enums;

namespace SimaiParserWithAntlr.DataModels.Notes;

public class TouchNote : NoteBase
{
    public TouchNote(double hiSpeed, AreaCodeEnum areaCode, int areaNumber, bool isFirework) : base(NoteTypeEnum.Touch,
        hiSpeed)
    {
        AreaCode = areaCode;
        AreaNumber = areaNumber;
        IsFirework = isFirework;
    }

    public AreaCodeEnum AreaCode { get; set; }
    public int AreaNumber { get; set; }
    public bool IsFirework { get; set; }
}
