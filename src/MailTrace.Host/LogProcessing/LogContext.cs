namespace MailTrace.Host.LogProcessing
{
    using System;
    using System.Collections.Generic;

    public interface ILogContext
    {
        IList<Log> Logs { get; set; }
    }

    public class LogContext : ILogContext
    {
        public IList<Log> Logs { get; set; } = new List<Log>();
    }

    public class Log
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public bool Processed { get; set; }
    }
}