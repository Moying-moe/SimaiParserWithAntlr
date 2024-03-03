using Newtonsoft.Json;
using SimaiParserWithAntlr.Enums;

namespace SimaiParserWithAntlr.DataModels.Notes;

public class TapNote : NoteBase
{
    public TapNote(double bar, double beat, double time, ButtonEnum button, bool isBreak, bool isEx) : base(
        NoteTypeEnum.Tap, bar, beat, time)
    {
        Button = button;
        IsBreak = isBreak;
        IsEx = isEx;
    }

    [JsonConverter(typeof(ButtonEnumJsonConverter))]
    public ButtonEnum Button { get; set; }

    public bool IsBreak { get; set; }
    public bool IsEx { get; set; }
}
