using SimaiParserWithAntlr.DataModels;
using SimaiParserWithAntlr.NoteLayerParser.DataModels;

namespace SimaiParserWithAntlr.NoteLayerParser;

public class NoteParser
{
    private NoteBlockWalker _walker;
    public string RawText { get; private set; }
    public List<NoteGroup> NoteGroupList => _walker.NoteGroupList;
    public List<WarningInfo> WarningList => _walker.WarningList;
    public List<ErrorInfo> ErrorList => _walker.ErrorList;

    public NoteParser(NoteBlockWalker walker, string rawText)
    {
        _walker = walker;
        RawText = rawText;
    }

    public static NoteParser GenerateFromText(string text)
    {
        NoteParser result = new(NoteBlockWalker.GenerateFromText(text), text);
        return result;
    }
}
