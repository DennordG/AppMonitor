using System.Diagnostics;
using System.Text;

namespace Monitor.Experiments;

public class ExperimentOutput
{
    public bool IsComplete { get; set; }
    public IDictionary<string, Stopwatch> Durations { get; } = new Dictionary<string, Stopwatch>();
    public IList<ExperimentLog> Logs { get; } = new List<ExperimentLog>();
    public StringBuilder ProgramOutputs { get; } = new StringBuilder();
    public StringBuilder ProgramErrors { get; } = new StringBuilder();
}
