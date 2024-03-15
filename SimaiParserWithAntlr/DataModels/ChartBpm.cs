namespace SimaiParserWithAntlr.DataModels
{

    public class ChartBpm
    {
        public ChartBpm(NoteTiming timing, double bpm)
        {
            Timing = timing;
            Bpm = bpm;
        }

        public NoteTiming Timing { get; set; }
        public double Bpm { get; set; }
    }
}