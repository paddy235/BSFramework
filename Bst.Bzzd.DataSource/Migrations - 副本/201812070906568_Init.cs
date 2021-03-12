namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.wg_report",
                c => new
                {
                    ReportId = c.Guid(nullable: false),
                    ReportUserId = c.String(maxLength: 36, storeType: "nvarchar"),
                    ReportUser = c.String(maxLength: 50, storeType: "nvarchar"),
                    ReportDeptId = c.String(maxLength: 36, storeType: "nvarchar"),
                    ReportDeptName = c.String(maxLength: 50, storeType: "nvarchar"),
                    ReportTime = c.DateTime(nullable: false, precision: 0),
                    ReportType = c.Int(nullable: false),
                    ReportContent = c.String(maxLength: 2000, storeType: "nvarchar"),
                })
                .PrimaryKey(t => t.ReportId);

            CreateTable(
                "dbo.wg_reportnotice",
                c => new
                {
                    NoticeId = c.Guid(nullable: false),
                    UserId = c.String(maxLength: 36, storeType: "nvarchar"),
                    UserName = c.String(maxLength: 50, storeType: "nvarchar"),
                    IsRead = c.Boolean(nullable: false),
                    ReportId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.NoticeId)
                .ForeignKey("dbo.wg_report", t => t.ReportId, cascadeDelete: true)
                .Index(t => t.ReportId);

            CreateTable(
                "dbo.sys_scheduler",
                c => new
                {
                    SchedulerId = c.Guid(nullable: false),
                    SchedulerName = c.String(maxLength: 50, storeType: "nvarchar"),
                    Status = c.String(maxLength: 20, storeType: "nvarchar"),
                    NextRunTime = c.DateTime(precision: 0),
                    LastRunTime = c.DateTime(nullable: false, precision: 0),
                    Trigger = c.String(maxLength: 500, storeType: "nvarchar"),
                })
                .PrimaryKey(t => t.SchedulerId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.wg_reportnotice", "ReportId", "dbo.wg_report");
            DropIndex("dbo.wg_reportnotice", new[] { "ReportId" });
            DropTable("dbo.sys_scheduler");
            DropTable("dbo.wg_reportnotice");
            DropTable("dbo.wg_report");
        }
    }
}
