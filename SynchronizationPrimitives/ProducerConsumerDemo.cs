using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SynchronizationPrimitives;

public class ProducerConsumerDemo : IDisposable
{
    private readonly ConcurrentQueue<int> queue = new();

    private readonly AutoResetEvent signal = new(false);

    private int processedCount;

    public async Task RunAsync()
    {
        Console.WriteLine();
        Console.WriteLine("=== PRODUCER-CONSUMER DEMO ===");

        Task consumerTask = Task.Run(Consume);

        Task producer1 = Task.Run(() => Produce(1, 10));
        Task producer2 = Task.Run(() => Produce(2, 10));
        Task producer3 = Task.Run(() => Produce(3, 10));
        Task producer4 = Task.Run(() => Produce(4, 10));

        await Task.WhenAll(producer1, producer2, producer3, producer4);

        queue.Enqueue(-1);

        signal.Set();

        await consumerTask;

        Console.WriteLine();
        Console.WriteLine($"Обработано элементов: {processedCount}");
    }

    private void Produce(int producerId, int count)
    {
        for (int i = 1; i <= count; i++)
        {
            int value = producerId * 100 + i;

            queue.Enqueue(value);

            Console.WriteLine($"Производитель {producerId}: создал {value}");

            signal.Set();

            Thread.Sleep(Random.Shared.Next(100, 400));
        }

        Console.WriteLine($"Производитель {producerId}: завершил работу");
    }

    private void Consume()
    {
        while (true)
        {
            signal.WaitOne();

            while (queue.TryDequeue(out int value))
            {
                if (value == -1)
                {
                    Console.WriteLine("Потребитель завершает работу.");
                    return;
                }

                if (value > 250)
                {
                    Console.WriteLine($"  [!] Высокоприоритетная задача: {value}");
                }

                Console.WriteLine($"Потребитель обработал: {value}");

                Interlocked.Increment(ref processedCount);

                Thread.Sleep(1000);
            }
        }
    }

    public void Dispose()
    {
        signal.Dispose();
    }
}