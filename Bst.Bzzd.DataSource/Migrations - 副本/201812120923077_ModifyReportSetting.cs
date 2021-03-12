namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyReportSetting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.base_reportsetting", "Start", c => c.String(maxLength: 20, storeType: "nvarchar"));
            AddColumn("dbo.base_reportsetting", "End", c => c.String(maxLength: 20, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.base_reportsetting", "End");
            DropColumn("dbo.base_reportsetting", "Start");
        }
    }
}
