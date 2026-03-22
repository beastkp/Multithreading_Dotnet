using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    // Gate starts closed (false) — consumer waits until producer signals
    static AutoResetEvent _signal = new AutoResetEvent(false);
    static Queue<string> _workQueue = new Queue<string>();
    static bool _producerDone = false;

    static async Task Main()
    {
        Console.WriteLine("Producer Consumer with AutoResetEvent\n");

        Task producer = Task.Run(Producer);
        Task consumer = Task.Run(Consumer);

        await Task.WhenAll(producer, consumer);

        Console.WriteLine("\nAll work completed.");
        Console.ReadLine();
    }

    static void Producer()
    {
        string[] workItems = { "Order #1001", "Order #1002", "Order #1003", "Order #1004", "Order #1005" };

        foreach (var item in workItems)
        {
            // Simulate time taken to produce a work item
            Thread.Sleep(500);

            _workQueue.Enqueue(item);
            Console.WriteLine($"Producer — queued '{item}'");

            // Signal the consumer that one item is ready
            _signal.Set();
        }

        _producerDone = true;
        _signal.Set(); // Final signal to unblock consumer so it can exit
    }

    static void Consumer()
    {
        while (true)
        {
            // Wait for producer to signal that an item is ready
            _signal.WaitOne();

            if (_workQueue.Count == 0 && _producerDone)
            {
                Console.WriteLine("Consumer — no more items, exiting");
                break;
            }

            while (_workQueue.Count > 0)
            {
                string item = _workQueue.Dequeue();

                // Simulate processing time
                Thread.Sleep(300);

                Console.WriteLine($"Consumer — processed '{item}'");
            }
        }
    }
}