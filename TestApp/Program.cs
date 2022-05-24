using Common;
using TestApp;

using (Duration.New("Application"))
{
    if (args.Length == 0)
    {
        throw new ArgumentException("No arguments specified", nameof(args));
    }

    Input? input = null;

    using (Duration.New("InputSetup"))
    {
        var n = Convert.ToInt32(args[0]);

        var numbers = Enumerable.Range(0, n).Select(Convert.ToInt64).ToArray();

        var threadsCount = args.Length > 1 ? Convert.ToInt32(args[1]) : 1;
        input = new Input(numbers, threadsCount);
    }

    using (Duration.New("ValidateInput"))
    {
        ThrowIfNotValidInput(input);
    }

    using (Duration.New("LongTask"))
    {
        await Task.Delay(2500);
    }

    var list = new List<int>();
    using (Duration.New("AllocateMemoryInList"))
    {
        for (var i = 0; i < 4; i++)
        {
            list.AddRange(Enumerable.Range(0, 10000000));
        }

        Console.WriteLine("List allocated");
    }

    using (Duration.New("SortAllocatedList"))
    {
        list.Sort();
        Console.WriteLine("List sorted");
    }

    using (Duration.New("ComputeSum"))
    {
        var sum = await ComputeSumAsync(input);
        Console.WriteLine("Sum: {0}", sum);
    }

    using (Duration.New("GarbageCollect"))
    {
        GC.Collect();
        await Task.Delay(1000);
        Console.WriteLine("Garbage collected");
    }
}

static async Task<long> ComputeSumAsync(Input input)
{
    if (input.ThreadsCount == 1)
    {
        return input.Numbers.Sum();
    }

    var tasks = new List<Task<long>>();

    foreach (var range in Helper.GetRanges(input.Numbers.Count, input.ThreadsCount))
    {
        tasks.Add(Task.Factory.StartNew(obj =>
        {
            var (start, end) = ((int start, int end))obj!;

            return Sum(input.Numbers, start, end);
        }, range));
    }

    var result = await Task.WhenAll(tasks);

    return result.Sum();
}

static long Sum(IList<long> numbers, int start, int end)
{
    var sum = 0L;

    for (int i = start; i < end; i++)
    {
        sum += numbers[i];
    }

    return sum;
}

static void ThrowIfNotValidInput(Input input)
{
    if (input.Numbers == null)
    {
        throw new InvalidInputException("No numbers present for sum");
    }

    if (input.ThreadsCount < 1)
    {
        throw new InvalidInputException("Threads count need to be greater than 0");
    }
}