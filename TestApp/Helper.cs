namespace TestApp;

public static class Helper
{
    public static IEnumerable<(int start, int end)> GetRanges(int n, int batchesCount)
    {
        if (batchesCount == 0 || batchesCount > n)
        {
            yield return (0, n);
        }

        var batchSize = n / batchesCount;
        var rest = n % batchesCount;

        var i = 0;
        while (i < n)
        {
            if (i + batchSize <= n)
            {
                yield return (i, i + batchSize);
                i += batchSize;
            }
            else
            {
                yield return (i, i + rest);
                i += rest;
            }
        }
    }
}
