using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace MessageEncoder
{
    internal static class Logger
    {
        private const ConsoleColor DefaultForegroundColor = ConsoleColor.Gray;

        public class LoggerMessage
        {
            public string Message { get; set; }
            public string Header { get; set; }
            public ConsoleColor HeaderColor { get; set; } = ConsoleColor.DarkYellow;
        }

        private static ConcurrentQueue<LoggerMessage> _messagesQueue = new ();
        private static bool _isWorking = false;

        static Logger() 
        {
            Console.ResetColor();
        }

        public static void Log(string message, string header = null)
        {
            AddRecord(new LoggerMessage 
            {
                Message = message,
                Header = header,
                HeaderColor = ConsoleColor.DarkYellow,
            });
        }

        public static void LogError(string message, string header = "Error")
        {
            AddRecord(new LoggerMessage
            {
                Message = message,
                Header = header,
                HeaderColor = ConsoleColor.Red,
            });
        }

        public static void LogWarning(string message, string header = "Warning")
        {
            AddRecord(new LoggerMessage
            {
                Message = message,
                Header = header,
                HeaderColor = ConsoleColor.Yellow,
            });
        }

        public static void TurnOff()
        {
            _isWorking = false;
        }

        public static Task TurnOn()
        {
            _isWorking = true;
            return Task.Run(() => 
            {
                while (_isWorking || _messagesQueue.TryPeek(out _))
                {
                    if (_messagesQueue.TryDequeue(out LoggerMessage record))
                    {
                        PrintRecord(record);
                    }
                }
            });
        }

        private static void PrintRecord(LoggerMessage record)
        {
            if (!string.IsNullOrWhiteSpace(record.Header))
            {
                Console.Write("[");
                Console.ForegroundColor = record.HeaderColor;
                Console.Write(record.Header);
                Console.ForegroundColor = DefaultForegroundColor;
                Console.Write("]: ");
            }

            Console.WriteLine(record.Message);
        }

        private static void AddRecord(LoggerMessage message)
        {
            _messagesQueue.Enqueue(message);
        }
    }
}
