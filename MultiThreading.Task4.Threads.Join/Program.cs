/*
 * 4.	Write a program which recursively creates 10 threads.
 * Each thread should be with the same body and receive a state with integer number, decrement it,
 * print and pass as a state into the newly created thread.
 * Use Thread class for this task and Join for waiting threads.
 * 
 * Implement all of the following options:
 * - a) Use Thread class for this task and Join for waiting threads.
 * - b) ThreadPool class for this task and Semaphore for waiting threads.
 */

using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task4.Threads.Join
{
    class Program
    {
        private static SemaphoreSlim semaphore = new SemaphoreSlim(0, 1);

        static void Main(string[] args)
        {
            Console.WriteLine("4.	Write a program which recursively creates 10 threads.");
            Console.WriteLine("Each thread should be with the same body and receive a state with integer number, decrement it, print and pass as a state into the newly created thread.");
            Console.WriteLine("Implement all of the following options:");
            Console.WriteLine();
            Console.WriteLine("- a) Use Thread class for this task and Join for waiting threads.");
            Console.WriteLine("- b) ThreadPool class for this task and Semaphore for waiting threads.");

            Console.WriteLine();

            CreateThreads(10);

            Console.WriteLine(string.Empty.PadRight(20, '*'));

            CreateTasks(10);

            Console.ReadLine();
        }

        private static void CreateThreads(int number)
        {
            if (number == 0) return;

            var thread = new Thread(() =>
            {
                PrintValue(--number);
                CreateThreads(number);
            });

            thread.Start();
            thread.Join();
        }

        private static void CreateTasks(int number)
        {
            if (number == 0) return;

            Task.Run(() =>
            {
                PrintValue(--number);
                CreateTasks(number);

                semaphore.Release(1);
            });

            semaphore.Wait();
        }

        private static void PrintValue(int value) => Console.WriteLine($"Thread: {Thread.CurrentThread.ManagedThreadId}. Current value: {value}");
    }
}
