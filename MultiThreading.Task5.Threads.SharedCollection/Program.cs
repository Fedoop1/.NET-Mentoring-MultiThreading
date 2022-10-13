/*
 * 5. Write a program which creates two threads and a shared collection:
 * the first one should add 10 elements into the collection and the second should print all elements
 * in the collection after each adding.
 * Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.
 */
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task5.Threads.SharedCollection
{
    class Program
    {
        private static Random random = new Random();
        private static AutoResetEvent autoReadEvent = new AutoResetEvent(false);
        private static AutoResetEvent autoWriteEvent = new AutoResetEvent(true);

        private static List<int> collection = new List<int>();

        static void Main(string[] args)
        {
            Console.WriteLine("5. Write a program which creates two threads and a shared collection:");
            Console.WriteLine("the first one should add 10 elements into the collection and the second should print all elements in the collection after each adding.");
            Console.WriteLine("Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.");
            Console.WriteLine();

            var cts = new CancellationTokenSource();

            var writer = Task.Run(() => WriteElements(10));
            var reader = Task.Run(() => ReadAndPrintElements(cts.Token), cts.Token);

            try
            {
                writer.Wait();
            }
            catch (AggregateException exception)
            {
                foreach (var exceptionInnerException in exception.InnerExceptions)
                {
                    Console.WriteLine(exceptionInnerException.Message);
                }
            }
            finally
            {
                cts.Cancel();
            }

            Console.ReadLine();
        }

        private static void WriteElements(int elementsCount)
        {
            while (elementsCount > 0)
            {
                autoWriteEvent.WaitOne();

                collection.Add(random.Next(1, 10001));
                elementsCount--;

                autoReadEvent.Set();
            }
        }

        private static void ReadAndPrintElements(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                autoReadEvent.WaitOne();

                foreach (var item in collection)
                {
                    Console.Write(item + " ");
                }

                Console.WriteLine();
                autoWriteEvent.Set();
            }
        }
    }
}
