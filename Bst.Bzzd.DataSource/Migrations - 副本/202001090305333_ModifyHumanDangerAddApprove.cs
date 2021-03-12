namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyHumanDangerAddApprove : DbMigration
    {
        public override void Up()
        {
            AddColumn("wg_humandanger", "State", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("wg_humandanger", "State");
        }
    }
}
