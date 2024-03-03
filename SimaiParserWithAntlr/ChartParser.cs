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
                BpmList.Add(new ChartBpm(_timing, bpmElement.Bpm));
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
            var eachGroup = new EachGroup(noteGroupStartTiming);
            
            foreach (var note in group)
            {
                switch (note)
                {
                    case ParserTapNote pTap:
                        eachGroup.NoteList.Add(new TapNote(hiSpeed, pTap.Button, pTap.IsBreak, pTap.IsEx, false));
                        break;
                    case ParserHoldNote pHold:
                    {
                        NoteTiming duration;
                        switch (pHold.Duration.Type)
                        {
                            case DurationTypeEnum.Fraction or DurationTypeEnum.DelayFraction:
                                duration = NoteTiming.FromBeat(
                                    Resolution / pHold.Duration.FracDenominator * pHold.Duration.FracNumerator, Resolution);
                                duration.Time = CalculateDurationTime(duration, bpm);
                                break;
                            case DurationTypeEnum.BpmFraction or DurationTypeEnum.DelayBpmFraction:
                                duration = NoteTiming.FromBeat(
                                    Resolution / pHold.Duration.FracDenominator * pHold.Duration.FracNumerator, Resolution);
                                duration.Time = CalculateDurationTime(duration, pHold.Duration.Bpm);
                                break;
                            case DurationTypeEnum.Time or DurationTypeEnum.BpmTime
                                or DurationTypeEnum.DelayTime:
                                duration = new NoteTiming(0, 0, pHold.Duration.Time);
                                break;
                            case DurationTypeEnum.Empty or DurationTypeEnum.Unknown:
                            default:
                                duration = new NoteTiming(0, 0, 0);
                                break;
                        }
                        eachGroup.NoteList.Add(new HoldNote(hiSpeed, pHold.Button, pHold.IsBreak, pHold.IsEx, duration));
                        break;
                    }
                }
            }

            // fake each interval for next group
            noteGroupStartTiming.Add(_fakeEachInterval, Resolution);
            noteGroupStartTiming.Time = CalculateTime(noteGroupStartTiming);
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
