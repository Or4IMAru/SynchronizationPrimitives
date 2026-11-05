using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SynchronizationPrimitives;

public class SemaphoreDemo
{
    private readonly SemaphoreSlim _semaphore = new(2, 2);

    private int _activeTasks;
    private int _maxActiveTasks;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== SEMAPHORE DEMO ===");

        Task[] tasks = Enumerable.Range(1, 12)
            .Select(id => ProcessAsync(id, cancellationToken))
            .ToArray();

        try
        {
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка выполнения: {ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine($"Максимум одновременных задач: {_maxActiveTasks}");
        Console.WriteLine($"Свободных разрешений: {_semaphore.CurrentCount}");

        Console.WriteLine();
        Console.WriteLine("Проверка повторного использования:");

        await ProcessAsync(99, cancellationToken);

        Console.WriteLine("Проверка завершена.");
    }

    private async Task ProcessAsync(int id, CancellationToken token)
    {
        Console.WriteLine($"Задача {id}: ожидает разрешения");

        bool acquired;
        try
        {
            acquired = await _semaphore.WaitAsync(TimeSpan.FromSeconds(2), token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"Задача {id}: ожидание отменено");
            return;
        }

        if (!acquired)
        {
            Console.WriteLine($"Задача {id}: не получила разрешение за 2 секунды. Завершение.");
            return;
        }

        try
        {
            int active = Interlocked.Increment(ref _activeTasks);
            UpdateMaximum(active);
            Console.WriteLine($"Задача {id}: начала работу. Активно: {active}");

            await Task.Delay(Random.Shared.Next(500, 1500), token);


            Console.WriteLine($"Задача {id}: завершена");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"Задача {id}: отменена во время выполнения");
            throw; 
        }
        finally
        {
            Interlocked.Decrement(ref _activeTasks);

            _semaphore.Release();
            Console.WriteLine($"Задача {id}: освободила разрешение");
        }
    }

    private void UpdateMaximum(int current)
    {
        int previous;

        do
        {
            previous = Volatile.Read(ref _maxActiveTasks);

            if (current <= previous)
                return;

        } while (
            Interlocked.CompareExchange(
                ref _maxActiveTasks,
                current,
                previous
            ) != previous
        );
    }
}