namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyBudgetAddIndex : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.wg_budget", "DeptId", clustered: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.wg_budget", new[] { "DeptId" });
        }
    }
}
