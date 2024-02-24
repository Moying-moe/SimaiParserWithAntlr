

using SimaiParserWithAntlr.I18nModule;

namespace SimaiParserWithAntlr.DataModels;

public class BaseExceptionInfo
{
    public I18nKeyEnum Key { get; private set; }
    public string Message { get; private set; }
    public TextPosition Start { get; private set; }
    public TextPosition Stop { get; private set; }

    public BaseExceptionInfo(TextPosition start, TextPosition stop, I18nKeyEnum key)
    {
        Key = key;
        Message = I18n.Instance.Get(key);
        Start = start;
        Stop = stop;
    }

    public BaseExceptionInfo(TextPosition start, TextPosition stop, I18nKeyEnum key, params object[] args)
    {
        Key = key;
        Message = string.Format(I18n.Instance.Get(key), args);
        Start = start;
        Stop = stop;
    }
}
