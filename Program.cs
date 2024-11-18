using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
class Program
{
    private static readonly object _lock = new object();

    // static void Main(string[] args)
    // {
    //     int start = 1;
    //     int end = 1_000_000_000;
    //     (long, long) biggestItteration = (0, 0);

    //     // Get the number of available processors
    //     int processorCount = Environment.ProcessorCount;

    //     System.Console.WriteLine(processorCount);

    //     // Split the range into chunks for each core
    //     var ranges = SplitRange(start, end, processorCount);

    //     // Create a task for each range
    //     var tasks = ranges.Select(range =>
    //         Task.Run(() =>
    //         {
    //             for (int i = range.Item1; i <= range.Item2; i++)
    //             {
    //                 collatzConjecture(i, ref biggestItteration);
    //             }
    //         })
    //     ).ToArray();

    //     // Wait for all tasks to complete
    //     Task.WaitAll(tasks);

    //     Console.WriteLine("Searching completed.");
    //     System.Console.WriteLine($"Biggest found {biggestItteration.Item1} with len: {biggestItteration.Item2}");
    // }

    static void Main()
    {
        AutoResetEvent mainEvent = new AutoResetEvent(false);
        int workerThreads;
        int portThreads;

        Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(3);  


        // Get the number of available processors
        int processorCount = Environment.ProcessorCount;

        System.Console.WriteLine(processorCount);


        ThreadPool.GetMaxThreads(out workerThreads, out portThreads);
        Console.WriteLine("\nMaximum worker threads: \t{0}" +
            "\nMaximum completion port threads: {1}",
            workerThreads, portThreads);

        ThreadPool.GetAvailableThreads(out workerThreads, 
            out portThreads);
        Console.WriteLine("\nAvailable worker threads: \t{0}" +
            "\nAvailable completion port threads: {1}\n",
            workerThreads, portThreads);

    }

    static List<Tuple<int, int>> SplitRange(int start, int end, int chunks)
    {
        int rangeSize = (end - start + 1) / chunks;
        var ranges = new List<Tuple<int, int>>();

        for (int i = 0; i < chunks; i++)
        {
            int rangeStart = start + i * rangeSize;
            int rangeEnd = (i == chunks - 1) ? end : rangeStart + rangeSize - 1;

            ranges.Add(Tuple.Create(rangeStart, rangeEnd));
        }

        return ranges;
    }

    static void collatzConjecture(long n, ref (long, long) biggestItteration)
    {
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

        if (iteration > biggestItteration.Item2){
            lock(_lock){
                biggestItteration = (initN, iteration);
            }
            System.Console.WriteLine($"Biggest found {initN} with len: {iteration}");
        }
    }
}
