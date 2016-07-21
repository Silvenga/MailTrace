namespace MailTrace.Data.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class Email
    {
        public string Id { get; set; }

        [Index]
        public string MessageId { get; set; }
    }
}