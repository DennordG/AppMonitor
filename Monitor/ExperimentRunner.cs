using Monitor.Experiments;
using System.Diagnostics;

namespace Monitor;

public static class ExperimentRunner
{
    public static void Run(Experiment experiment)
    {
        for (var i = 0; i < experiment.Input.Config.RepeatCount; i++)
        {
            StartNewProcess(experiment);
        }
    }

    private static void StartNewProcess(Experiment experiment)
    {
        experiment.StartNewRun();

        using var process = new Process();

        process.StartInfo.FileName = experiment.Input.ExePath;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;

        var arguments = PreprocessArguments(experiment.Input.Config.Arguments!);

        process.StartInfo.Arguments = arguments;

        process.OutputDataReceived += experiment.Process_OutputDataReceived;
        process.ErrorDataReceived += experiment.Process_ErrorDataReceived;
        process.Start();

        EnsureProcessStarts(process);

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        do
        {
            AddExperimentLog(experiment, process);
        } while (!process.WaitForExit(1000));

        AddExperimentLog(experiment, process);

        process.Close();

        experiment.CompleteCurrentRun();
    }

    private static string PreprocessArguments(string arguments)
    {
        return string.Join(' ', arguments.Split(Environment.NewLine));
    }

    private static void EnsureProcessStarts(Process process)
    {
        while (true)
        {
            try
            {
                _ = process.ProcessName;
                break;
            }
            catch
            {
                Thread.Sleep(500);
                process.Kill();
                process.Start();
            }
        }
    }

    private static void AddExperimentLog(Experiment experiment, Process process)
    {
        try
        {
            var memoryUsage = process.WorkingSet64 / 1024 / 1024;
            var peakMemoryUsage = process.PeakWorkingSet64 / 1024 / 1024;
            var userProcessorTime = process.UserProcessorTime;
            var privilegedProcessorTime = process.PrivilegedProcessorTime;
            var totalProcessorTime = process.TotalProcessorTime;

            var currentRunResult = experiment.GetCurrentRunResult();

            currentRunResult.Logs.Add(new ExperimentLog
            {
                MemoryUsage = memoryUsage,
                PeakMemoryUsage = peakMemoryUsage,
                UserProcessorTime = userProcessorTime,
                PrivilegedProcessorTime = privilegedProcessorTime,
                TotalProcessorTime = totalProcessorTime
            });
        }
        catch
        { }

        process.Refresh();
    }
}
