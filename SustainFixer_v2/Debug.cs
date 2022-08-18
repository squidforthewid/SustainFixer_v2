using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SustainFixer
{
    public enum Log
    {
        Warning,
        Error
    }

    public static class Debug
    {
        public static void WriteLine(string line, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(line);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void ReadLine()
        {
            Console.ReadLine();
        }

        public static void Write(string line, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;
            Console.Write(line);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
