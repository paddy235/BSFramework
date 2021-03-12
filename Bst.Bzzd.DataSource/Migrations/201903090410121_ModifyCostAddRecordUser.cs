namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyCostAddRecordUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.wg_costrecord", "RecordUserId", c => c.String(maxLength: 36, storeType: "nvarchar"));
            AddColumn("dbo.wg_costrecord", "RecordUser", c => c.String(maxLength: 30, storeType: "nvarchar"));
            CreateIndex("dbo.wg_costrecord", "RecordUserId", clustered: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.wg_costrecord", new[] { "RecordUserId" });
            DropColumn("dbo.wg_costrecord", "RecordUser");
            DropColumn("dbo.wg_costrecord", "RecordUserId");
        }
    }
}
