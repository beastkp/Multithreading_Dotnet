using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
    // Thread safe dictionary — no manual locking needed
    static ConcurrentDictionary<string, int> _requestCounts
        = new ConcurrentDictionary<string, int>();

    static async Task Main()
    {
        string[] endpoints = {
            "/api/users",
            "/api/products",
            "/api/orders",
            "/api/users",     
            "/api/products",
            "/api/users"
        };

        Console.WriteLine("Simulating concurrent API requests...\n");

        List<Task> tasks = new List<Task>();

        // Simulate 50 concurrent requests hitting random endpoints
        Random random = new Random();
        for (int i = 1; i <= 50; i++)
        {
            string endpoint = endpoints[random.Next(endpoints.Length)];  // Duplicates simulate real traffic
            tasks.Add(Task.Run(() => TrackRequest(endpoint)));
        }

        await Task.WhenAll(tasks);

        Console.WriteLine("\nRequest counts:");
        foreach (var entry in _requestCounts)
            Console.WriteLine($"{entry.Key} — {entry.Value} requests");

        Console.ReadLine();
    }

    static void TrackRequest(string endpoint)
    {
        // AddOrUpdate is atomic — no lock needed
        // If key exists  increment the count else initialize to 1
        _requestCounts.AddOrUpdate(
            key: endpoint,
            addValue: 1,
            updateValueFactory: (key, existingCount) => existingCount + 1
        );

        Console.WriteLine($"Tracked request to {endpoint}");
    }
}