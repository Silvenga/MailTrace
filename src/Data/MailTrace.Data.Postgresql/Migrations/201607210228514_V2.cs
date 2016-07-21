namespace MailTrace.Data.Postgresql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("public.EmailProperties", "IX__Host_QueueId");
            DropIndex("public.EmailProperties", new[] { "Key" });
            DropIndex("public.EmailProperties", new[] { "Value" });
            AlterColumn("public.EmailProperties", "Host", c => c.String(nullable: false));
            AlterColumn("public.EmailProperties", "QueueId", c => c.String(nullable: false));
            AlterColumn("public.EmailProperties", "Key", c => c.String(nullable: false));
            AlterColumn("public.EmailProperties", "Value", c => c.String(nullable: false));
            CreateIndex("public.EmailProperties", new[] { "Host", "QueueId" }, name: "IX__Host_QueueId");
            CreateIndex("public.EmailProperties", "Key");
            CreateIndex("public.EmailProperties", "Value");
        }
        
        public override void Down()
        {
            DropIndex("public.EmailProperties", new[] { "Value" });
            DropIndex("public.EmailProperties", new[] { "Key" });
            DropIndex("public.EmailProperties", "IX__Host_QueueId");
            AlterColumn("public.EmailProperties", "Value", c => c.String());
            AlterColumn("public.EmailProperties", "Key", c => c.String());
            AlterColumn("public.EmailProperties", "QueueId", c => c.String());
            AlterColumn("public.EmailProperties", "Host", c => c.String());
            CreateIndex("public.EmailProperties", "Value");
            CreateIndex("public.EmailProperties", "Key");
            CreateIndex("public.EmailProperties", new[] { "Host", "QueueId" }, name: "IX__Host_QueueId");
        }
    }
}
