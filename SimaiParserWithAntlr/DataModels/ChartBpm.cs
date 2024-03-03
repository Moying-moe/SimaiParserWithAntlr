namespace SimaiParserWithAntlr.DataModels;

public class ChartBpm
{
    public NoteTiming Timing { get; set; }
    public double Bpm { get; set; }

    public ChartBpm(NoteTiming timing, double bpm)
    {
        Timing = timing;
        Bpm = bpm;
    }
}
