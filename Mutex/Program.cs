using System;
using System.Threading;
using System.IO;
using System.Diagnostics;

class Program
{
    static string filePath = Path.Combine(Path.GetTempPath(), "counter.txt");
    static Mutex mutex = new Mutex(false, "GlobalCounterMutex");

    static void Main()
    {
        // Current process ID to distinguish between the two running apps
        int processId = Environment.ProcessId;
        //Console.WriteLine($"Counter file location: {filePath}");

        Console.WriteLine($"Process {processId} started");
        Console.WriteLine($"Process {processId} waiting to acquire Mutex...\n");

        for (int i = 0; i < 5; i++)
        {
            UpdateCounter(processId, i + 1);
        }

        Console.WriteLine($"\nProcess {processId} completed.");
        Console.WriteLine($"Final Counter Value: {ReadCounter()}");

        Console.ReadLine();
    }

    static void UpdateCounter(int processId, int iteration)
    {
        mutex.WaitOne(); // Block until this process owns the Mutex
        Console.WriteLine($"Process {processId} — Iteration {iteration} — Mutex acquired");

        try
        {
            int value = ReadCounter();
            value++;
            WriteCounter(value);
            Console.WriteLine($"Process {processId} — Iteration {iteration} — Counter updated to {value}");
            //Thread.Sleep(1000);

        }
        finally
        {
            Console.WriteLine($"Process {processId} — Iteration {iteration} — Mutex released\n");
            mutex.ReleaseMutex();
        }
    }

    static int ReadCounter()
    {
        if (!File.Exists(filePath))
            return 0;
        string content = File.ReadAllText(filePath);
        return string.IsNullOrEmpty(content) ? 0 : int.Parse(content);
    }

    static void WriteCounter(int value)
    {
        File.WriteAllText(filePath, value.ToString());
    }
}
