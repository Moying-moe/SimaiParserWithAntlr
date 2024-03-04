using MathNet.Numerics;
using SimaiParserWithAntlr.DataModels;
using SimaiParserWithAntlr.DataModels.Notes;
using SimaiParserWithAntlr.Enums;
using SimaiParserWithAntlr.NoteLayerParser;
using SimaiParserWithAntlr.NoteLayerParser.DataModels;
using SimaiParserWithAntlr.NoteLayerParser.Notes;
using SimaiParserWithAntlr.StructureLayerParser;
using SimaiParserWithAntlr.StructureLayerParser.Structures;

namespace SimaiParserWithAntlr;

public class ChartParser
{
    public const int DEFAULT_RESOLUTION = 384;
    
    public string RawText { get; private set; }
    public List<ChartBpm> BpmList { get; } = new();
    public List<EachGroup> NoteList { get; } = new();
    public int Resolution { get; private set; } = DEFAULT_RESOLUTION;

    private NoteTiming _fakeEachInterval = new NoteTiming(0, 1);
    private NoteTiming _timing;

    private ChartParser(string rawText)
    {
        RawText = rawText;
    }

    private void Analyze()
    {
        var structureParser = ChartStructureParser.GenerateFromText(RawText);

        Resolution = DEFAULT_RESOLUTION;
        foreach (var element in structureParser.ElementList)
        {
            // calculate the resolution
            if (element is ResolutionElement resolutionElement)
            {
                Resolution = (int)Euclid.LeastCommonMultiple(Resolution, resolutionElement.Resolution);
            }
        }

        _timing = new NoteTiming(1, 0, 0);
        _fakeEachInterval = new NoteTiming(0, DEFAULT_RESOLUTION / Resolution);
        
        foreach (var element in structureParser.ElementList)
        {
            if (element is BpmElement bpmElement)
            {
                BpmList.Add(new ChartBpm(_timing.Clone(), bpmElement.Bpm));
                continue;
            }

            if (element is not NoteBlockElement noteBlockElement)
            {
                continue;
            }
            
            // analyze note block
            var noteParser = NoteParser.GenerateFromText(noteBlockElement.RawText, noteBlockElement.Range.Start);
            AnalyzeNoteBlock(noteParser, (double)noteBlockElement.Bpm!, (int)noteBlockElement.Resolution!,
                noteBlockElement.HiSpeed);
        }
        
        // TODO: combine same time but not same group in `NoteList`
        Dictionary<NoteTiming, EachGroup> mergeDict = new();
        foreach (var group in NoteList)
        {
            if (mergeDict.ContainsKey(group.Timing))
            {
                mergeDict[group.Timing].NoteList.AddRange(group.NoteList);
            }
            else
            {
                mergeDict.Add(group.Timing, group);
            }
        }

        NoteList.Clear();
        NoteList.AddRange(mergeDict.Values);
        NoteList.Sort((a, b) => a.Timing.CompareTo(b.Timing));
        
    }

    private void AnalyzeNoteBlock(NoteParser noteParser, double blockBpm, int blockResolution, double blockHiSpeed)
    {
        var groupInterval = NoteTiming.FromBeat(Resolution / blockResolution, Resolution);
        
        foreach (var noteGroup in noteParser.NoteGroupList)
        {
            AnalyzeNoteGroup(noteGroup, blockBpm, blockHiSpeed);
            _timing.Add(groupInterval, Resolution);
            _timing.Time = CalculateTime(_timing);
        }
    }

    private void AnalyzeNoteGroup(ParserNoteGroup noteGroup, double bpm, double hiSpeed)
    {
        var noteGroupStartTiming = _timing.Clone();
        
        foreach (var group in noteGroup.NoteList)
        {
            var eachGroup = new EachGroup(noteGroupStartTiming.Clone());
            
            foreach (var note in group)
            {
                switch (note)
                {
                    case ParserTapNote pTap:
                        eachGroup.NoteList.Add(new TapNote(hiSpeed, pTap.Button, pTap.IsBreak, pTap.IsEx, false));
                        break;
                    case ParserHoldNote pHold:
                        ParseDuration(pHold.Duration, bpm, out var hDuration);
                        eachGroup.NoteList.Add(new HoldNote(hiSpeed, pHold.Button, pHold.IsBreak, pHold.IsEx, hDuration));
                        break;
                    case ParserTouchNote pTouch:
                        eachGroup.NoteList.Add(new TouchNote(hiSpeed, pTouch.AreaCode, pTouch.AreaNumber,
                            pTouch.IsFirework));
                        break;
                    case ParserTouchHoldNote pTouchHold:
                        ParseDuration(pTouchHold.Duration, bpm, out var thDuration);
                        eachGroup.NoteList.Add(new TouchHoldNote(hiSpeed, pTouchHold.AreaCode, pTouchHold.AreaNumber,
                            pTouchHold.IsFirework, thDuration));
                        break;
                    case ParserSlideNote pSlide:
                        SlideNote slide = new(hiSpeed, pSlide.Button, pSlide.IsBreakTap, pSlide.IsExTap,
                            pSlide.IsHeadless);
                        foreach (var pBody in pSlide.SlideBodies)
                        {
                            if (pBody.IsChainDuration)
                            {
                                NoteTiming? overallDelay = null;
                                List<SlideNote.SlidePart> slideChain = new();
                                foreach (var pPart in pBody.SlideChain)
                                {
                                    ParseDelayDuration(pPart.Duration!, bpm, out var delay, out var duration);
                                    slideChain.Add(new SlideNote.SlidePart(pPart.Type, pPart.TurnButton,
                                        pPart.StopButton, delay, duration));
                                    
                                    if (overallDelay == null)
                                    {
                                        overallDelay = delay;
                                    }
                                }

                                slide.SlideBodies.Add(new SlideNote.SlideBody(pBody.IsBreakSlide, slideChain,
                                    overallDelay!, null));
                            }
                            else
                            {
                                List<SlideNote.SlidePart> slideChain = new();
                                foreach (var pPart in pBody.SlideChain)
                                {
                                    slideChain.Add(new SlideNote.SlidePart(pPart.Type, pPart.TurnButton,
                                        pPart.StopButton, null, null));
                                }
                                ParseDelayDuration(pBody.Duration!, bpm, out var delay, out var duration);
                                slide.SlideBodies.Add(new SlideNote.SlideBody(pBody.IsBreakSlide, slideChain, delay,
                                    duration));
                            }
                        }

                        eachGroup.NoteList.Add(slide);
                        break;
                }
            }
            
            NoteList.Add(eachGroup);

            // fake each interval for next group
            noteGroupStartTiming.Add(_fakeEachInterval, Resolution);
            noteGroupStartTiming.Time = CalculateTime(noteGroupStartTiming);
        }
    }

    /**
     * TODO:
     * Currently, we cannot reconstruct the original chart text from the `NoteList` generated by the `ChartParser`.
     * This is because we use `NoteTiming` to record durations, such as the delay and duration of slides, and the
     * duration of holds.
     * Different syntax use different ways to describe these durations. For example, in simai, it allows special BPMs to
     * be used for a specific duration, but some languages do not support this and only allow the use of global BPM.
     * However, NoteTiming does not record these details. Instead, it always uses the BPM from the BpmList. This even
     * leads to NoteTiming being unable to independently describe a time point.
     * 
     * This may be a design flaw, and we may need a more scientific way of describing it.
     * However, so far, because each NoteTiming always calculates its actual time value, it is sufficient for reading
     * chart and serve for playing. But as a syntax parser, it may not be enough (if for chart converter purposes).
     */
    
    private void ParseDelayDuration(NoteDuration duration, double bpm, out NoteTiming delay, out NoteTiming timing)
    {
        switch (duration.Type)
        {
            case DurationTypeEnum.DelayFraction or DurationTypeEnum.DelayTime or DurationTypeEnum.DelayBpmFraction:
                delay = new NoteTiming(0, 0, duration.Delay);
                break;
            case DurationTypeEnum.BpmTime or DurationTypeEnum.BpmFraction:
                delay = NoteTiming.FromBeat(Resolution / 4, Resolution);
                delay.Time = CalculateDurationTime(delay, duration.Bpm);
                break;
            default:
                // Fraction, Time, Empty, Unknown
                delay = NoteTiming.FromBeat(Resolution / 4, Resolution);
                delay.Time = CalculateDurationTime(delay, bpm);
                break;
        }

        switch (duration.Type)
        {
            case DurationTypeEnum.Time or DurationTypeEnum.BpmTime or DurationTypeEnum.DelayTime:
                timing = new NoteTiming(0, 0, duration.Time);
                break;
            case DurationTypeEnum.BpmFraction or DurationTypeEnum.DelayBpmFraction:
                timing = NoteTiming.FromBeat(Resolution / duration.FracDenominator * duration.FracNumerator,
                    Resolution);
                timing.Time = CalculateDurationTime(timing, duration.Bpm);
                break;
            case DurationTypeEnum.Fraction or DurationTypeEnum.DelayFraction:
                timing = NoteTiming.FromBeat(Resolution / duration.FracDenominator * duration.FracNumerator,
                    Resolution);
                timing.Time = CalculateDurationTime(timing, bpm);
                break;
            default:
                // Empty, Unknown
                timing = new NoteTiming(0, 0, 0);
                break;
        }
    }

    private void ParseDuration(NoteDuration duration, double bpm, out NoteTiming timing)
    {
        switch (duration.Type)
        {
            case DurationTypeEnum.Fraction or DurationTypeEnum.DelayFraction:
                timing = NoteTiming.FromBeat(
                    Resolution / duration.FracDenominator * duration.FracNumerator, Resolution);
                timing.Time = CalculateDurationTime(timing, bpm);
                break;
            case DurationTypeEnum.BpmFraction or DurationTypeEnum.DelayBpmFraction:
                timing = NoteTiming.FromBeat(
                    Resolution / duration.FracDenominator * duration.FracNumerator, Resolution);
                timing.Time = CalculateDurationTime(timing, duration.Bpm);
                break;
            case DurationTypeEnum.Time or DurationTypeEnum.BpmTime
                or DurationTypeEnum.DelayTime:
                timing = new NoteTiming(0, 0, duration.Time);
                break;
            case DurationTypeEnum.Empty or DurationTypeEnum.Unknown:
            default:
                timing = new NoteTiming(0, 0, 0);
                break;
        }
    }

    /**
     * Calculate the note timing with bpm list
     */
    private double CalculateTime(NoteTiming timing)
    {
        var lastBpm = BpmList.First();
        
        double time = 0;

        foreach (var bpm in BpmList.Skip(1))
        {
            if (timing < bpm.Timing)
            {
                lastBpm = bpm;
                break;
            }
            
            var bpmSecDuration = NoteTiming.Subtract(bpm.Timing, lastBpm.Timing, Resolution);
            time += bpmSecDuration.Bar * 240d / lastBpm.Bpm + bpmSecDuration.Beat * 240d / lastBpm.Bpm / Resolution;
            lastBpm = bpm;
        }
        
        var lastBpmSecDuration = NoteTiming.Subtract(timing, lastBpm.Timing, Resolution);
        time += lastBpmSecDuration.Bar * 240d / lastBpm.Bpm + lastBpmSecDuration.Beat * 240d / lastBpm.Bpm / Resolution;
        
        return time;
    }

    /**
     * Time calculation for duration
     */
    private double CalculateDurationTime(NoteTiming timing, double bpm)
    {
        return timing.Bar * 240d / bpm + timing.Beat * 240d / bpm / Resolution;
    }

    public static ChartParser GenerateFromText(string text)
    {
        var parser = new ChartParser(text);
        parser.Analyze();
        return parser;
    }
}
