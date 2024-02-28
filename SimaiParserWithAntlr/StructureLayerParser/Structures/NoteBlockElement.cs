using SimaiParserWithAntlr.DataModels;

namespace SimaiParserWithAntlr.StructureLayerParser.Structures;

public class NoteBlockElement : ElementBase
{
    public double? Bpm { get; set; }
    public int? Resolution { get; set; }
    public double HiSpeed { get; set; }
    
    public NoteBlockElement(string rawText, TextPositionRange range) : base(rawText, range)
    {
    }

    public override string GetFormattedString()
    {
        return RawText;
    }
}
