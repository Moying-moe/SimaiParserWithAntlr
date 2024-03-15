using SimaiParserWithAntlr.DataModels;

namespace SimaiParserWithAntlr.StructureLayerParser.Structures
{

    public class HiSpeedElement : ElementBase
    {
        public HiSpeedElement(string rawText, TextPositionRange range, double hiSpeed) : base(rawText, range)
        {
            HiSpeed = hiSpeed;
        }

        public double HiSpeed { get; set; }

        public override string GetFormattedString()
        {
            return $"<HS*{HiSpeed}>";
        }
    }
}