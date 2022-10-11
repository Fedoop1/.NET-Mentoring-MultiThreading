/*
 * 2.	Write a program, which creates a chain of four Tasks.
 * First Task – creates an array of 10 random integer.
 * Second Task – multiplies this array with another random integer.
 * Third Task – sorts this array by ascending.
 * Fourth Task – calculates the average value. All this tasks should print the values to console.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiThreading.Task2.Chaining
{
    class Program
    {
        private const int RandomIntegersCount = 10;

        private static readonly Random Random = new Random();

        static void Main(string[] args)
        {
            Console.WriteLine(".Net Mentoring Program. MultiThreading V1 ");
            Console.WriteLine("2.	Write a program, which creates a chain of four Tasks.");
            Console.WriteLine("First Task – creates an array of 10 random integer.");
            Console.WriteLine("Second Task – multiplies this array with another random integer.");
            Console.WriteLine("Third Task – sorts this array by ascending.");
            Console.WriteLine("Fourth Task – calculates the average value. All this tasks should print the values to console");
            Console.WriteLine();

            Task.Run(() =>
            {
                var result = new int[RandomIntegersCount];
                for (var index = 0; index < RandomIntegersCount; index++) result[index] = Random.Next();

                Console.WriteLine($"First task created array of ten random integers");
                PrintArray(result);

                return result;
            }).ContinueWith((source) =>
            {
                var randomInt = Random.Next();

                var result = source.Result.Select(i => i * randomInt).ToArray();

                Console.WriteLine($"Second task multiplied the array by {randomInt}. ");
                PrintArray(result);

                return result;
            }).ContinueWith(source =>
            {
                Array.Sort(source.Result);

                Console.WriteLine("Third task sorted the array by ascending.");
                PrintArray(source.Result);

                return source.Result;
            }).ContinueWith(source =>
                Console.WriteLine($"Fourth task calculated the average. Average value: {source.Result.Average()}"));

            Console.ReadLine();
        }

        private static void PrintArray(IEnumerable<int> source) => Console.WriteLine($"Current values: [{string.Join(", ", source)}]");
    }
}
