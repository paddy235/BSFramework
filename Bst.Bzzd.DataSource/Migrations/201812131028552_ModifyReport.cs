namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.wg_report", "IsSubmit", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.wg_report", "IsSubmit");
        }
    }
}
