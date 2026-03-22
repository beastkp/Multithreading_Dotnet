using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    // Shared in-memory store
    static Dictionary<string, string> _store = new Dictionary<string, string>
    {
        { "config:timeout", "30s" },
        { "config:retries", "3" },
        { "config:environment", "production" }
    };

    // The slim lock controlling access to the store
    static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

    static async Task Main()
    {
        Console.WriteLine("Starting in-memory store simulation...\n");
        Console.WriteLine("Initial store state:");

        PrintStore();
        Console.WriteLine("\n");

        List<Task> tasks = new List<Task>();

        // Spin up 6 reader tasks — all can run simultaneously
        for (int i = 1; i <= 6; i++)
        {
            int readerId = i;
            tasks.Add(Task.Run(() => ReadFromStore(readerId, "config:timeout")));
        }

        // Spin up 2 writer tasks — must wait for all readers to finish
        for (int i = 1; i <= 2; i++)
        {
            int writerId = i;
            tasks.Add(Task.Run(() => WriteToStore(writerId, $"config:timeout", $"{writerId * 10}s")));
        }

        await Task.WhenAll(tasks);

        Console.WriteLine("\nFinal store state:");
        PrintStore();

        Console.ReadLine();
    }

    static void ReadFromStore(int readerId, string key)
    {
        // Acquire read lock — multiple readers can hold this simultaneously
        _lock.EnterReadLock();
        try
        {
            Console.WriteLine($"Reader {readerId} — acquired read lock");

            // Simulate read taking some time
            Thread.Sleep(500);

            string value = _store.ContainsKey(key) ? _store[key] : "not found";
            Console.WriteLine($"Reader {readerId} — read '{key}' = '{value}'");
        }
        finally
        {
            _lock.ExitReadLock();
            Console.WriteLine($"Reader {readerId} — released read lock\n");
        }
    }

    static void WriteToStore(int writerId, string key, string value)
    {
        // Acquire write lock — only one writer, no readers allowed simultaneously
        _lock.EnterWriteLock();
        try
        {
            Console.WriteLine($"Writer {writerId} — acquired write lock (all readers blocked)");

            // Simulate write taking some time
            Thread.Sleep(1000);

            _store[key] = value;
            Console.WriteLine($"Writer {writerId} — updated '{key}' to '{value}'");
        }
        finally
        {
            _lock.ExitWriteLock();
            Console.WriteLine($"Writer {writerId} — released write lock\n");
        }
    }

    static void PrintStore()
    {
        _lock.EnterReadLock();
        try
        {
            foreach (var entry in _store)
                Console.WriteLine($"  {entry.Key} = {entry.Value}");
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }
}