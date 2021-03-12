namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyCostAddIndex : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.wg_costitem", "DeptId", clustered: true);
            CreateIndex("dbo.wg_costrecord", "DeptId", clustered: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.wg_costrecord", new[] { "DeptId" });
            DropIndex("dbo.wg_costitem", new[] { "DeptId" });
        }
    }
}
