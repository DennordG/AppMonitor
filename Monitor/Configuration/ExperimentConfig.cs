namespace Monitor.Configuration;

public class ExperimentConfig
{
    public string? Arguments { get; init; }
    public int RepeatCount { get; init; } = 1;

    public bool IsValid() => Arguments != null && RepeatCount > 0;
}
