using System;
using System.Collections.Concurrent;

using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        int[] numbers = Enumerable.Range(1, 1000000).ToArray();


        int degreeOfParallelism = Environment.ProcessorCount;

        if (args.Length > 0 && int.TryParse(args[0], out int n))
        {
            degreeOfParallelism = n;
        }
        else
        {
            Console.WriteLine("Введiть кількiсть потокiв:");
            if (int.TryParse(Console.ReadLine(), out n))
            {
                degreeOfParallelism = n;
            }
        }

        Console.WriteLine($"Використовується {degreeOfParallelism} потокiв");

        Stopwatch stopwatch = new Stopwatch();

        
        stopwatch.Start();
        long parallelSum = CalculateParallelSum(numbers, degreeOfParallelism);
        stopwatch.Stop();
        Console.WriteLine("Parallel Sum: " + parallelSum);
        Console.WriteLine($"Паралельне виконання: {stopwatch.ElapsedMilliseconds} ms");

        stopwatch.Restart();
        long sequentialSum = 0;
        foreach (int number in numbers)
        {
            sequentialSum += Calculate(number);
        }
        stopwatch.Stop();
        Console.WriteLine("Sequential Sum: " + sequentialSum);
        Console.WriteLine($"Секвенцiйне виконання: {stopwatch.ElapsedMilliseconds} ms");
    }

    static long CalculateParallelSum(int[] numbers, int degreeOfParallelism)
    {
        long sum = 0;
        object lockObject = new object();

        Parallel.ForEach(
            Partitioner.Create(0, numbers.Length),
            new ParallelOptions { MaxDegreeOfParallelism = degreeOfParallelism },
            () => 0L,
            (range, state, localSum) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    localSum += Calculate(numbers[i]);
                }
                return localSum;
            },
            localSum =>
            {
                lock (lockObject)
                {
                    sum += localSum;
                }
            });

        return sum;
    }

    static long Calculate(int number)
    {
        return (long)number * number;
    }
}
