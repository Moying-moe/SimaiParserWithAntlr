using SimaiParserWithAntlr.DataModels;

namespace SimaiParserWithAntlr.StructureLayerParser.Structures;

public abstract class ElementBase
{
    public string RawText { get; set; }
    public TextPositionRange Range { get; set; }

    protected ElementBase(string rawText, TextPositionRange range)
    {
        RawText = rawText;
        Range = range;
    }

    public abstract string GetFormattedString();
}
