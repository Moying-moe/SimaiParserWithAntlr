using SimaiParserWithAntlr.DataModels;

namespace SimaiParserWithAntlr.StructureLayerParser.Structures;

public abstract class ElementBase
{
    protected ElementBase(string rawText, TextPositionRange range)
    {
        RawText = rawText;
        Range = range;
    }

    public string RawText { get; set; }
    public TextPositionRange Range { get; set; }

    public abstract string GetFormattedString();
}
