namespace SimaiParserWithAntlr.NoteLayerParser.Notes;

public abstract class NoteBase
{
    protected NoteBase(TextPosition start, TextPosition stop)
    {
        Start = start;
        Stop = stop;
    }

    // Indicates the position of this note in the text.
    public TextPosition Start { get; set; }
    public TextPosition Stop { get; set; }

    public abstract string GetRawString();
}
