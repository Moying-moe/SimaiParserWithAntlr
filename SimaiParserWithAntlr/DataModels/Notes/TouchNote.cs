using Newtonsoft.Json;
using SimaiParserWithAntlr.Enums;

namespace SimaiParserWithAntlr.DataModels.Notes;

public class TouchNote : NoteBase
{
    public TouchNote(double bar, double beat, double time, AreaEnum area, bool isFirework) : base(NoteTypeEnum.Touch,
        bar, beat, time)
    {
        Area = area;
        IsFirework = isFirework;
    }

    [JsonConverter(typeof(AreaEnumJsonConverter))]
    public AreaEnum Area { get; set; }

    public bool IsFirework { get; set; }
}
