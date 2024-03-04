using SimaiParserWithAntlr.Enums;
using SimaiParserWithAntlr.Utils;

namespace SimaiParserWithAntlr.DataModels.Notes;

public class SlideNote : NoteBase
{
    public SlideNote(double hiSpeed, int button, bool isBreakTap, bool isExTap, bool isHeadless,
        List<SlideBody> slideBodies) : base(NoteTypeEnum.Slide, hiSpeed)
    {
        Button = button;
        IsBreakTap = isBreakTap;
        IsExTap = isExTap;
        IsHeadless = isHeadless;
        SlideBodies = slideBodies;
    }

    public SlideNote(double hiSpeed, int button, bool isBreakTap, bool isExTap, bool isHeadless) : this(hiSpeed, button,
        isBreakTap, isExTap, isHeadless, new List<SlideBody>())
    {
    }

    public int Button { get; set; }
    public bool IsBreakTap { get; set; }
    public bool IsExTap { get; set; }
    public bool IsHeadless { get; set; }
    public List<SlideBody> SlideBodies { get; set; }

    public class SlideBody
    {
        public SlideBody(bool isBreakSlide, List<SlidePart> slideChain, NoteTiming delay, NoteTiming? duration)
        {
            IsBreakSlide = isBreakSlide;
            SlideChain = slideChain;
            Delay = delay;
            Duration = duration;
        }

        public bool IsBreakSlide { get; set; }
        public List<SlidePart> SlideChain { get; set; }

        public NoteTiming Delay { get; set; }

        public NoteTiming? Duration { get; set; }

        // is duration is specified by each slide-part?
        public bool IsChainDuration => Duration == null;
    }

    public class SlidePart
    {
        public SlidePart(SlideTypeEnum type, int turnButton, int stopButton, NoteTiming? delay, NoteTiming? duration)
        {
            Type = type;
            TurnButton = turnButton;
            StopButton = stopButton;
            Delay = delay;
            Duration = duration;
        }

        public SlideTypeEnum Type { get; set; }

        public int TurnButton { get; set; } = ButtonHelper.UNKNOWN_BUTTON;
        public int StopButton { get; set; }

        /**
         * Specify the delay between this part and the previous part.
         * Generally, its value is 0. **This attribute is for future features.**
         * 
         * Note that if the slide duration is specified by each slide-part,
         * the first part of the slide's delay is **0**.
         * And the delay of this whole slide is specified by `SlideBody::Delay`
         */
        public NoteTiming? Delay { get; set; }

        public NoteTiming? Duration { get; set; }
    }
}
