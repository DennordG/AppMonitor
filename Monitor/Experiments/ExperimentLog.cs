namespace Monitor.Experiments;

public class ExperimentLog
{
    public long MemoryUsage { get; init; }
    public long PeakMemoryUsage { get; init; }
    public TimeSpan UserProcessorTime { get; init; }
    public TimeSpan PrivilegedProcessorTime { get; init; }
    public TimeSpan TotalProcessorTime { get; init; }

    public void Print(TextWriter textWriter)
    {
        textWriter.WriteLine($"Physical memory usage (MB) : {MemoryUsage} (PEAK: {PeakMemoryUsage})");
        textWriter.WriteLine($"User processor time        : {UserProcessorTime}");
        textWriter.WriteLine($"Privileged processor time  : {PrivilegedProcessorTime}");
        textWriter.WriteLine($"Total processor time       : {TotalProcessorTime}");
    }
}
