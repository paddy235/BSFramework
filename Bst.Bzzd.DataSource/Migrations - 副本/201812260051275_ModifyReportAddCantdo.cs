namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyReportAddCantdo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.wg_report", "Cantdo", c => c.String(maxLength: 2000, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.wg_report", "Cantdo");
        }
    }
}
