using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using MessageEncoder.Logging.Strategies;
using MessageEncoder.Logging.Strategies.Contracts;

namespace MessageEncoder.Logging
{
    internal static class Logger
    {
        private static readonly ILogWriteStrategy _writeStrategy;

        private static ConcurrentQueue<LoggerMessage> _messagesQueue = new();
        private static bool _isWorking = false;
        private static Task _workingTask;

        static Logger()
        {
            //TODO: использовать DI в static, bruh
            _writeStrategy = new ConsolePrintStrategy();
        }

        public static void Run()
        {
            _isWorking = true;
            _workingTask = Task.Run(() =>
            {
                while (_isWorking || _messagesQueue.TryPeek(out _))
                {
                    if (_messagesQueue.TryDequeue(out LoggerMessage message))
                    {
                        if (!_writeStrategy.WriteLog(message))
                        {
                            //ignored
                        }
                    }
                }
            });
        }

        public static void Wait()
        {
            _isWorking = false;
            _workingTask.Wait();
        }

        public static void Log(string message, string header = null)
        {
            AddRecord(new LoggerMessage
            {
                Message = message,
                Header = header,
                Type = LoggerMessageType.Info,
            });
        }

        public static void LogError(string message, string header = "Error")
        {
            AddRecord(new LoggerMessage
            {
                Message = message,
                Header = header,
                Type = LoggerMessageType.Error,
            });
        }

        public static void LogWarning(string message, string header = "Warning")
        {
            AddRecord(new LoggerMessage
            {
                Message = message,
                Header = header,
                Type = LoggerMessageType.Warning,
            });
        }

        private static void AddRecord(LoggerMessage message)
        {
            _messagesQueue.Enqueue(message);
        }
    }
}
