namespace MailTrace.Data
{
    using System;
    using System.Data.Common;
    using System.Data.Entity;

    using MailTrace.Data.Entities;

    public class TraceContext : DbContext
    {
        public TraceContext() : base(nameof(TraceContext))
        {
        }

        public TraceContext(DbConnection connection) : base(connection, false)
        {
        }

        public DbSet<Log> Logs { get; set; }

        public DbSet<Email> Emails { get; set; }

        public DbSet<EmailLog> EmailLogs { get; set; }

        public DbSet<EmailMeta> EmailMetas { get; set; }

        public virtual void Migrate()
        {
            throw new NotImplementedException();
        }
    }
}