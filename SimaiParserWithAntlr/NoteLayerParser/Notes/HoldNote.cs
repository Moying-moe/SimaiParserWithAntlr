using SimaiParserWithAntlr.DataModels;
using SimaiParserWithAntlr.NoteLayerParser.DataModels;

namespace SimaiParserWithAntlr.NoteLayerParser.Notes;

public class HoldNote : NoteBase
{
    public HoldNote(TextPositionRange range, int button, bool isBreak, bool isEx, NoteDuration duration) : base(range)
    {
        Button = button;
        IsBreak = isBreak;
        IsEx = isEx;
        Duration = duration;
    }

    public int Button { get; set; }
    public bool IsBreak { get; set; }
    public bool IsEx { get; set; }
    public NoteDuration Duration { get; set; }

    public override string GetRawString()
    {
        var result = $"{Button}";
        
        if (IsBreak)
        {
            result += Constants.BREAK_MARK;
        }

        if (IsEx)
        {
            result += Constants.EX_MARK;
        }

        result += Constants.HOLD_MARK;
        result += Duration.GetRawString();

        return result;
    }
}
