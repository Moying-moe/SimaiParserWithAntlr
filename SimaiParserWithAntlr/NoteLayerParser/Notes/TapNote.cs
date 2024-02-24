using SimaiParserWithAntlr.DataModels;

namespace SimaiParserWithAntlr.NoteLayerParser.Notes;

public class TapNote : NoteBase
{
    public TapNote(TextPositionRange range, TextPosition stop, int button, bool isBreak, bool isEx) : base(range)
    {
        Button = button;
        IsBreak = isBreak;
        IsEx = isEx;
    }

    public TapNote(TextPositionRange range, int button) : base(range)
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
