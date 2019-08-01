using System;
using HbaseNetCoreTest;

namespace HbaseNetCoreConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var rwTest = new HbaseRWTest();
            rwTest.HbaseRWAllTest();
            Console.ReadLine();
        }
    }
}
