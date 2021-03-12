namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTrainingUserAddTicketId : DbMigration
    {
        public override void Up()
        {
            AddColumn("wg_traininguser", "TicketId", c => c.String(maxLength: 2000, storeType: "nvarchar"));
            AlterColumn("wg_traininguser", "No", c => c.String(maxLength: 2000, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            AlterColumn("wg_traininguser", "No", c => c.String(maxLength: 50, storeType: "nvarchar"));
            DropColumn("wg_traininguser", "TicketId");
        }
    }
}
