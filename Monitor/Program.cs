using Monitor.Configuration;
using Monitor.Experiments;
using Newtonsoft.Json;

namespace Monitor;

public static class Program
{
    public static void Main()
    {
        var configJson = File.ReadAllText("config.json");
        var config = JsonConvert.DeserializeObject<AppConfig>(configJson);

        if (config?.IsValid() != true)
        {
            throw new ApplicationException("Invalid configuration.");
        }

        foreach (var (expIndex, experimentConfig) in config.Experiments!.Select((e, i) => (i + 1, e)))
        {
            var experiment = new Experiment(new ExperimentInput(experimentConfig, config.ExePath!));

            Console.WriteLine($"Running experiment #{expIndex}...");

            ExperimentRunner.Run(experiment);

            Console.WriteLine($"Finished running experiment #{expIndex}");

            Console.WriteLine($"Writing results for experiment #{expIndex}...");

            Directory.CreateDirectory("Results");

            using (var fileStream = File.Create($"Results/experiment_{expIndex}.txt"))
            {
                using var textWriter = new StreamWriter(fileStream);

                experiment.PrintResults(textWriter);
            }

            using (var fileStream = File.Create($"Results/experiment_{expIndex}_logs.txt"))
            {
                using var textWriter = new StreamWriter(fileStream);

                experiment.PrintLogs(textWriter);
            }

            var json = JsonConvert.SerializeObject(experiment, Formatting.Indented);
            File.WriteAllText($"Results/experiment_{expIndex}.json", json);

            Console.WriteLine($"Finished writing results for experiment #{expIndex}");
            Console.WriteLine();
        }

        Console.WriteLine("\n\nPress any key to exit.");
        Console.ReadLine();
    }
}
