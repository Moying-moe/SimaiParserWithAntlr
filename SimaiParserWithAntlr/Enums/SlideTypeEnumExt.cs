using SimaiParserWithAntlr.Utils;

namespace SimaiParserWithAntlr.Enums;

public static class SlideTypeEnumExt
{
    private static readonly TwoWayDictionary<string, SlideTypeEnum> SLIDE_TYPE_MAP = new()
    {
        { "-", SlideTypeEnum.Straight },
        { "<", SlideTypeEnum.CircleLeft },
        { ">", SlideTypeEnum.CircleRight },
        { "^", SlideTypeEnum.CircleNear },
        { "v", SlideTypeEnum.Fold },
        { "p", SlideTypeEnum.CurveCcw },
        { "q", SlideTypeEnum.CurveCw },
        { "pp", SlideTypeEnum.TweakCurveCcw },
        { "qq", SlideTypeEnum.TweakCurveCw },
        { "s", SlideTypeEnum.Thunder },
        { "z", SlideTypeEnum.ThunderMirror },
        { "V", SlideTypeEnum.Turn },
        { "w", SlideTypeEnum.Fan },
        { "", SlideTypeEnum.Unknown }
    };

    public static bool TryParse(string value, out SlideTypeEnum result)
    {
        if (SLIDE_TYPE_MAP.TryGetValue(value, out result))
        {
            return true;
        }

        result = SlideTypeEnum.Unknown;
        return false;
    }

    public static string ToFormattedString(SlideTypeEnum slideType)
    {
        return SLIDE_TYPE_MAP.GetKeyOrDefault(slideType, "");
    }
}
