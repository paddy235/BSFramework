namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyHumanDangerTrainingAddStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("wg_traininguser", "Status", c => c.Int(nullable: false));
            AddColumn("wg_traininguser", "TrainingTime", c => c.DateTime(precision: 0));
        }
        
        public override void Down()
        {
            DropColumn("wg_traininguser", "TrainingTime");
            DropColumn("wg_traininguser", "Status");
        }
    }
}
