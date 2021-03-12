namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyReportAddNoticeType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.wg_reportnotice", "NoticeType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.wg_reportnotice", "NoticeType");
        }
    }
}
