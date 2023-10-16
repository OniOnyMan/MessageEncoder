using System;
using System.Collections.Generic;
using System.Text;

namespace MessageEncoder.Logging
{
    public enum LoggerMessageType
    {
        // WIP
        Info,
        Warning,
        Error,
    }

    public class LoggerMessage
    {
        public string Message { get; set; }

        public string Header { get; set; }

        public LoggerMessageType Type { get; set; }
    }
}
