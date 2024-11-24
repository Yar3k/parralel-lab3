using System.Diagnostics;
using System.Runtime.InteropServices;
class Program
{
    private static readonly object _lock = new object();
    private static Mode _mode;

    static void Main(string[] args)
    {
        int start;
        int end;
        (long, long) biggestItteration = (0, 0);
        int processors;

        if (args.Length < 3)
        {
            Console.WriteLine("Please provide required arguments: dotnet run <interval_from> <interval_to> <mode[normal|debug]> <processors_count(optional)>");
            return;
        }

        start = int.Parse(args[0]);
        end = int.Parse(args[1]);

        if (!Enum.TryParse(args[2], true, out _mode))
        {
            Console.WriteLine("Invalid mode. Use 'normal' or 'debug'.");
            return;
        }

        processors = args.Length == 4 ? int.Parse(args[3]) : Environment.ProcessorCount;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            int affinity = (int)Math.Pow(2, processors) - 1;
            Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(affinity);
        }
        else
        {
            Console.WriteLine($"Environment is nething Linux, neither Windows! Will be used maximum available processors ({Environment.ProcessorCount})");
        }


        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();


        Parallel.For(start, end + 1, () =>
        (0L, 0L),
        (i, state, localMax) =>
        {
            var result = collatzConjecture(i);
            return result.Item2 > localMax.Item2 ? result : localMax;
        },
        localMax =>
        {
            lock (_lock)
            {
                if (localMax.Item2 > biggestItteration.Item2)
                    biggestItteration = localMax;
            }
        }
    );

        stopWatch.Stop();
        TimeSpan elapsedTime = stopWatch.Elapsed;

        Console.WriteLine("Runtime " + elapsedTime);
        Console.WriteLine($"Biggest found {biggestItteration.Item1} with len: {biggestItteration.Item2}");
    }

    static (long, long) collatzConjecture(long n)
    {
        long initN = n;
        long iteration = 0;

        while (n != 1)
        {
            iteration++;

            if (_mode == Mode.Debug)
            {
                Console.WriteLine($"Step {iteration}: Current value = {n}");
            }

            if ((n & 1) == 1)
                n = 3 * n + 1;
            else
                n = n / 2;
        }

        if (_mode == Mode.Debug)
        {
            Console.WriteLine($"Finished sequence for {initN} with {iteration} iterations.");
        }

        return (initN, iteration);
    }
}