namespace SimaiParserWithAntlr.Enums;

public enum SlideTypeEnum
{
    Straight, // -
    CircleLeft, // <
    CircleRight, // >
    CircleNear, // ^
    Fold, // v
    CurveCw, // q
    CurveCcw, // p
    TweakCurveCw, // qq
    TweakCurveCcw, // pp
    Thunder, // s
    ThunderMirror, // z
    Turn, // V
    Fan, // w
    Unknown = -1
}
