using System;
using System.Threading;

namespace SynchronizationPrimitives;

public class MutexDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== MUTEX ДЕМО ===");
        Console.WriteLine("Этот эксперимент демонстрирует синхронизацию между процессами.");
        Console.WriteLine("1. Запустите приложение.");
        Console.WriteLine("2. Не закрывая его, запустите вторую копию этого же приложения.");
        Console.WriteLine("3. Вторая копия сообщит, что ресурс занят.\n");

        using Mutex mutex = new(
            initiallyOwned: false,
            name: "Local\\SynchronizationPrimitivesDemo"
        );

        bool acquired = mutex.WaitOne(0);

        if (!acquired)
        {
            Console.WriteLine("[ВНИМАНИЕ] Другой экземпляр приложения уже работает и удерживает блокировку.");
            Console.WriteLine("Завершите первую копию приложения, чтобы освободить ресурс.");
            return;
        }

        try
        {
            Console.WriteLine("[УСПЕХ] Блокировка получена. Ресурс занят текущим процессом.");
            Console.WriteLine("Нажмите Enter для освобождения блокировки и завершения работы...");

            Console.ReadLine();
        }
        finally
        {
            mutex.ReleaseMutex();
            Console.WriteLine("Блокировка освобождена.");
        }
    }
}