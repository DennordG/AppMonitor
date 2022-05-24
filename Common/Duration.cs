using System.Diagnostics.CodeAnalysis;

namespace Common;

public sealed class Duration : IDisposable
{
    private readonly string _group;

    private Duration(string group)
    {
        _group = group;
        Console.WriteLine("{0}: Start", _group);
    }

    void IDisposable.Dispose()
    {
        Console.WriteLine("{0}: Stop", _group);
    }

    public static Duration New(string group)
    {
        return new Duration(group);
    }

    public static bool IsValid(string input, [NotNullWhen(true)] out DurationInfo? info)
    {
        info = null;

        var split = input.Split(':', StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).Where(i => i.Length > 0).ToList();
        if (split.Count == 2)
        {
            if (split[1] == "Start")
            {
                info = new DurationInfo(split[0], DurationType.Start);
            }
            else if (split[1] == "Stop")
            {
                info = new DurationInfo(split[0], DurationType.Stop);
            }
        }

        return info != null;
    }
}
