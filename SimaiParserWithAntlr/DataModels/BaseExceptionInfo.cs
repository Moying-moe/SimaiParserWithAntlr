using Antlr4.Runtime.Tree.Pattern;
using SimaiParserWithAntlr.I18nModule;

namespace SimaiParserWithAntlr.DataModels;

public class BaseExceptionInfo
{
    public I18nKeyEnum Key { get; private set; }
    public string Message { get; private set; }
    public TextPositionRange Range { get; private set; }

    public BaseExceptionInfo(TextPositionRange range, I18nKeyEnum key)
    {
        Key = key;
        Message = I18n.Instance.Get(key);
        Range = range;
    }

    public BaseExceptionInfo(TextPositionRange range, I18nKeyEnum key, params object[] args)
    {
        Key = key;
        Message = string.Format(I18n.Instance.Get(key), args);
        Range = range;
    }

    public string GetFormattedInfo(string text)
    {
        string result = $"Exception at Line {Range.Start.Line} Column {Range.Stop.Column}:";
        result += Range.GetPositionedString(text, 5, 5, true, true) + "\n";
        result += $"{Key}: {Message}";
        return result;
    }
}
