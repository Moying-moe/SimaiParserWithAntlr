using SimaiParserWithAntlr.DataModels;

namespace SimaiParserWithAntlr.StructureLayerParser.Structures;

public class CommentElement : ElementBase
{
    public CommentElement(string rawText, TextPositionRange range, string content) : base(rawText, range)
    {
        Content = content;
    }

    public string Content { get; set; }

    public override string GetFormattedString()
    {
        if (Content.StartsWith(" "))
        {
            return Constants.COMMENT_SYMBOL + Content;
        }

        return Constants.COMMENT_SYMBOL + " " + Content;
    }
}
