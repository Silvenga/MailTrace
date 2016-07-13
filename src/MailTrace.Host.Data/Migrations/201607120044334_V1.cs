namespace MailTrace.Host.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("public.EmailProperties", new[] { "Host" });
            DropIndex("public.EmailProperties", new[] { "QueueId" });
            DropIndex("public.Emails", new[] { "Host" });
            DropIndex("public.Emails", new[] { "QueueId" });
            AddColumn("public.Emails", "MessageId", c => c.String());
            CreateIndex("public.EmailProperties", new[] { "Host", "QueueId" }, name: "IX__Host_QueueId");
            CreateIndex("public.EmailProperties", "Value");
            CreateIndex("public.EmailProperties", "SourceTime");
            CreateIndex("public.Emails", "MessageId");
            DropColumn("public.Emails", "Host");
            DropColumn("public.Emails", "QueueId");
        }
        
        public override void Down()
        {
            AddColumn("public.Emails", "QueueId", c => c.String());
            AddColumn("public.Emails", "Host", c => c.String());
            DropIndex("public.Emails", new[] { "MessageId" });
            DropIndex("public.EmailProperties", new[] { "SourceTime" });
            DropIndex("public.EmailProperties", new[] { "Value" });
            DropIndex("public.EmailProperties", "IX__Host_QueueId");
            DropColumn("public.Emails", "MessageId");
            CreateIndex("public.Emails", "QueueId");
            CreateIndex("public.Emails", "Host");
            CreateIndex("public.EmailProperties", "QueueId");
            CreateIndex("public.EmailProperties", "Host");
        }
    }
}
