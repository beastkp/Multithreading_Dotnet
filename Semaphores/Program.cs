using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    // Allow a maximum of 3 concurrent API calls at a time
    static SemaphoreSlim semaphore = new SemaphoreSlim(3, 3);

    static async Task Main()
    {
        int totalRequests = 5;
        List<Task> tasks = new List<Task>();

        Console.WriteLine($"Sending {totalRequests} API requests with max 3 concurrent...\n");

        for (int i = 1; i <= totalRequests; i++)
        {
            int requestId = i;
            tasks.Add(SimulateApiCall(requestId));
        }

        await Task.WhenAll(tasks);

        Console.WriteLine("\nAll requests completed.");
        Console.ReadLine();
    }

    static async Task SimulateApiCall(int requestId)
    {
        Console.WriteLine($"Request {requestId} — waiting for slot...");

        // Wait until a slot is available
        await semaphore.WaitAsync();

        try
        {
            Console.WriteLine($"Request {requestId} — slot acquired, calling API...");

            // Simulate API call taking between 1-3 seconds
            await Task.Delay(new Random().Next(1000, 3000));

            Console.WriteLine($"Request {requestId} — API call completed");
        }
        finally
        {
            // Always release the slot — even if the API call fails
            semaphore.Release();
            Console.WriteLine($"Request {requestId} — slot released, next request can enter\n");
        }
    }
}
