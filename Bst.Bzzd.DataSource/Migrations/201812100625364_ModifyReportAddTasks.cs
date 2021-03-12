namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ModifyReportAddTasks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.wg_report", "Tasks", c => c.String(maxLength: 2000, storeType: "nvarchar"));
            AddColumn("dbo.wg_report", "Plan", c => c.String(maxLength: 2000, storeType: "nvarchar"));
            AddColumn("dbo.wg_report", "Cantdo", c => c.String(maxLength: 2000, storeType: "nvarchar"));
            AddColumn("dbo.wg_report", "StartTime", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.wg_report", "EndTime", c => c.DateTime(nullable: false, precision: 0));
            this.Sql("update wg_report set starttime = now(), endtime = now()");
        }

        public override void Down()
        {
            DropColumn("dbo.wg_report", "EndTime");
            DropColumn("dbo.wg_report", "StartTime");
            DropColumn("dbo.wg_report", "Cantdo");
            DropColumn("dbo.wg_report", "Plan");
            DropColumn("dbo.wg_report", "Tasks");
        }
    }
}
