using SimaiParserWithAntlr.DataModels;

namespace SimaiParserWithAntlr.StructureLayerParser.Structures
{

    public class BpmElement : ElementBase
    {
        public BpmElement(string rawText, TextPositionRange range, double bpm) : base(rawText, range)
        {
            Bpm = bpm;
        }

        public double Bpm { get; set; }

        public override string GetFormattedString()
        {
            return $"({Bpm})";
        }
    }
}