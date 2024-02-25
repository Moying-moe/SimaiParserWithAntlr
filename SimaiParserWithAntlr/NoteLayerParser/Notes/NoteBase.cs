using SimaiParserWithAntlr.DataModels;

namespace SimaiParserWithAntlr.NoteLayerParser.Notes;

public abstract class NoteBase
{
    protected NoteBase(TextPositionRange range)
    {
        Range = range;
    }

    // Indicates the position of this note in the text.
    public TextPositionRange Range { get; set; }

    public abstract string GetFormattedString();
}
