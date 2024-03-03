using SimaiParserWithAntlr.DataModels;
using SimaiParserWithAntlr.Enums;
using SimaiParserWithAntlr.NoteLayerParser.DataModels;
using SimaiParserWithAntlr.NoteLayerParser.Exceptions;

namespace SimaiParserWithAntlr.NoteLayerParser.Notes;

public class ParserSlideNote : ParserNoteBase
{
    public class SlidePart
    {
        public SlideTypeEnum Type { get; set; }

        private ButtonEnum _turnButton = ButtonEnum.Unknown;
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

        public ButtonEnum StopButton { get; set; }
        public NoteDuration? Duration { get; set; }

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

        public string GetFormattedString()
        {
            var result = "";
            
            result += SlideTypeEnumExt.ToFormattedString(Type);
            
            if (Type == SlideTypeEnum.Turn)
            {
                result += ButtonEnumExt.ToFormattedString(_turnButton);
            }
            result += ButtonEnumExt.ToFormattedString(StopButton);
            
            if (Duration != null)
            {
                result += Duration.GetFormattedString();
            }

            return result;
        }
    }
    
    public class SlideBody
    {
        public bool IsBreakSlide { get; set; }
        public List<SlidePart> SlideChain { get; set; }
        public NoteDuration? Duration { get; set; }
        // is duration is specified by each slide-part?
        public bool IsChainDuration => Duration == null;

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

        public string GetFormattedString()
        {
            // If the slide chain is not using chain duration mode,
            // `result` should end with a button number instead of a duration section.
            // So we can add a break mark directly.
            // Otherwise, we just add the break mark at the end and then return.
            var result = string.Join("", SlideChain.Select(part => part.GetFormattedString()));
            if (IsBreakSlide)
            {
                result += Constants.BREAK_MARK;
            }
            
            if (IsChainDuration)
            {
                return result;
            }
            else
            {
                // Duration is guaranteed not to be null.
                result += Duration!.GetFormattedString();
                return result;
            }
        }
    }

    public ButtonEnum Button { get; set; }
    public bool IsBreakTap { get; set; }
    public bool IsExTap { get; set; }
    public bool IsHeadless { get; set; }
    public List<SlideBody> SlideBodies { get; set; }

    public ParserSlideNote(string rawText, TextPositionRange range, ButtonEnum button, bool isBreakTap, bool isExTap,
        bool isHeadless, List<SlideBody> slideBodies) : base(rawText, range)
    {
        Button = button;
        IsBreakTap = isBreakTap;
        IsExTap = isExTap;
        IsHeadless = isHeadless;
        SlideBodies = slideBodies;
    }

    public ParserSlideNote(string rawText, TextPositionRange range, ButtonEnum button, bool isBreakTap, bool isExTap,
        bool isHeadless) : this(rawText, range, button, isBreakTap, isExTap, isHeadless, new List<SlideBody>())
    {
    }

    public void AddBody(SlideBody body)
    {
        SlideBodies.Add(body);
    }

    public override string GetFormattedString()
    {
        var result = $"{ButtonEnumExt.ToFormattedString(Button)}";
        
        if (IsBreakTap)
        {
            result += Constants.BREAK_MARK;
        }

        if (IsExTap)
        {
            result += Constants.EX_MARK;
        }

        result += string.Join("*", SlideBodies.Select(body => body.GetFormattedString()));

        return result;
    }
}
