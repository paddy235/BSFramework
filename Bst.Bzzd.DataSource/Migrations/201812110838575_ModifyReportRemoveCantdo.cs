namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyReportRemoveCantdo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.wg_report", "Undo", c => c.String(maxLength: 2000, storeType: "nvarchar"));
            DropColumn("dbo.wg_report", "Cantdo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.wg_report", "Cantdo", c => c.String(maxLength: 2000, storeType: "nvarchar"));
            DropColumn("dbo.wg_report", "Undo");
        }
    }
}
