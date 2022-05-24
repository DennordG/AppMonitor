using Monitor.Configuration;

namespace Monitor.Experiments;

public class ExperimentInput
{
    public ExperimentConfig Config { get; }
    public string ExePath { get; }

    public ExperimentInput(ExperimentConfig config, string exePath)
    {
        Config = config;
        ExePath = exePath;
    }
}
