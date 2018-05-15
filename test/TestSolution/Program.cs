using System;

namespace TestSolution
{
    enum Tools
    {
        Anvil,
        Screwdriver
    }

    struct Transformer
    {
        public string Name { get; }
        public Transformer(string name)
        {
            Name = name;
        }
    }

    class Program
    {
        public EventHandler Event;
        public string Property { get; set;}

        public Program()
        {
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    public static class Extensions
    {
        static Extensions()
        {
        }

        public static string GetString(this int value)
        {
            return $"{value}";
        }
    }
}
