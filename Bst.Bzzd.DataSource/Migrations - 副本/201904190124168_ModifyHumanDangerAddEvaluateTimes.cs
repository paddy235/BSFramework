namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ModifyHumanDangerAddEvaluateTimes : DbMigration
    {
        public override void Up()
        {
            AddColumn("wg_humandangertraining", "TrainingTask", c => c.String(maxLength: 500, storeType: "nvarchar"));
            AddColumn("wg_traininguser", "EvaluateTimes", c => c.Int(nullable: false));
            DropColumn("wg_humandangertraining", "TraningTask");
        }

        public override void Down()
        {
            AddColumn("wg_humandangertraining", "TraningTask", c => c.String(maxLength: 500, storeType: "nvarchar"));
            DropColumn("wg_traininguser", "EvaluateTimes");
            DropColumn("wg_humandangertraining", "TrainingTask");
        }
    }
}
