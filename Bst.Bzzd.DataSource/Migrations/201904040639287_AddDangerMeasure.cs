namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDangerMeasure : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.wg_dangercategory",
                c => new
                    {
                        CategoryId = c.Guid(nullable: false),
                        CategoryName = c.String(maxLength: 200, storeType: "nvarchar"),
                        ParentCategoryId = c.Guid(),
                    })
                .PrimaryKey(t => t.CategoryId)
                .ForeignKey("dbo.wg_dangercategory", t => t.ParentCategoryId)
                .Index(t => t.ParentCategoryId);
            
            CreateTable(
                "dbo.wg_dangermeasure",
                c => new
                    {
                        MeasureId = c.Guid(nullable: false),
                        DangerReason = c.String(maxLength: 200, storeType: "nvarchar"),
                        MeasureContent = c.String(maxLength: 500, storeType: "nvarchar"),
                        OperateUserId = c.String(maxLength: 36, storeType: "nvarchar"),
                        OperateUser = c.String(maxLength: 30, storeType: "nvarchar"),
                        OperateTime = c.DateTime(nullable: false, precision: 0),
                        CategoryId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.MeasureId)
                .ForeignKey("dbo.wg_dangercategory", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);
                    }
        
        public override void Down()
        {
            DropForeignKey("wg_dangermeasure", "CategoryId", "wg_dangercategory");
            DropForeignKey("wg_dangercategory", "ParentCategoryId", "wg_dangercategory");
            DropIndex("wg_dangermeasure", new[] { "CategoryId" });
            DropIndex("wg_dangercategory", new[] { "ParentCategoryId" });
            DropTable("wg_dangermeasure");
            DropTable("wg_dangercategory");
        }
    }
}
