namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddHumanDanger : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.wg_humandanger",
                c => new
                {
                    HumanDangerId = c.Guid(nullable: false),
                    Task = c.String(maxLength: 500, storeType: "nvarchar"),
                    TaskArea = c.String(maxLength: 500, storeType: "nvarchar"),
                    DeptId = c.String(maxLength: 2000, storeType: "nvarchar"),
                    DeptName = c.String(maxLength: 2000, storeType: "nvarchar"),
                    TaskType = c.String(maxLength: 2000, storeType: "nvarchar"),
                    DangerLevel = c.String(maxLength: 200, storeType: "nvarchar"),
                    OtherMeasure = c.String(maxLength: 500, storeType: "nvarchar"),
                    OperateUser = c.String(maxLength: 50, storeType: "nvarchar"),
                    OperateUserId = c.String(maxLength: 36, storeType: "nvarchar"),
                    OperateTime = c.DateTime(nullable: false, precision: 0),
                })
                .PrimaryKey(t => t.HumanDangerId);

            CreateTable(
                "dbo.wg_humandangermeasure",
                c => new
                {
                    HumanDangerMeasureId = c.Guid(nullable: false),
                    CategoryId = c.Guid(nullable: false),
                    Category = c.String(maxLength: 200, storeType: "nvarchar"),
                    MeasureId = c.Guid(nullable: false),
                    DangerReason = c.String(maxLength: 500, storeType: "nvarchar"),
                    MeasureContent = c.String(maxLength: 500, storeType: "nvarchar"),
                    HumanDangerId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.HumanDangerMeasureId)
                .ForeignKey("dbo.wg_humandanger", t => t.HumanDangerId, cascadeDelete: true)
                .Index(t => t.HumanDangerId);

        }

        public override void Down()
        {
            DropForeignKey("wg_humandangermeasure", "HumanDangerId", "wg_humandanger");
            DropIndex("wg_humandangermeasure", new[] { "HumanDangerId" });
            DropTable("wg_humandangermeasure");
            DropTable("wg_humandanger");
        }
    }
}
