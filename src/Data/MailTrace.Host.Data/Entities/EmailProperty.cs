﻿namespace MailTrace.Host.Data.Entities
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

        [Index, Required]
        public string Key { get; set; }

        [Index, Required]
        public string Value { get; set; }

        [Index, Required]
        public DateTime? SourceTime { get; set; }
    }
}