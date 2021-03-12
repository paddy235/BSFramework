namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ModifyReportAddComments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.wg_reportcomment",
                c => new
                {
                    CommentId = c.Guid(nullable: false),
                    Content = c.String(maxLength: 500, storeType: "nvarchar"),
                    CommentUserId = c.String(maxLength: 36, storeType: "nvarchar"),
                    CommentUser = c.String(maxLength: 30, storeType: "nvarchar"),
                    ReportId = c.Guid(nullable: false),
                    CommentTime = c.DateTime(nullable: false, precision: 0),
                })
                .ForeignKey("dbo.wg_report", t => t.ReportId, cascadeDelete: true)
                .Index(t => t.ReportId);
        }

        public override void Down()
        {
            DropForeignKey("wg_reportcomment", "ReportId", "wg_report");
            DropIndex("dbo.wg_reportcomment", new[] { "ReportId" });
            DropTable("dbo.wg_reportcomment");
        }
    }
}
