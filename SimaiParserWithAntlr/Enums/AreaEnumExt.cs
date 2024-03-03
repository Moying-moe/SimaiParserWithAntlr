namespace SimaiParserWithAntlr.Enums;

public static class AreaEnumExt
{
    public static bool TryParse(string value, out AreaEnum result)
    {
        return Enum.TryParse(value, out result);
    }

    public static string ToFormattedString(AreaEnum area)
    {
        if (area == AreaEnum.Unknown)
        {
            return "";
        }
        return area.ToString();
    }
}
