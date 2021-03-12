namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReportSetting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.base_reportsetting",
                c => new
                    {
                        SettingId = c.Guid(nullable: false),
                        SettingName = c.String(maxLength: 200, storeType: "nvarchar"),
                        StartTime = c.DateTime(nullable: false, precision: 0),
                        EndTime = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.SettingId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.base_reportsetting");
        }
    }
}
