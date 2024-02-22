using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using SimaiParserWithAntlr.NoteLayerParser.Notes;

namespace SimaiParserWithAntlr.NoteLayerParser;

internal class NoteBlockWalker : NoteBlockParserBaseListener
{
    public List<NoteGroup> NoteGroupList { get; } = new();

    public override void EnterNote_group(NoteBlockParser.Note_groupContext context)
    {
        NoteGroup noteGroup = new();

        if (context.each_tap() is { } eachTap)
        {
            // each tap group like `15`
            if (int.TryParse(eachTap.pos1.Text, out var btn1) && int.TryParse(eachTap.pos2.Text, out var btn2))
            {
                TextPosition pos1 = new(eachTap.pos1.Line, eachTap.pos1.Column);
                TextPosition pos2 = new(eachTap.pos2.Line, eachTap.pos2.Column);

                noteGroup.AddEach(NoteGroup.BuildEach()
                    .Add(new TapNote(pos1, pos1, btn1))
                    .Add(new TapNote(pos2, pos2, btn2))
                    .Build());
            }
            // TODO: throw warning or error
        }

        NoteGroupList.Add(noteGroup);

        base.EnterNote_group(context);
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
