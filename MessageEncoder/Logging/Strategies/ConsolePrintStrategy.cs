using System;

using MessageEncoder.Logging.Strategies.Contracts;

namespace MessageEncoder.Logging.Strategies
{
    public class ConsolePrintStrategy : ILogWriteStrategy
    {
        private const ConsoleColor DefaultForegroundColor = ConsoleColor.Gray;

        public ConsolePrintStrategy()
        {
            Console.ResetColor();
        }

        public bool WriteLog(LoggerMessage message)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(message.Header))
                {
                    Console.Write("[");
                    Console.ForegroundColor = MapColor(message.Type);
                    Console.Write(message.Header);
                    Console.ForegroundColor = DefaultForegroundColor;
                    Console.Write("]: ");
                }

                Console.WriteLine(message.Message);
                return true;
            }
            catch
            {
                return false;
            }
        }


        private ConsoleColor MapColor(LoggerMessageType type)
        {
            var result = type switch
            {
                LoggerMessageType.Info => ConsoleColor.DarkYellow,
                LoggerMessageType.Warning => ConsoleColor.Yellow,
                LoggerMessageType.Error => ConsoleColor.Red,
                _ => ConsoleColor.DarkGray,
            };
            return result;
        }
    }
}
