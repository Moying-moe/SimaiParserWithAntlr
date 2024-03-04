using SimaiParserWithAntlr.DataModels;

namespace SimaiParserWithAntlr.NoteLayerParser.Notes;

public abstract class ParserNoteBase
{
    protected ParserNoteBase(string rawText, TextPositionRange range)
    {
        RawText = rawText;
        Range = range;
    }

    public string RawText { get; set; }

    // Indicates the position of this note in the text.
    public TextPositionRange Range { get; set; }

    public abstract string GetFormattedString();
}
