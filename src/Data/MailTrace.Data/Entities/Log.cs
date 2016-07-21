namespace MailTrace.Data.Entities
{
    using System;

    public class Log
    {
        public int Id { get; set; }

        public string Host { get; set; }

        public string QueueId { get; set; }

        public string Message { get; set; }

        public bool Processed { get; set; }

        public DateTime SourceTime { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}