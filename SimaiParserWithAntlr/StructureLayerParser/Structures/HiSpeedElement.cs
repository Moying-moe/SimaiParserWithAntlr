using SimaiParserWithAntlr.DataModels;

namespace SimaiParserWithAntlr.StructureLayerParser.Structures;

public class HiSpeedElement : ElementBase
{
    public double HiSpeed { get; set; }

    public HiSpeedElement(string rawText, TextPositionRange range, double hiSpeed) : base(rawText, range)
    {
        HiSpeed = hiSpeed;
    }

    public override string GetFormattedString()
    {
        return $"<HS*{HiSpeed}>";
    }
}
