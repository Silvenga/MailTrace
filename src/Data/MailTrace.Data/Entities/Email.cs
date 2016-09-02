namespace MailTrace.Data.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Email
    {
        private const char Deliminator = ';';

        public string Id { get; set; }

        public IList<EmailMeta> EmailMetas { get; set; }

        [Index, Required]
        public string MessageId { get; set; }

        [Index, Required]
        public string Recipient { get; set; }

        [NotMapped]
        public IList<string> RecipientList
        {
            get { return Recipient?.Split(Deliminator) ?? Array.Empty<string>(); }
            set { Recipient = string.Join(Deliminator.ToString(), value); }
        }

        [Index, Required]
        public string From { get; set; }

        [Required]
        public string Eml { get; set; }
    }
}