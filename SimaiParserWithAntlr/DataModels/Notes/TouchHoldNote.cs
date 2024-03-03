using Newtonsoft.Json;
using SimaiParserWithAntlr.Enums;
using SimaiParserWithAntlr.NoteLayerParser.DataModels;

namespace SimaiParserWithAntlr.DataModels.Notes;

public class TouchHoldNote : NoteBase
{
    public TouchHoldNote(double bar, double beat, double time, AreaEnum area, bool isFirework, NoteDuration duration) :
        base(NoteTypeEnum.TouchHold, bar, beat, time)
    {
        Area = area;
        IsFirework = isFirework;
        Duration = duration;
    }

    [JsonConverter(typeof(AreaEnumJsonConverter))]
    public AreaEnum Area { get; set; }

    public bool IsFirework { get; set; }
    public NoteDuration Duration { get; set; }
}
