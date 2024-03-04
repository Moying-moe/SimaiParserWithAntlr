using SimaiParserWithAntlr.DataModels;

namespace SimaiParserWithAntlr.StructureLayerParser.Structures;

public class ResolutionElement : ElementBase
{
    public ResolutionElement(string rawText, TextPositionRange range, int resolution) : base(rawText, range)
    {
        Resolution = resolution;
    }

    public int Resolution { get; set; }

    public override string GetFormattedString()
    {
        return $"{{{Resolution}}}";
    }
}
