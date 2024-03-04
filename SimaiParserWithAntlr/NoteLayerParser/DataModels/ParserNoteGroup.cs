using SimaiParserWithAntlr.DataModels;
using SimaiParserWithAntlr.NoteLayerParser.Notes;

namespace SimaiParserWithAntlr.NoteLayerParser.DataModels;

/**
 * A group of notes at a single time point.
 * It may contain notes that are not actually at the same time (due to fake-each syntax).
 */
public class ParserNoteGroup
{
    // The outer layer represents a *fake-each note* group list,
    // and the inner layer represents *each group*.
    public List<List<ParserNoteBase>> NoteList { get; } = new();
    public string RawText { get; set; }
    public TextPositionRange Range { get; set; }

    public ParserNoteGroup(string rawText) : this(rawText, TextPositionRange.EMPTY)
    {
    }

    public ParserNoteGroup(string rawText, TextPositionRange range)
    {
        RawText = rawText;
        Range = range;
    }

    public void AddEach(List<ParserNoteBase> group)
    {
        NoteList.Add(group);
    }

    public string GetFormattedString()
    {
        var eachStringList = NoteList.Select(eachGroup => eachGroup.Select(note => note.GetFormattedString()).ToList())
            .Select(noteStringList => string.Join(Constants.EACH_SEPARATOR, noteStringList)).ToList();

        return string.Join(Constants.FAKE_EACH_SEPARATOR, eachStringList);
    }

    public static EachGroupBuilder BuildEach()
    {
        return new EachGroupBuilder();
    }

    public class EachGroupBuilder
    {
        private readonly List<ParserNoteBase> _noteList = new();

        public EachGroupBuilder Add(ParserNoteBase note)
        {
            _noteList.Add(note);
            return this;
        }

        public List<ParserNoteBase> Build()
        {
            return _noteList;
        }
    }
}
