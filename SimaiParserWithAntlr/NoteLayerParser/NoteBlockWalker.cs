using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using SimaiParserWithAntlr.DataModels;
using SimaiParserWithAntlr.I18nModule;
using SimaiParserWithAntlr.NoteLayerParser.Notes;

namespace SimaiParserWithAntlr.NoteLayerParser;

internal class NoteBlockWalker : NoteBlockParserBaseListener
{
    public TextPosition Offset { get; private set; }

    public List<NoteGroup> NoteGroupList { get; } = new();
    public List<WarningInfo> WarningList { get; } = new();
    public List<ErrorInfo> ErrorList { get; } = new();

    public override void EnterNote_group(NoteBlockParser.Note_groupContext context)
    {
        NoteGroup noteGroup = new();
        TextPositionRange groupRange = new(context.Start, context.Stop);

        if (context.each_tap() is { } eachTap)
        {
            // each tap group like `15`
            TextPositionRange range1 = new(eachTap.pos1, eachTap.pos1);
            TextPositionRange range2 = new(eachTap.pos2, eachTap.pos2);

            if (int.TryParse(eachTap.pos1.Text, out var btn1) && int.TryParse(eachTap.pos2.Text, out var btn2))
            {
                noteGroup.AddEach(NoteGroup.BuildEach()
                    .Add(new TapNote(range1, btn1))
                    .Add(new TapNote(range2, btn2))
                    .Build());
            }
            else
            {
                ThrowError(groupRange, I18nKeyEnum.FailToParseNumber, context.GetText(), "int");
            }
        }

        NoteGroupList.Add(noteGroup);

        base.EnterNote_group(context);
    }

    private void ThrowError(TextPositionRange range, I18nKeyEnum key, params object[] args)
    {
        ErrorList.Add(new ErrorInfo(range, key, args));
    }

    public static NoteBlockWalker GenerateFromText(string text)
    {
        // Constructing the syntax tree
        AntlrInputStream charStream = new(text);
        ITokenSource lexer = new NoteBlockLexer(charStream);
        ITokenStream tokens = new CommonTokenStream(lexer);
        NoteBlockParser parser = new(tokens)
        {
            BuildParseTree = true
        };

        IParseTree tree = parser.note_block();

        // Visiting using the listener
        NoteBlockWalker walker = new();
        ParseTreeWalker.Default.Walk(walker, tree);

        return walker;
    }
}
