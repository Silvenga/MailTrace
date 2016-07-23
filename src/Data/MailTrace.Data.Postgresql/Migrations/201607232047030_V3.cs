namespace MailTrace.Data.Postgresql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("public.EmailProperties", "LogId", c => c.String(nullable: false, maxLength: 32));
        }
        
        public override void Down()
        {
            DropColumn("public.EmailProperties", "LogId");
        }
    }
}
