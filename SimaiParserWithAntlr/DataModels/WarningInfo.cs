using SimaiParserWithAntlr.I18nModule;

namespace SimaiParserWithAntlr.DataModels;

public class WarningInfo : BaseExceptionInfo
{
    public WarningInfo(TextPositionRange range, I18nKeyEnum key) : base(range, key)
    {
    }

    public WarningInfo(TextPositionRange range, I18nKeyEnum key, params object[] args) : base(range, key, args)
    {
    }
    
    public new string GetFormattedInfo(string text)
    {
        string result = $"Warning at Line {Range.Start.Line} Column {Range.Stop.Column}:\n";
        result += Range.GetPositionedString(text, 5, 5, true, true) + "\n";
        result += $"{Key}: {Message}";
        return result;
    }
}
