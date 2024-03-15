using SimaiParserWithAntlr.DataModels.Notes;
using System.Collections.Generic;

namespace SimaiParserWithAntlr.DataModels
{

    public class EachGroup
    {
        public EachGroup(NoteTiming timing)
        {
            Timing = timing;
        }

        public NoteTiming Timing { get; set; }
        public List<NoteBase> NoteList { get; } = new();
    }
}