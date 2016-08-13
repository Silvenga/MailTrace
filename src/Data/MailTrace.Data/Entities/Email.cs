namespace MailTrace.Data.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class Email
    {
        private const char Deliminator = ';';

        public string Id { get; set; }

        [Index, Required]
        public string MessageId { get; set; }

        [Index, Required]
        public string To { get; set; }

        [NotMapped]
        public IList<string> ToList
        {
            get { return To?.Split(Deliminator) ?? Enumerable.Empty<string>().ToArray(); }
            set { To = string.Join(Deliminator.ToString(), value); }
        }

        [Index, Required]
        public string From { get; set; }

        [Required]
        public string Eml { get; set; }
    }
}