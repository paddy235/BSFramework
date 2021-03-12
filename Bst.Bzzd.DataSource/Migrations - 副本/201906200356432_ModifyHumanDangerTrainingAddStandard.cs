namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ModifyHumanDangerTrainingAddStandard : DbMigration
    {
        public override void Up()
        {
            AddColumn("wg_trainingmeasure", "Standard", c => c.String(maxLength: 500, storeType: "nvarchar"));
        }

        public override void Down()
        {
            DropColumn("wg_trainingmeasure", "Standard");
        }
    }
}
