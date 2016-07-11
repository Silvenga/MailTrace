namespace MailTrace.Host.Data
{
    using System.Data.Entity;

    using MailTrace.Host.Data.Entities;

    public class TraceContext : DbContext
    {
        public DbSet<Log> Logs { get; set; }

        public DbSet<Email> Emails { get; set; }

        public DbSet<EmailProperty> EmailProperties { get; set; }
    }
}