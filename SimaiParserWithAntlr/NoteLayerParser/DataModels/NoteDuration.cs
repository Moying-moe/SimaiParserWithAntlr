using SimaiParserWithAntlr.Enums;
using SimaiParserWithAntlr.NoteLayerParser.Exceptions;

namespace SimaiParserWithAntlr.NoteLayerParser.DataModels
{

    public class NoteDuration
    {
        private float _bpm;
        private float _delay;

        private int _fracDenominator;
        private int _fracNumerator;
        private float _time;
        public DurationTypeEnum Type { get; private set; }

        public int FracDenominator
        {
            get
            {
                if (!IsFractionType)
                {
                    throw new UnsupportedPropInCurrentDurationType();
                }

                return _fracDenominator;
            }
            set
            {
                if (!IsFractionType)
                {
                    throw new UnsupportedPropInCurrentDurationType();
                }

                _fracDenominator = value;
            }
        }

        public int FracNumerator
        {
            get
            {
                if (!IsFractionType)
                {
                    throw new UnsupportedPropInCurrentDurationType();
                }

                return _fracNumerator;
            }
            set
            {
                if (!IsFractionType)
                {
                    throw new UnsupportedPropInCurrentDurationType();
                }

                _fracNumerator = value;
            }
        }

        public float Time
        {
            get
            {
                if (!IsTimeType)
                {
                    throw new UnsupportedPropInCurrentDurationType();
                }

                return _time;
            }
            set
            {
                if (!IsTimeType)
                {
                    throw new UnsupportedPropInCurrentDurationType();
                }

                _time = value;
            }
        }

        public float Bpm
        {
            get
            {
                if (!IsBpmType)
                {
                    throw new UnsupportedPropInCurrentDurationType();
                }

                return _bpm;
            }
            set
            {
                if (!IsBpmType)
                {
                    throw new UnsupportedPropInCurrentDurationType();
                }

                _bpm = value;
            }
        }

        public float Delay
        {
            get
            {
                if (!IsDelayType)
                {
                    throw new UnsupportedPropInCurrentDurationType();
                }

                return _delay;
            }
            set
            {
                if (!IsDelayType)
                {
                    throw new UnsupportedPropInCurrentDurationType();
                }

                _delay = value;
            }
        }

        public bool IsFractionType => Type is DurationTypeEnum.Fraction or DurationTypeEnum.BpmFraction
            or DurationTypeEnum.DelayFraction or DurationTypeEnum.DelayBpmFraction;

        public bool IsTimeType => Type is DurationTypeEnum.Time or DurationTypeEnum.BpmTime or DurationTypeEnum.DelayTime;

        public bool IsBpmType =>
            Type is DurationTypeEnum.BpmFraction or DurationTypeEnum.BpmTime or DurationTypeEnum.DelayBpmFraction;

        public bool IsDelayType => Type is DurationTypeEnum.DelayFraction or DurationTypeEnum.DelayTime
            or DurationTypeEnum.DelayBpmFraction;

        public bool IsEmpty => Type == DurationTypeEnum.Empty;

        public static NoteDuration FromFraction(int denominator, int numerator)
        {
            return new NoteDuration
            {
                Type = DurationTypeEnum.Fraction,
                _fracDenominator = denominator,
                _fracNumerator = numerator
            };
        }

        public static NoteDuration FromBpmFraction(float bpm, int denominator, int numerator)
        {
            return new NoteDuration
            {
                Type = DurationTypeEnum.BpmFraction,
                _bpm = bpm,
                _fracDenominator = denominator,
                _fracNumerator = numerator
            };
        }

        public static NoteDuration FromTime(float time)
        {
            return new NoteDuration
            {
                Type = DurationTypeEnum.Time,
                _time = time
            };
        }

        public static NoteDuration FromBpmTime(float bpm, float time)
        {
            return new NoteDuration
            {
                Type = DurationTypeEnum.BpmTime,
                _bpm = bpm,
                _time = time
            };
        }

        public static NoteDuration FromDelayFraction(float delay, int denominator, int numerator)
        {
            return new NoteDuration
            {
                Type = DurationTypeEnum.DelayFraction,
                _delay = delay,
                _fracDenominator = denominator,
                _fracNumerator = numerator
            };
        }

        public static NoteDuration FromDelayTime(float delay, float time)
        {
            return new NoteDuration
            {
                Type = DurationTypeEnum.DelayTime,
                _delay = delay,
                _time = time
            };
        }

        public static NoteDuration FromDelayBpmFraction(float delay, float bpm, int denominator, int numerator)
        {
            return new NoteDuration
            {
                Type = DurationTypeEnum.DelayBpmFraction,
                _delay = delay,
                _bpm = bpm,
                _fracDenominator = denominator,
                _fracNumerator = numerator
            };
        }

        public static NoteDuration Empty()
        {
            return new NoteDuration
            {
                Type = DurationTypeEnum.Empty
            };
        }

        public string GetFormattedString()
        {
            switch (Type)
            {
                case DurationTypeEnum.Fraction:
                    return $"[{_fracDenominator}:{_fracNumerator}]";
                case DurationTypeEnum.Time:
                    return $"[#{_time}]";
                case DurationTypeEnum.BpmFraction:
                    return $"[{_bpm}#{_fracDenominator}:{_fracNumerator}]";
                case DurationTypeEnum.BpmTime:
                    return $"[{_bpm}#{_time}]";
                case DurationTypeEnum.DelayFraction:
                    return $"[{_delay}##{_fracDenominator}:{_fracNumerator}]";
                case DurationTypeEnum.DelayTime:
                    return $"[{_delay}##{_time}]";
                case DurationTypeEnum.DelayBpmFraction:
                    return $"[{_delay}##{_bpm}#{_fracDenominator}:{_fracNumerator}]";
                case DurationTypeEnum.Empty:
                case DurationTypeEnum.Unknown:
                default:
                    return "";
            }
        }
    }

}