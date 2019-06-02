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

    class Hest
    {
        public event EventHandler OnKnegg;

        public Hest()
        {

        }

        public void Knegg()
        {
            
        }
    }

    class Program
    {
        public EventHandler Event;

        public string Property { get; set;}

        public Program()
        {
            var hest = new Hest();
            hest.OnKnegg += HarKnegga;
            var a = 5.GetString();
        }

        private static void HarKnegga(object sender, EventArgs e)
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
            var a = 1;
        }

        public static string GetString(this int value)
        {
            return $"{value}";
        }
    }
}
