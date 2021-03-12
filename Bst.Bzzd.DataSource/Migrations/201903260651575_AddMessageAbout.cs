namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMessageAbout : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.base_messageconfig",
                c => new
                    {
                        ConfigId = c.Guid(nullable: false),
                        ConfigKey = c.String(maxLength: 30, storeType: "nvarchar"),
                        Enabled = c.Boolean(nullable: false),
                        Title = c.String(maxLength: 200, storeType: "nvarchar"),
                        Template = c.String(maxLength: 500, storeType: "nvarchar"),
                        Category = c.Int(nullable: false),
                        RecieveType = c.String(maxLength: 50, storeType: "nvarchar"),
                        Assembly = c.String(maxLength: 200, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.ConfigId)
                .Index(t => t.ConfigKey, unique: true);
            
            CreateTable(
                "dbo.wg_message",
                c => new
                    {
                        MessageId = c.Guid(nullable: false),
                        Title = c.String(maxLength: 500, storeType: "nvarchar"),
                        Content = c.String(maxLength: 2000, storeType: "nvarchar"),
                        UserId = c.String(maxLength: 36, storeType: "nvarchar"),
                        BusinessId = c.String(maxLength: 36, storeType: "nvarchar"),
                        IsFinished = c.Boolean(nullable: false),
                        HasReaded = c.Boolean(nullable: false),
                        Category = c.Int(nullable: false),
                        MessageKey = c.String(maxLength: 30, storeType: "nvarchar"),
                        CreateTime = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.MessageId)
                .Index(t => t.UserId, clustered: true)
                .Index(t => t.BusinessId, clustered: true)
                .Index(t => t.MessageKey, clustered: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.wg_message", new[] { "MessageKey" });
            DropIndex("dbo.wg_message", new[] { "BusinessId" });
            DropIndex("dbo.wg_message", new[] { "UserId" });
            DropIndex("dbo.base_messageconfig", new[] { "ConfigKey" });
            DropTable("dbo.wg_message");
            DropTable("dbo.base_messageconfig");
        }
    }
}
