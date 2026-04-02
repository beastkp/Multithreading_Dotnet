using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static Barrier barrier = new Barrier(3, b =>
    {
        // This callback runs after all threads reach the barrier
        // and before they are released to the next phase
        Console.WriteLine($"\n All threads completed Phase {b.CurrentPhaseNumber}. Moving to next phase...\n");
    });

    static async Task Main()
    {
        Task[] tasks = new Task[3];

        for (int i = 0; i < 3; i++)
        {
            int threadId = i;
            tasks[i] = Task.Run(() => DoWork(threadId));
        }

        await Task.WhenAll(tasks);
        Console.ReadLine();
    }

    static void DoWork(int threadId)
    {
        // Phase 1
        Console.WriteLine($"Thread {threadId} — working on Phase 0...");
        Thread.Sleep(new Random().Next(500, 1500));
        Console.WriteLine($"Thread {threadId} — Phase 0 complete, waiting at barrier...");
        barrier.SignalAndWait(); // Wait for all threads to finish Phase 1

        // Phase 2 — only starts after ALL threads finish Phase 1
        Console.WriteLine($"Thread {threadId} — working on Phase 1...");
        Thread.Sleep(new Random().Next(500, 1500));
        Console.WriteLine($"Thread {threadId} — Phase 1 complete, waiting at barrier...");
        barrier.SignalAndWait(); // Wait for all threads to finish Phase 2

        Console.WriteLine($"Thread {threadId} — all phases done");
    }
}