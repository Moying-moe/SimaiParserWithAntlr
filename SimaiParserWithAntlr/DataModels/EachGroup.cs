using SimaiParserWithAntlr.DataModels.Notes;

namespace SimaiParserWithAntlr.DataModels;

public class EachGroup
{
    public NoteTiming Timing { get; set; }
    public List<NoteBase> NoteList { get; } = new();

    public EachGroup(NoteTiming timing)
    {
        Timing = timing;
    }
}
