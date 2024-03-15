namespace SimaiParserWithAntlr.Utils
{

    public static class ButtonHelper
    {
        public const int UNKNOWN_BUTTON = -1;

        public static bool IsButtonNumberValid(int buttonNumber)
        {
            return buttonNumber is >= 1 and <= 8;
        }

        public static bool TryParse(string value, out int result)
        {
            return int.TryParse(value, out result) && IsButtonNumberValid(result);
        }
    }
}