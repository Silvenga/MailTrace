namespace MailTrace.Host.Data.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class EmailProperty
    {
        private const string HostQueueIdIndex = "IX_" + "_" + nameof(Host) + "_" + nameof(QueueId);

        public int Id { get; set; }

        [Index(HostQueueIdIndex, 0)]
        public string Host { get; set; }

        [Index(HostQueueIdIndex, 1)]
        public string QueueId { get; set; }

        [Index]
        public string Key { get; set; }

        [Index]
        public string Value { get; set; }

        [Index]
        public DateTime SourceTime { get; set; }
    }
}