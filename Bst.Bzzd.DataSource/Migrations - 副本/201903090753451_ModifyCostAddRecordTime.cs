namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyCostAddRecordTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.wg_costrecord", "RecordTime", c => c.DateTime(nullable: false, precision: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.wg_costrecord", "RecordTime");
        }
    }
}
