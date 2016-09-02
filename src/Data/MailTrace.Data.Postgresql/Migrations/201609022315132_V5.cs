namespace MailTrace.Data.Postgresql.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class V5 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "public.EmailProperties", newName: "EmailLogs");

            CreateTable(
                    "public.EmailMetas",
                    c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(nullable: false),
                        Value = c.String(),
                        Email_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                 .ForeignKey("public.Emails", t => t.Email_Id, cascadeDelete: true)
                 .Index(t => t.Key)
                 .Index(t => t.Value)
                 .Index(t => t.Email_Id);

            DropIndex("public.Emails", new[] {"To"});
            RenameColumn("public.Emails", "To", "Recipient");
            CreateIndex("public.Emails", "Recipient");
        }

        public override void Down()
        {
            DropForeignKey("public.EmailMetas", "Email_Id", "public.Emails");

            DropIndex("public.Emails", new[] {"Recipient"});
            RenameColumn("public.Emails", "Recipient", "To");
            CreateIndex("public.Emails", "To");

            DropIndex("public.EmailMetas", new[] {"Email_Id"});
            DropIndex("public.EmailMetas", new[] {"Value"});
            DropIndex("public.EmailMetas", new[] {"Key"});
            DropTable("public.EmailMetas");

            RenameTable(name: "public.EmailLogs", newName: "EmailProperties");
        }
    }
}