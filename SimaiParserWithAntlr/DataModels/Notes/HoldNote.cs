using Newtonsoft.Json;
using SimaiParserWithAntlr.Enums;
using SimaiParserWithAntlr.NoteLayerParser.DataModels;

namespace SimaiParserWithAntlr.DataModels.Notes;

public class HoldNote : NoteBase
{
    public HoldNote(double bar, double beat, double time, ButtonEnum button, bool isBreak, bool isEx,
        NoteDuration duration) : base(NoteTypeEnum.Hold, bar, beat, time)
    {
        Button = button;
        IsBreak = isBreak;
        IsEx = isEx;
        Duration = duration;
    }

    [JsonConverter(typeof(ButtonEnumJsonConverter))]
    public ButtonEnum Button { get; set; }

    public bool IsBreak { get; set; }
    public bool IsEx { get; set; }
    public NoteDuration Duration { get; set; }
}
