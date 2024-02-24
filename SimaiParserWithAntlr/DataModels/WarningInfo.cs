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
}
