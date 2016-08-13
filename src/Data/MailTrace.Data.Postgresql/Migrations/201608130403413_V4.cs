namespace MailTrace.Data.Postgresql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V4 : DbMigration
    {
        public override void Up()
        {
            DropIndex("public.Emails", new[] { "MessageId" });
            AddColumn("public.Emails", "To", c => c.String(nullable: false));
            AddColumn("public.Emails", "From", c => c.String(nullable: false));
            AddColumn("public.Emails", "Eml", c => c.String(nullable: false));
            AlterColumn("public.Emails", "MessageId", c => c.String(nullable: false));
            CreateIndex("public.Emails", "MessageId");
            CreateIndex("public.Emails", "To");
            CreateIndex("public.Emails", "From");
        }
        
        public override void Down()
        {
            DropIndex("public.Emails", new[] { "From" });
            DropIndex("public.Emails", new[] { "To" });
            DropIndex("public.Emails", new[] { "MessageId" });
            AlterColumn("public.Emails", "MessageId", c => c.String());
            DropColumn("public.Emails", "Eml");
            DropColumn("public.Emails", "From");
            DropColumn("public.Emails", "To");
            CreateIndex("public.Emails", "MessageId");
        }
    }
}
