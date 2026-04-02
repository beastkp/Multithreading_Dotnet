using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        int totalWorkers = 5;

        // Initialize with the number of workers we need to wait for
        CountdownEvent countdown = new CountdownEvent(totalWorkers);

        Console.WriteLine($"Waiting for {totalWorkers} workers to complete...\n");

        for (int i = 1; i <= 5; i++)
        {
            int workerId = i;
            await Task.Run(() =>
            {
                // Simulate work taking different amounts of time
                Thread.Sleep(new Random().Next(500, 2000));
                Console.WriteLine($"Worker {workerId} — completed.");

                // Signal that this worker is done — decrements the count
                countdown.Signal();
            });
        }

        // Block until all workers have signalled completion
        countdown.Wait();

        Console.WriteLine("\nAll workers done — proceeding with next step");
        Console.ReadLine();
    }
}