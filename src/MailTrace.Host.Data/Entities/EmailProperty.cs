namespace MailTrace.Host.Data.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class EmailProperty
    {
        public int Id { get; set; }

        [Index]
        public string Host { get; set; }

        [Index]
        public string QueueId { get; set; }

        [Index]
        public string Key { get; set; }

        public string Value { get; set; }

        public DateTime SourceTime { get; set; }
    }
}