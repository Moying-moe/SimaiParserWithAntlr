using SimaiParserWithAntlr.Enums;

namespace SimaiParserWithAntlr.DataModels.Notes
{

    public class TapNote : NoteBase
    {
        public TapNote(double hiSpeed, int button, bool isBreak, bool isEx, bool isFakeStar) : base(NoteTypeEnum.Tap,
            hiSpeed)
        {
            Button = button;
            IsBreak = isBreak;
            IsEx = isEx;
            IsFakeStar = isFakeStar;
        }

        public int Button { get; set; }
        public bool IsBreak { get; set; }
        public bool IsEx { get; set; }
        public bool IsFakeStar { get; set; }
    }
}