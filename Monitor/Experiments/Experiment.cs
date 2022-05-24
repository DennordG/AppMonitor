using Common;
using System.Diagnostics;

namespace Monitor.Experiments;

public class Experiment
{
    public ExperimentInput Input { get; }
    public List<ExperimentOutput> Results { get; }

    public Experiment(ExperimentInput input)
    {
        Input = input;
        Results = new List<ExperimentOutput>();
    }

    public void StartNewRun()
    {
        Results.Add(new ExperimentOutput());
    }

    public ExperimentOutput GetCurrentRunResult()
    {
        return Results.LastOrDefault(r => !r.IsComplete) ?? throw new ApplicationException("No new run has been started");
    }

    public void CompleteCurrentRun()
    {
        GetCurrentRunResult().IsComplete = true;
    }

    public void PrintResults(TextWriter textWriter)
    {
        textWriter.WriteLine($"Arguments: {Input.Config.Arguments}");
        textWriter.WriteLine($"Repeated: {Input.Config.RepeatCount} times");

        foreach (var (index, result) in Results.Select((e, i) => (i + 1, e)))
        {
            textWriter.WriteLine();
            textWriter.WriteLine($"Run #{index}");
            textWriter.WriteLine("###############################################");

            textWriter.WriteLine("Durations:");
            textWriter.WriteLine("-----------------------------------------------");
            PrintDurations(textWriter, result.Durations);
        }

        textWriter.WriteLine();
        textWriter.WriteLine("Minimum durations:");
        textWriter.WriteLine("-----------------------------------------------");
        var minimumDurations = Results.SelectMany(r => r.Durations).GroupBy(g => g.Key, StringComparer.OrdinalIgnoreCase).ToDictionary(g => g.Key, g => g.Min(i => i.Value.Elapsed.TotalMilliseconds));
        PrintDurations(textWriter, minimumDurations);

        textWriter.WriteLine();
        textWriter.WriteLine("Average durations:");
        textWriter.WriteLine("-----------------------------------------------");
        var averageDurations = Results.SelectMany(r => r.Durations).GroupBy(g => g.Key, StringComparer.OrdinalIgnoreCase).ToDictionary(g => g.Key, g => g.Average(i => i.Value.Elapsed.TotalMilliseconds));
        PrintDurations(textWriter, averageDurations);

        textWriter.WriteLine();
        textWriter.WriteLine("Maximum durations:");
        textWriter.WriteLine("-----------------------------------------------");
        var maximumDurations = Results.SelectMany(r => r.Durations).GroupBy(g => g.Key, StringComparer.OrdinalIgnoreCase).ToDictionary(g => g.Key, g => g.Max(i => i.Value.Elapsed.TotalMilliseconds));
        PrintDurations(textWriter, maximumDurations);
    }

    public static void PrintDurations(TextWriter textWriter, IDictionary<string, Stopwatch> durations)
    {
        foreach (var (group, stopwatch) in durations)
        {
            textWriter.WriteLine("{0}: {1}ms", group, stopwatch.Elapsed.TotalMilliseconds);
        }
    }

    public static void PrintDurations(TextWriter textWriter, IDictionary<string, double> durations)
    {
        foreach (var (group, duration) in durations)
        {
            textWriter.WriteLine("{0}: {1}ms", group, duration);
        }
    }

    public void PrintLogs(TextWriter textWriter)
    {
        foreach (var (index, result) in Results.Select((e, i) => (i + 1, e)))
        {
            textWriter.WriteLine($"Run #{index}");
            textWriter.WriteLine("###############################################");

            textWriter.WriteLine("Logs:");
            textWriter.WriteLine("-----------------------------------------------");

            foreach (var log in result.Logs)
            {
                log.Print(textWriter);
                textWriter.WriteLine();
            }
        }
    }

    public void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data == null)
        {
            return;
        }

        var currentRunResult = GetCurrentRunResult();

        if (Duration.IsValid(e.Data, out var info))
        {
            if (info.Type == DurationType.Start)
            {
                currentRunResult.Durations[info.Group] = Stopwatch.StartNew();
            }
            else
            {
                currentRunResult.Durations[info.Group].Stop();
            }
        }
        else
        {
            currentRunResult.ProgramOutputs.AppendLine(e.Data);
        }
    }

    public void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data == null)
        {
            return;
        }

        var currentRunResult = GetCurrentRunResult();

        currentRunResult.ProgramErrors.AppendLine(e.Data);
    }
}
