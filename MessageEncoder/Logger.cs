using System;
using System.Collections.Generic;
using System.Text;

namespace MessageEncoder
{
    internal static class Logger
    {
        static Logger() 
        {
            Console.ResetColor();
        }

        public static void Log(string message, string header = null)
        {
            if (!string.IsNullOrWhiteSpace(header))
            {
                PrintHeader(header);
            }

            Console.WriteLine(message);
        }

        public static void LogError(string message, string header = "Error")
        {
            PrintHeader(header, ConsoleColor.Red);
            Console.WriteLine(message);
        }

        public static void LogWarning(string message, string header = "Warning")
        {
            PrintHeader(header, ConsoleColor.Yellow);
            Console.WriteLine(message);
        }

        private static void PrintHeader(string header, ConsoleColor color = ConsoleColor.DarkYellow)
        {
            Console.Write("[");
            Console.ForegroundColor = color;
            Console.Write(header);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("]: ");
        }
    }
}
