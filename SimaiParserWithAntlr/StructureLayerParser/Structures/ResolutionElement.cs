using SimaiParserWithAntlr.DataModels;

namespace SimaiParserWithAntlr.StructureLayerParser.Structures;

public class ResolutionElement : ElementBase
{
    public int Resolution { get; set; }

    public ResolutionElement(string rawText, TextPositionRange range, int resolution) : base(rawText, range)
    {
        Resolution = resolution;
    }

    public override string GetFormattedString()
    {
        return $"{{{Resolution}}}";
    }
}
