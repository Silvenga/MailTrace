namespace MailTrace.Host.Models.Logs
{
    using System;
    using System.Collections.Generic;

    public class LogLine
    {
        public DateTime SourceTime { get; set; }

        public string Host { get; set; }

        public PostfixService Service { get; set; }

        public string QueueId { get; set; }

        public string Message { get; set; }

        public IList<LineAttribute> Attributes { get; set; }
    }
}