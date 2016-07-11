namespace MailTrace.Host.Data.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class Email
    {
        public string Id { get; set; }

        [Index]
        public string Host { get; set; }

        [Index]
        public string QueueId { get; set; }
    }
}