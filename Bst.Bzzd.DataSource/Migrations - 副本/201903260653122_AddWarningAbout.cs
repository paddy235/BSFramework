namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWarningAbout : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.base_warningconfig",
                c => new
                    {
                        ConfigId = c.Guid(nullable: false),
                        Assembly = c.String(maxLength: 200, storeType: "nvarchar"),
                        MessageKey = c.String(maxLength: 30, storeType: "nvarchar"),
                        Enabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ConfigId);
            
            CreateTable(
                "dbo.wg_warning",
                c => new
                    {
                        WarningId = c.Guid(nullable: false),
                        BusinessId = c.String(maxLength: 36, storeType: "nvarchar"),
                        MessageKey = c.String(maxLength: 30, storeType: "nvarchar"),
                        IsPublished = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.WarningId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.wg_warning");
            DropTable("dbo.base_warningconfig");
        }
    }
}
