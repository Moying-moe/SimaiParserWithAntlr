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
}
