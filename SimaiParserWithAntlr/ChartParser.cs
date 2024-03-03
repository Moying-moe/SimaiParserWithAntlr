using SimaiParserWithAntlr.DataModels;
using SimaiParserWithAntlr.DataModels.Notes;
using SimaiParserWithAntlr.NoteLayerParser;
using SimaiParserWithAntlr.StructureLayerParser;
using SimaiParserWithAntlr.StructureLayerParser.Structures;

namespace SimaiParserWithAntlr;

public class ChartParser
{
    public const int DEFAULT_RESOLUTION = 384;
    
    public string RawText { get; private set; }
    public List<ChartTiming> TimingList { get; } = new();
    public List<EachGroup> NoteList { get; } = new();
    public int Resolution { get; private set; } = DEFAULT_RESOLUTION;

    private ChartParser(string rawText)
    {
        RawText = rawText;
    }

    private void Analyze()
    {
        var structureParser = ChartStructureParser.GenerateFromText(RawText);
        // TODO
    }

    public static ChartParser GenerateFromText(string text)
    {
        var parser = new ChartParser(text);
        parser.Analyze();
        return parser;
    }
}
