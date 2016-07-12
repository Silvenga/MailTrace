namespace MailTrace.Host.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V0 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "public.EmailProperties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Host = c.String(),
                        QueueId = c.String(),
                        Key = c.String(),
                        Value = c.String(),
                        SourceTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Host)
                .Index(t => t.QueueId)
                .Index(t => t.Key);
            
            CreateTable(
                "public.Emails",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Host = c.String(),
                        QueueId = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Host)
                .Index(t => t.QueueId);
            
            CreateTable(
                "public.Logs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Host = c.String(),
                        QueueId = c.String(),
                        Message = c.String(),
                        Processed = c.Boolean(nullable: false),
                        SourceTime = c.DateTime(nullable: false),
                        Timestamp = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropIndex("public.Emails", new[] { "QueueId" });
            DropIndex("public.Emails", new[] { "Host" });
            DropIndex("public.EmailProperties", new[] { "Key" });
            DropIndex("public.EmailProperties", new[] { "QueueId" });
            DropIndex("public.EmailProperties", new[] { "Host" });
            DropTable("public.Logs");
            DropTable("public.Emails");
            DropTable("public.EmailProperties");
        }
    }
}
