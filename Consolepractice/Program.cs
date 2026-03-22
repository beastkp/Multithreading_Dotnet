using System;
using System.Diagnostics.Metrics;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    // Shared resource — accessible by all threads simultaneously
    static int counter = 0;

    static void Main()
    {
        int numberOfTasks = 10;
        int incrementsPerTask = 1000;

        Task[] tasks = new Task[numberOfTasks];

        for (int i = 0; i < numberOfTasks; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < incrementsPerTask; j++)
                {
                    // Critical Section — counter++ is NOT just a single operation.
                    // It's actually 3 operations: Read -> Increment -> Write.
                    // When multiple threads execute this simultaneously,
                    // some increments get overwritten — this is a Race Condition.
                    counter++;
                    Thread.SpinWait(100000); // Simulating some work
                }
            });
        }

        Task.WaitAll(tasks);

        // Expected: 10,000 — but actual will almost always be less
        // because threads "stepped on each other" during counter++
        Console.WriteLine($"Expected Value: {numberOfTasks * incrementsPerTask}");
        Console.WriteLine($"Actual Value:   {counter}");

        Console.ReadLine();
    }
}