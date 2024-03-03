namespace SimaiParserWithAntlr.Enums;

public static class ButtonEnumExt
{
    public static bool TryParse(string value, out ButtonEnum result)
    {
        return Enum.TryParse($"Button{value}", out result);
    }
    
    public static bool TryParse(int value, out ButtonEnum result)
    {
        return Enum.TryParse($"Button{value}", out result);
    }

    public static string ToFormattedString(ButtonEnum btn)
    {
        switch (btn)
        {
            case ButtonEnum.Button1:
                return "1";
            case ButtonEnum.Button2:
                return "2";
            case ButtonEnum.Button3:
                return "3";
            case ButtonEnum.Button4:
                return "4";
            case ButtonEnum.Button5:
                return "5";
            case ButtonEnum.Button6:
                return "6";
            case ButtonEnum.Button7:
                return "7";
            case ButtonEnum.Button8:
                return "8";
            case ButtonEnum.Unknown:
            default:
                return "";
        }
    }

    public static int ToButtonNumber(ButtonEnum btn)
    {
        switch (btn)
        {
            case ButtonEnum.Button1:
                return 1;
            case ButtonEnum.Button2:
                return 2;
            case ButtonEnum.Button3:
                return 3;
            case ButtonEnum.Button4:
                return 4;
            case ButtonEnum.Button5:
                return 5;
            case ButtonEnum.Button6:
                return 6;
            case ButtonEnum.Button7:
                return 7;
            case ButtonEnum.Button8:
                return 8;
            case ButtonEnum.Unknown:
            default:
                return -1;
        }
    }
}
