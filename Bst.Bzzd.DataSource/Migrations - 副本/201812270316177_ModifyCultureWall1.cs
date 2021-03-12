namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyCultureWall1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.wg_culturewall", "summarydate", c => c.DateTime(precision: 0));
            AddColumn("dbo.wg_culturewall", "slogandate", c => c.DateTime(precision: 0));
            AddColumn("dbo.wg_culturewall", "visiondate", c => c.DateTime(precision: 0));
            AddColumn("dbo.wg_culturewall", "conceptdate", c => c.DateTime(precision: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.wg_culturewall", "conceptdate");
            DropColumn("dbo.wg_culturewall", "visiondate");
            DropColumn("dbo.wg_culturewall", "slogandate");
            DropColumn("dbo.wg_culturewall", "summarydate");
        }
    }
}
