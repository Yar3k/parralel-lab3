using System.Diagnostics;
using System.Runtime.InteropServices;
class Program
{
    private static readonly object _lock = new object();

    static void Main(string[] args)
    {
        int start;
        int end;
        string mode;
        (long, long) biggestItteration = (0, 0);
        int processors;

        if (args.Length < 3)
        {
            Console.WriteLine("Please provide required arguments: dotnet run <interval_from> <interval_to> <mode[normal|debug](optional)> <processors_count(optional)>");
            return;
        }
        start = int.Parse(args[0]);
        end = int.Parse(args[1]);
        mode = args[2];
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


        Parallel.For(1, end + 1, () =>
        (long.MaxValue, 0L),
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

        Console.WriteLine("RunTime " + elapsedTime);
        Console.WriteLine($"Biggest found {biggestItteration.Item1} with len: {biggestItteration.Item2}");
    }

    static (long, long) collatzConjecture(long n)
    {
        Console.WriteLine($"Checking {n}");
        long initN = n;
        long iteration = 0;

        while (n != 1)
        {
            iteration++;
            if ((n & 1) == 1)
                n = 3 * n + 1;
            else
                n = n / 2;
        }
        return (initN, iteration);
    }


}
