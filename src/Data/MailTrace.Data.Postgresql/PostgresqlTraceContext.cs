namespace MailTrace.Data.Postgresql
{
    using System.Data.Entity;
    using System.Data.Entity.Migrations;

    using MailTrace.Data.Postgresql.Migrations;
    using MailTrace.Data;

    public class PostgresqlTraceContext : TraceContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");
            base.OnModelCreating(modelBuilder);
        }

        public override void Migrate()
        {
            var migratorConfig = new Configuration();
            var dbMigrator = new DbMigrator(migratorConfig);
            dbMigrator.Update();
        }
    }
}