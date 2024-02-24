using System.Globalization;
using System.Text.RegularExpressions;
using IniParser;

namespace SimaiParserWithAntlr.I18nModule;

// ReSharper disable once InconsistentNaming
public class I18n
{
    public static readonly CultureInfo DEFAULT_CULTURE_INFO = CultureInfo.GetCultureInfo("en_US");

    // ReSharper disable once InconsistentNaming
    private readonly Dictionary<CultureInfo, Dictionary<string, string>> _i18nMap = new();

    private I18n()
    {
        // ReSharper disable once InconsistentNaming
        var i18nFiles = Directory.GetFiles("i18n", "i18n_*.ini");

        var parser = new FileIniDataParser();
        foreach (var fn in i18nFiles)
        {
            var cultureName = new Regex(@"i18n_(.+?)\.ini").Match(fn).Groups[1].Value;
            cultureName = cultureName.Replace("_", "-");
            CultureInfo culture;
            try
            {
                culture = CultureInfo.GetCultureInfo(cultureName);
            }
            catch (CultureNotFoundException)
            {
                culture = new CultureInfo(cultureName);
            }

            var data = parser.ReadFile(fn);

            _i18nMap.Add(culture, data.Global.ToDictionary(e => e.KeyName, e => e.Value));
        }
    }

    public static I18n Instance { get; } = new();

    public string Get(I18nKeyEnum key)
    {
        return Get(key, key.ToString());
    }

    public string Get(I18nKeyEnum key, string defaultValue)
    {
        CultureInfo curCulture;
        try
        {
            curCulture = CultureInfo.CurrentCulture;
        }
        catch (ArgumentNullException)
        {
            curCulture = DEFAULT_CULTURE_INFO;
        }

        if (_i18nMap.TryGetValue(curCulture, out var map) && map.TryGetValue(key.ToString(), out var result))
        {
            return result;
        }

        return defaultValue;
    }
}
