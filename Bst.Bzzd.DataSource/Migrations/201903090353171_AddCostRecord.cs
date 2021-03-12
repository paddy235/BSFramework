namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCostRecord : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.wg_costitem",
                c => new
                    {
                        CostItemId = c.Guid(nullable: false),
                        DeptId = c.String(maxLength: 36, storeType: "nvarchar"),
                        DeptName = c.String(maxLength: 30, storeType: "nvarchar"),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostRecordId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.CostItemId)
                .ForeignKey("dbo.wg_costrecord", t => t.CostRecordId, cascadeDelete: true)
                .Index(t => t.CostRecordId);
            
            CreateTable(
                "dbo.wg_costrecord",
                c => new
                    {
                        RecordId = c.Guid(nullable: false),
                        DeptId = c.String(maxLength: 36, storeType: "nvarchar"),
                        DeptName = c.String(maxLength: 30, storeType: "nvarchar"),
                        Category = c.String(maxLength: 20, storeType: "nvarchar"),
                        BudgetAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.RecordId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("wg_costitem", "CostRecordId", "wg_costrecord");
            DropIndex("dbo.wg_costitem", new[] { "CostRecordId" });
            DropTable("dbo.wg_costrecord");
            DropTable("dbo.wg_costitem");
        }
    }
}
