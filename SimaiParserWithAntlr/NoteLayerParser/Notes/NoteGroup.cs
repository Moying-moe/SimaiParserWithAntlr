﻿namespace SimaiParserWithAntlr.NoteLayerParser.Notes;

/**
 * A group of notes at a single time point.
 * It may contain notes that are not actually at the same time (due to fake-each syntax).
 */
public class NoteGroup
{
    // The outer layer represents a *fake-each note* group list,
    // and the inner layer represents *each group*.
    public List<List<NoteBase>> NoteList { get; } = new();

    public void AddEach(List<NoteBase> group)
    {
        NoteList.Add(group);
    }

    public string GetRawString()
    {
        var eachStringList = NoteList.Select(eachGroup => eachGroup.Select(note => note.GetRawString()).ToList())
            .Select(noteStringList => string.Join(Constants.EACH_SEPARATOR, noteStringList)).ToList();

        return string.Join(Constants.FAKE_EACH_SEPARATOR, eachStringList);
    }

    public static EachGroupBuilder BuildEach()
    {
        return new EachGroupBuilder();
    }

    public class EachGroupBuilder
    {
        private readonly List<NoteBase> _noteList = new();

        public EachGroupBuilder Add(NoteBase note)
        {
            _noteList.Add(note);
            return this;
        }

        public List<NoteBase> Build()
        {
            return _noteList;
        }
    }
}
