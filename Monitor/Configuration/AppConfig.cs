namespace Monitor.Configuration;

public class AppConfig
{
    public List<ExperimentConfig>? Experiments { get; init; }
    public string? ExePath { get; init; }

    public bool IsValid() => !string.IsNullOrWhiteSpace(ExePath) && Experiments?.All(e => e.IsValid()) == true;
}
