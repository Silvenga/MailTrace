namespace MailTrace.Data.Postgresql.Migrations
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
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.Emails",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        MessageId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
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
                        Timestamp = c.DateTimeOffset(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("public.Logs");
            DropTable("public.Emails");
            DropTable("public.EmailProperties");
        }
    }
}
