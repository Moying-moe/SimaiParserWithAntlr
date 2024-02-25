using SimaiParserWithAntlr.NoteLayerParser;
using SimaiParserWithAntlr.NoteLayerParser.DataModels;
using SimaiParserWithAntlr.NoteLayerParser.Enums;
using SimaiParserWithAntlr.NoteLayerParser.Notes;
using Xunit.Abstractions;

namespace ParserTest.NoteBlockTest;

public class NoteBlockTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public NoteBlockTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    private const double EPS = 1e-5;

    private static bool FloatEquals(float a, float b)
    {
        return Math.Abs(a - b) < EPS;
    }

    [Fact]
    public void Hold_SimpleTest()
    {
        NoteBlockWalker.GenerateFromText("1h,");
    }
    
    [Fact]
    public void Hold_CorrectFormat()
    {
        Dictionary<string, Func<HoldNote, bool>> markCheckMap = new()
        {
            {"", note => note is { IsBreak: false, IsEx: false }},
            {"b", note => note is { IsBreak: true, IsEx: false }},
            {"x", note => note is { IsBreak: false, IsEx: true }},
            {"bx", note => note is { IsBreak: true, IsEx: true }},
        };
        Dictionary<string, Func<HoldNote, bool>> durCheckMap = new()
        {
            {
                "[8:1]",
                note => note.Duration is { Type: DurationTypeEnum.Fraction, FracDenominator: 8, FracNumerator: 1 }
            },
            {
                "[120#8:1]",
                note => note.Duration is { Type: DurationTypeEnum.BpmFraction, FracDenominator: 8, FracNumerator: 1 } &&
                        FloatEquals(note.Duration.Bpm, 120)
            },
            { "[#0.5]", note => note.Duration.Type == DurationTypeEnum.Time && FloatEquals(note.Duration.Time, 0.5f) },
            {
                "[120#0.5]",
                note => note.Duration.Type == DurationTypeEnum.BpmTime && FloatEquals(note.Duration.Bpm, 120) &&
                        FloatEquals(note.Duration.Time, 0.5f)
            },
            {
                "[0.3##8:1]",
                note =>
                    note.Duration is { Type: DurationTypeEnum.DelayFraction, FracDenominator: 8, FracNumerator: 1 } &&
                    FloatEquals(note.Duration.Delay, 0.3f)
            },
            {
                "[0.3##1.5]",
                note => note.Duration.Type == DurationTypeEnum.DelayTime && FloatEquals(note.Duration.Delay, 0.3f) &&
                        FloatEquals(note.Duration.Time, 1.5f)
            },
            {
                "[0.3##120#8:1]",
                note => note.Duration is
                            { Type: DurationTypeEnum.DelayBpmFraction, FracDenominator: 8, FracNumerator: 1 } &&
                        FloatEquals(note.Duration.Delay, 0.3f) && FloatEquals(note.Duration.Bpm, 120)
            },
            { "", note => note.Duration.Type == DurationTypeEnum.Empty },
        };

        Dictionary<string, Func<HoldNote, bool>> checkMap = new();

        foreach (var mark in markCheckMap)
        {
            foreach (var dur in durCheckMap)
            {
                var noteString = $"1{mark.Key}h{dur.Key},";
                var result = NoteBlockWalker.GenerateFromText(noteString);
                
                Assert.Single(result.NoteGroupList);
                Assert.Empty(result.ErrorList);
                Assert.Empty(result.WarningList);
                
                var noteGroup = result.NoteGroupList[0];
                Assert.Single(noteGroup.NoteList);
                Assert.Single(noteGroup.NoteList[0]);
                
                var note = (HoldNote) noteGroup.NoteList[0][0];
                Assert.NotNull(note);
                Assert.True(mark.Value(note));
                Assert.True(dur.Value(note));
                
                _testOutputHelper.WriteLine($"{noteString}\t{note.GetFormattedString()}\t{note}");
            }
        }
    }
}
