using SimaiParserWithAntlr.DataModels;

namespace SimaiParserWithAntlr.StructureLayerParser.Structures;

public class NoteBlockElement : ElementBase
{
    public NoteBlockElement(string rawText, TextPositionRange range) : base(rawText, range)
    {
    }

    public override string GetFormattedString()
    {
        return RawText;
    }
}
