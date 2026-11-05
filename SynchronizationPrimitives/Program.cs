using System;
using System.Threading.Tasks;
using SynchronizationPrimitives;

namespace SynchronizationPrimitives;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("\n=== ПРИМИТИВЫ СИНХРОНИЗАЦИИ ===\n");
        Console.WriteLine("1. SemaphoreSlim (Ограничение параллелизма)");
        Console.WriteLine("2. Producer-Consumer (AutoResetEvent + ConcurrentQueue)");
        Console.WriteLine("3. Mutex (Синхронизация между процессами)");

        Console.Write("\nВыберите эксперимент (1, 2 или 3): ");
        string? choice = Console.ReadLine();

        Console.WriteLine();

        switch (choice)
        {
            case "1":
                {
                    var demo = new SemaphoreDemo();
                    await demo.RunAsync();
                    break;
                }
            case "2":
                {
                    using var demo = new ProducerConsumerDemo();
                    await demo.RunAsync();
                    break;
                }
            case "3":
                {
                    MutexDemo.Run();
                    break;
                }
            default:
                {
                    Console.WriteLine("Неизвестная команда. Пожалуйста, введите 1, 2 или 3.");
                    break;
                }
        }

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
}