namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBudget : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.wg_budget",
                c => new
                    {
                        BudgetId = c.Guid(nullable: false),
                        DeptId = c.String(maxLength: 36, storeType: "nvarchar"),
                        DeptName = c.String(maxLength: 30, storeType: "nvarchar"),
                        Year = c.String(maxLength: 10, storeType: "nvarchar"),
                        Category = c.String(maxLength: 30, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.BudgetId);
            
            CreateTable(
                "dbo.wg_budgetdetail",
                c => new
                    {
                        DetailId = c.Guid(nullable: false),
                        Month = c.String(maxLength: 2, storeType: "nvarchar"),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BudgetId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.DetailId)
                .ForeignKey("dbo.wg_budget", t => t.BudgetId, cascadeDelete: true)
                .Index(t => t.BudgetId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("wg_budgetdetail", "BudgetId", "wg_budget");
            DropIndex("dbo.wg_budgetdetail", new[] { "BudgetId" });
            DropTable("dbo.wg_budgetdetail");
            DropTable("dbo.wg_budget");
        }
    }
}
