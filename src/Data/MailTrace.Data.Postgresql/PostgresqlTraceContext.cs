namespace MailTrace.Data.Postgresql
{
    using System.Data.Entity;

    using MailTrace.Host.Data;

    public class PostgresqlTraceContext : TraceContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");
            base.OnModelCreating(modelBuilder);
        }
    }
}