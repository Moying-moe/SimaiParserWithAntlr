using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SimaiParserWithAntlr.Enums;
using SimaiParserWithAntlr.NoteLayerParser.DataModels;
using SimaiParserWithAntlr.NoteLayerParser.Exceptions;

namespace SimaiParserWithAntlr.DataModels.Notes;

public class SlideNote : NoteBase
{
    public SlideNote(NoteTypeEnum type, double bar, double beat, double time, ButtonEnum button, bool isBreakTap,
        bool isExTap, bool isHeadless, List<SlideBody> slideBodies) : base(type, bar, beat, time)
    {
        Button = button;
        IsBreakTap = isBreakTap;
        IsExTap = isExTap;
        IsHeadless = isHeadless;
        SlideBodies = slideBodies;
    }

    public SlideNote(double bar, double beat, double time, ButtonEnum button, bool isBreakTap, bool isExTap,
        bool isHeadless, List<SlideBody> slideBodies) : base(NoteTypeEnum.Slide, bar, beat, time)
    {
        Button = button;
        IsBreakTap = isBreakTap;
        IsExTap = isExTap;
        IsHeadless = isHeadless;
        SlideBodies = slideBodies;
    }

    public SlideNote(double bar, double beat, double time, ButtonEnum button, bool isBreakTap, bool isExTap,
        bool isHeadless) : this(NoteTypeEnum.Slide, bar, beat, time, button, isBreakTap, isExTap, isHeadless,
        new List<SlideBody>())
    {
    }

    [JsonConverter(typeof(ButtonEnumJsonConverter))]
    public ButtonEnum Button { get; set; }

    public bool IsBreakTap { get; set; }
    public bool IsExTap { get; set; }
    public bool IsHeadless { get; set; }
    public List<SlideBody> SlideBodies { get; set; }

    public void AddBody(SlideBody body)
    {
        SlideBodies.Add(body);
    }

    public class SlidePart
    {
        private ButtonEnum _turnButton = ButtonEnum.Unknown;

        public SlidePart(SlideTypeEnum type, ButtonEnum turnButton, ButtonEnum stopButton, NoteDuration? duration)
        {
            _turnButton = turnButton;
            Type = type;
            StopButton = stopButton;
            Duration = duration;
        }

        public SlidePart(SlideTypeEnum type, ButtonEnum stopButton, NoteDuration? duration)
        {
            Type = type;
            StopButton = stopButton;
            Duration = duration;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public SlideTypeEnum Type { get; set; }

        [JsonConverter(typeof(ButtonEnumJsonConverter))]
        public ButtonEnum TurnButton
        {
            get
            {
                if (Type != SlideTypeEnum.Turn)
                {
                    throw new UnsupportedPropInCurrentSlideType();
                }

                return _turnButton;
            }
            set
            {
                if (Type != SlideTypeEnum.Turn)
                {
                    throw new UnsupportedPropInCurrentSlideType();
                }

                _turnButton = value;
            }
        }

        [JsonConverter(typeof(ButtonEnumJsonConverter))]
        public ButtonEnum StopButton { get; set; }

        public NoteDuration? Duration { get; set; }
    }

    public class SlideBody
    {
        public SlideBody(bool isBreakSlide, List<SlidePart> slideChain, NoteDuration? duration)
        {
            IsBreakSlide = isBreakSlide;
            SlideChain = slideChain;
            Duration = duration;
        }

        public SlideBody(bool isBreakSlide, List<SlidePart> slideChain)
        {
            IsBreakSlide = isBreakSlide;
            SlideChain = slideChain;
        }

        public bool IsBreakSlide { get; set; }
        public List<SlidePart> SlideChain { get; set; }

        public NoteDuration? Duration { get; set; }

        // is duration is specified by each slide-part?
        public bool IsChainDuration => Duration == null;
    }
}
