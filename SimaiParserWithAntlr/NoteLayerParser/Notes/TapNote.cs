namespace SimaiParserWithAntlr.NoteLayerParser.Notes;

public class TapNote : NoteBase
{
    public TapNote(TextPosition start, TextPosition stop, int button, bool isBreak, bool isEx) : base(start, stop)
    {
        Button = button;
        IsBreak = isBreak;
        IsEx = isEx;
    }

    public TapNote(TextPosition start, TextPosition stop, int button) : base(start, stop)
    {
        Button = button;
    }

    public int Button { get; set; }
    public bool IsBreak { get; set; }
    public bool IsEx { get; set; }

    public override string GetRawString()
    {
        throw new NotImplementedException();
    }
}
