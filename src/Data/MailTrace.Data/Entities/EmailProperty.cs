namespace MailTrace.Data.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class EmailProperty
    {
        private const string HostQueueIdIndex = "IX_" + "_" + nameof(Host) + "_" + nameof(QueueId);

        public int Id { get; set; }

        [Index(HostQueueIdIndex, 0), Required]
        public string Host { get; set; }

        [Index(HostQueueIdIndex, 1), Required]
        public string QueueId { get; set; }

        [Required, StringLength(32)]
        public string LogId { get; set; }

        [Index, Required]
        public string Key { get; set; }

        [Index, Required]
        public string Value { get; set; }

        [Index, Required]
        public DateTime? SourceTime { get; set; }

        public override string ToString()
        {
            return $"{Key}: {Value}, SourceTime: {SourceTime}, LogId: {LogId}";
        }
    }
}