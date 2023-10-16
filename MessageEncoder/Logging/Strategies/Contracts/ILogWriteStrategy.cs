using System;

namespace MessageEncoder.Logging.Strategies.Contracts
{
    public interface ILogWriteStrategy
    {
        bool WriteLog(LoggerMessage message);
    }
}
