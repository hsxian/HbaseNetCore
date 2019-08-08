using System;
using System.Diagnostics;
using System.Linq;
using HbaseNetCore.Parsers;
using HbaseNetCoreTest;
using HbaseNetCoreTest.Models;
using HbaseNetCore.Converts;

namespace HbaseNetCoreConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // var sth = new Stopwatch();
            // sth.Start();
            // var range = Enumerable.Range(0, 1000).ToList();
            // var students = range
            //     .Select(t => new Student { RowKey = t.ToString(), Name = $"hsx{t}", Age = t })
            //     .ToList();
            // Console.WriteLine($"\tcreate class,count:{students.Count}, time: {sth.Elapsed}");

            // sth.Restart();
            // var batchs = new HbaseParser().ToBatchMutations(students);
            // Console.WriteLine($"\tParser Class To BatchMutation Async,count:{batchs.Count}, time: {sth.Elapsed}");


            var rwTest = new HbaseRWTest();
            rwTest.HbaseRWAllTest();
            Console.ReadLine();
        }
    }
}
