namespace SimaiParserWithAntlr.DataModels;

public class NoteTiming : IComparable<NoteTiming>, IEquatable<NoteTiming>
{
    public static readonly NoteTiming DEFAULT_TIMING = new(1, 0, 0);

    public NoteTiming(int bar, int beat, double time)
    {
        Bar = bar;
        Beat = beat;
        Time = time;
    }

    public NoteTiming(int bar, int beat)
    {
        Bar = bar;
        Beat = beat;
    }

    public int Bar { get; set; }
    public int Beat { get; set; }
    public double Time { get; set; }

    public int CompareTo(NoteTiming? other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (ReferenceEquals(null, other))
        {
            return 1;
        }

        var barComparison = Bar.CompareTo(other.Bar);
        if (barComparison != 0)
        {
            return barComparison;
        }

        return Beat.CompareTo(other.Beat);
    }

    public bool Equals(NoteTiming? other)
    {
        return CompareTo(other) == 0;
    }

    private void Flat(int resolution)
    {
        if (Beat >= resolution)
        {
            Bar += Beat / resolution;
            Beat %= resolution;
        }

        if (Beat < 0)
        {
            Bar -= -Beat / resolution;
            Beat = Beat % resolution + resolution;
        }
    }

    public static NoteTiming FromBeat(int beat, int resolution)
    {
        var result = new NoteTiming(0, beat);
        result.Flat(resolution);
        return result;
    }

    public void Add(NoteTiming other, int resolution)
    {
        Bar += other.Bar;
        Beat += other.Beat;
        Time += other.Time;

        Flat(resolution);
    }

    public void Subtract(NoteTiming other, int resolution)
    {
        Bar -= other.Bar;
        Beat -= other.Beat;
        Time -= other.Time;

        Flat(resolution);
    }

    public NoteTiming Clone()
    {
        return new NoteTiming(Bar, Beat, Time);
    }

    public static NoteTiming Add(NoteTiming a, NoteTiming b, int resolution)
    {
        var result = a.Clone();
        result.Add(b, resolution);

        return result;
    }

    public static NoteTiming Subtract(NoteTiming a, NoteTiming b, int resolution)
    {
        var result = a.Clone();
        result.Subtract(b, resolution);

        return result;
    }

    public static bool operator >(NoteTiming a, NoteTiming b)
    {
        return a.CompareTo(b) > 0;
    }

    public static bool operator <(NoteTiming a, NoteTiming b)
    {
        return a.CompareTo(b) < 0;
    }

    public static bool operator >=(NoteTiming a, NoteTiming b)
    {
        return a.CompareTo(b) >= 0;
    }

    public static bool operator <=(NoteTiming a, NoteTiming b)
    {
        return a.CompareTo(b) <= 0;
    }

    public override string ToString()
    {
        return $"{nameof(Bar)}: {Bar}, {nameof(Beat)}: {Beat}, {nameof(Time)}: {Time}";
    }

    public static bool operator ==(NoteTiming? a, NoteTiming? b)
    {
        if (ReferenceEquals(null, a))
        {
            return ReferenceEquals(null, b);
        }

        return a.CompareTo(b) == 0;
    }

    public static bool operator !=(NoteTiming? a, NoteTiming? b)
    {
        return !(a == b);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((NoteTiming)obj);
    }

    public override int GetHashCode()
    {
        return (Bar, Beat).GetHashCode();
    }
}
