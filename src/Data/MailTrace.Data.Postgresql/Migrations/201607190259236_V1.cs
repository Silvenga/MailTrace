namespace MailTrace.Data.Postgresql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V1 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("public.EmailProperties", new[] { "Host", "QueueId" }, name: "IX__Host_QueueId");
            CreateIndex("public.EmailProperties", "Key");
            CreateIndex("public.EmailProperties", "Value");
            CreateIndex("public.EmailProperties", "SourceTime");
            CreateIndex("public.Emails", "MessageId");
        }
        
        public override void Down()
        {
            DropIndex("public.Emails", new[] { "MessageId" });
            DropIndex("public.EmailProperties", new[] { "SourceTime" });
            DropIndex("public.EmailProperties", new[] { "Value" });
            DropIndex("public.EmailProperties", new[] { "Key" });
            DropIndex("public.EmailProperties", "IX__Host_QueueId");
        }
    }
}
