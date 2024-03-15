using SimaiParserWithAntlr.Enums;

namespace SimaiParserWithAntlr.DataModels.Notes
{

    public class HoldNote : NoteBase
    {
        public HoldNote(double hiSpeed, int button, bool isBreak, bool isEx, NoteTiming duration) : base(NoteTypeEnum.Hold,
            hiSpeed)
        {
            Button = button;
            IsBreak = isBreak;
            IsEx = isEx;
            Duration = duration;
        }

        public int Button { get; set; }
        public bool IsBreak { get; set; }
        public bool IsEx { get; set; }

        public NoteTiming Duration { get; set; }
    }
}