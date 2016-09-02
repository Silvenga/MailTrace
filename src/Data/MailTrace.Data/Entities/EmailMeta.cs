namespace MailTrace.Data.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class EmailMeta
    {
        public int Id { get; set; }

        [Required]
        public Email Email { get; set; }

        [Required, Index]
        public string Key { get; set; }

        [Index]
        public string Value { get; set; }
    }
}