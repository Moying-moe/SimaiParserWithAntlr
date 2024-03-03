using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SimaiParserWithAntlr.Enums;

namespace SimaiParserWithAntlr.DataModels.Notes;

public abstract class NoteBase
{
    protected NoteBase(NoteTypeEnum type, double bar, double beat, double time)
    {
        Type = type;
        Bar = bar;
        Beat = beat;
        Time = time;
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public NoteTypeEnum Type { get; set; }

    public double Bar { get; set; }
    public double Beat { get; set; }
    public double Time { get; set; }
}
