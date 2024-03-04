using SimaiParserWithAntlr.DataModels.Notes;

namespace SimaiParserWithAntlr.DataModels;

public class EachGroup
{
    public EachGroup(NoteTiming timing)
    {
        Timing = timing;
    }

    public NoteTiming Timing { get; set; }
    public List<NoteBase> NoteList { get; } = new();
}
