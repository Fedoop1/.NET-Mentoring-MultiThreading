/*
*  Create a Task and attach continuations to it according to the following criteria:
   a.    Continuation task should be executed regardless of the result of the parent task.
   b.    Continuation task should be executed when the parent task finished without success.
   c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation
   d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled
   Demonstrate the work of the each case with console utility.
*/
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task6.Continuation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Create a Task and attach continuations to it according to the following criteria:");
            Console.WriteLine("a.    Continuation task should be executed regardless of the result of the parent task.");
            Console.WriteLine("b.    Continuation task should be executed when the parent task finished without success.");
            Console.WriteLine("c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.");
            Console.WriteLine("d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled.");
            Console.WriteLine("Demonstrate the work of the each case with console utility.");
            Console.WriteLine();

            Task a = Task.Run(() => throw new ArgumentException("Exception") { })
                .ContinueWith(state => Console.WriteLine($"A: Parent task is faulted: {state.IsFaulted}"));

            Task b = Task.Run(() => throw new ArgumentException("Exception"))
                .ContinueWith(state => Console.WriteLine("B: Parent task executed unsuccessfully"), TaskContinuationOptions.NotOnRanToCompletion);

            Task c = Task.Run(() => throw new ArgumentException($"Exception in thread {Thread.CurrentThread.ManagedThreadId}"))
                .ContinueWith(state => Console.WriteLine($"C: Parent task executed unsuccessfully. Error: {state.Exception.Message}. Current thread {Thread.CurrentThread.ManagedThreadId}")
            , TaskContinuationOptions.NotOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);

            var cts = new CancellationTokenSource(1500);

            Task d = Task.Run(() => { while (!cts.IsCancellationRequested) { cts.Token.ThrowIfCancellationRequested(); } }, cts.Token)
                .ContinueWith(
                state => Console.WriteLine(
                    $"Parent task cancelled: {state.IsCanceled}. Continuation thread is in thread pool: {Thread.CurrentThread.IsThreadPoolThread}."),
                TaskContinuationOptions.OnlyOnCanceled | TaskContinuationOptions.LongRunning);

            try
            {
                Task.WaitAll(a, b, c, d);
            }
            catch (AggregateException e)
            {
                foreach (var exception in e.InnerExceptions)
                {
                    Console.WriteLine($"{exception.GetType().Name}: {exception.Message}");
                }
            }

            Console.ReadLine();
        }
    }
}
