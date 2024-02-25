namespace SimaiParserWithAntlr.NoteLayerParser.DataModels;

public enum DurationTypeEnum
{
    Empty,
    Fraction,
    BpmFraction,
    Time,
    BpmTime,
    DelayFraction,
    DelayTime,
    DelayBpmFraction,
    Unknown = -1
}
