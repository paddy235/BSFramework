namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CostRecordprofessional : DbMigration
    {
        public override void Up()
        {
            AddColumn("wg_costrecord", "professional", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("wg_costrecord", "professional");
        }
    }
}
