using SimaiParserWithAntlr.I18nModule;

namespace SimaiParserWithAntlr.DataModels;

public class ErrorInfo : BaseExceptionInfo
{
    public ErrorInfo(TextPositionRange range, I18nKeyEnum key) : base(range, key)
    {
    }

    public ErrorInfo(TextPositionRange range, I18nKeyEnum key, params object[] args) : base(range, key, args)
    {
    }
    
    public new string GetFormattedInfo(string text)
    {
        string result = $"Error at Line {Range.Start.Line} Column {Range.Stop.Column}:";
        result += Range.GetPositionedString(text, 5, 5, true, true) + "\n";
        result += $"{Key}: {Message}";
        return result;
    }
}
