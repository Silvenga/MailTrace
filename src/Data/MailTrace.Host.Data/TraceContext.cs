namespace MailTrace.Host.Data
{
    using System.Data.Common;
    using System.Data.Entity;

    using MailTrace.Host.Data.Entities;

    public class TraceContext : DbContext
    {
        protected TraceContext() : base(nameof(TraceContext))
        {
        }

        public TraceContext(DbConnection connection) : base(connection, false)
        {
        }

        public DbSet<Log> Logs { get; set; }

        public DbSet<Email> Emails { get; set; }

        public DbSet<EmailProperty> EmailProperties { get; set; }
    }
}