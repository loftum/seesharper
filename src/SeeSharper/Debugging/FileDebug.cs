using System;
using System.IO;

namespace SeeSharper.Debugging
{
    public static class FileDebug
    {
        public static string Filename = @"C:\home\Garbage\Debug.log";

        static FileDebug()
        {
            File.WriteAllText(Filename, "");
        }

        public static void WriteLine(object o)
        {
            WriteLine(o?.ToString());
        }

        public static void WriteLine(string line)
        {
            if (line == null)
            {
                File.AppendAllText(Filename, Environment.NewLine);
            }
            File.AppendAllText(Filename, $"{line}{Environment.NewLine}");
        }

        public static void Write(string line)
        {
            if (line == null)
            {
                return;
            }
            File.AppendAllText(Filename, line);
        }
    }
}