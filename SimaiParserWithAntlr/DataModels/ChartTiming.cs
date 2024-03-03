namespace SimaiParserWithAntlr.DataModels;

public class ChartTiming
{
    public int Beat { get; set; }
    public int Bar { get; set; }

    public ChartTiming(int beat, int bar)
    {
        Beat = beat;
        Bar = bar;
    }
}
