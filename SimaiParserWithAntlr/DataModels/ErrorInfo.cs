using SimaiParserWithAntlr.I18nModule;

namespace SimaiParserWithAntlr.DataModels;

public class ErrorInfo : BaseExceptionInfo
{
    public ErrorInfo(TextPosition start, TextPosition stop, I18nKeyEnum key) : base(start, stop, key)
    {
    }

    public ErrorInfo(TextPosition start, TextPosition stop, I18nKeyEnum key, params object[] args) : base(start, stop, key, args)
    {
    }
}
