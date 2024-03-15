using SimaiParserWithAntlr.Enums;
using System;

namespace SimaiParserWithAntlr.Utils
{

    public class AreaHelper
    {
        public static bool TryParse(string value, out AreaCodeEnum areaCode, out int areaNumber)
        {
            if (value == "C")
            {
                areaCode = AreaCodeEnum.C;
                areaNumber = 1;
                return true;
            }

            if (value.Length != 2 || !Enum.TryParse(value[..1], out areaCode) ||
                !int.TryParse(value[1..], out areaNumber) || !ButtonHelper.IsButtonNumberValid(areaNumber))
            {
                areaCode = AreaCodeEnum.Unknown;
                areaNumber = ButtonHelper.UNKNOWN_BUTTON;
                return false;
            }

            return true;
        }
    }
}