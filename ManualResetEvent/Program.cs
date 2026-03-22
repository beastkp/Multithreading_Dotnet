using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    // Gate starts closed (false) — all workers wait until system is ready
    static ManualResetEvent _startGate = new ManualResetEvent(false);

    static async Task Main()
    {
        Console.WriteLine("System initializing — workers standing by...\n");

        int numberOfWorkers = 5;
        Task[] workers = new Task[numberOfWorkers];

        // Spin up all workers, they all block at the gate immediately
        for (int i = 1; i <= numberOfWorkers; i++)
        {
            int workerId = i;
            workers[i - 1] = Task.Run(() => Worker(workerId));
        }

        Console.WriteLine("Loading shared configuration...");
        await Task.Delay(3000);
        Console.WriteLine("Configuration loaded");

        Console.WriteLine("\nStarting gun fired — all workers released simultaneously!\n");

        // Set() opens the gate permanently — ALL workers pass through at once
        _startGate.Set();

        await Task.WhenAll(workers);

        Console.WriteLine("\nAll workers completed.");
        Console.ReadLine();
    }

    static void Worker(int workerId)
    {
        Console.WriteLine($"Worker {workerId} — ready and waiting at the gate");

        // All workers block here until ManualResetEvent is Set()
        _startGate.WaitOne();

        // All workers rush through simultaneously once gate opens
        Console.WriteLine($"Worker {workerId} — released, starting work");

        // Simulate work
        Thread.Sleep(new Random().Next(500, 2000));

        Console.WriteLine($"Worker {workerId} — work completed");
    }
}
