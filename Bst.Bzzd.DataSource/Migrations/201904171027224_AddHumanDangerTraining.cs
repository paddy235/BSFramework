namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddHumanDangerTraining : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "wg_humandangertraining",
                c => new
                {
                    TrainingId = c.Guid(nullable: false),
                    TraningTask = c.String(maxLength: 500, storeType: "nvarchar"),
                    HumanDangerId = c.Guid(),
                    CreateTime = c.DateTime(nullable: false, precision: 0),
                    CreateUserId = c.String(maxLength: 36, storeType: "nvarchar"),
                    DeptId = c.String(maxLength: 36, storeType: "nvarchar"),
                    DeptName = c.String(maxLength: 50, storeType: "nvarchar"),
                    MeetingJobId = c.String(maxLength: 36, storeType: "nvarchar"),
                })
                .PrimaryKey(t => t.TrainingId)
                .Index(t => t.HumanDangerId)
                .Index(t => t.DeptId);

            CreateTable(
                "wg_traininguser",
                c => new
                {
                    TrainingUserId = c.Guid(nullable: false),
                    UserId = c.String(maxLength: 36, storeType: "nvarchar"),
                    UserName = c.String(maxLength: 100, storeType: "nvarchar"),
                    TrainingPlace = c.String(maxLength: 500, storeType: "nvarchar"),
                    No = c.String(maxLength: 50, storeType: "nvarchar"),
                    DangerLevel = c.String(maxLength: 20, storeType: "nvarchar"),
                    OtherMeasure = c.String(maxLength: 500, storeType: "nvarchar"),
                    TrainingRole = c.Int(nullable: false),
                    IsDone = c.Boolean(nullable: false),
                    IsMarked = c.Boolean(nullable: false),
                    IsEvaluated = c.Boolean(nullable: false),
                    TrainingId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.TrainingUserId)
                .ForeignKey("wg_humandangertraining", t => t.TrainingId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.TrainingId);

            CreateTable(
                "wg_trainingmeasure",
                c => new
                {
                    TrainingMeasureId = c.Guid(nullable: false),
                    DangerReason = c.String(maxLength: 500, storeType: "nvarchar"),
                    MeasureContent = c.String(maxLength: 500, storeType: "nvarchar"),
                    MeasureId = c.Guid(),
                    CategoryId = c.Guid(nullable: false),
                    Category = c.String(maxLength: 200, storeType: "nvarchar"),
                    State = c.Int(nullable: false),
                    TrainingUserId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.TrainingMeasureId)
                .ForeignKey("wg_traininguser", t => t.TrainingUserId, cascadeDelete: true)
                .Index(t => t.TrainingUserId);

            CreateTable(
                "wg_trainingtype",
                c => new
                {
                    TaskTypeId = c.Guid(nullable: false),
                    TypeName = c.String(maxLength: 200, storeType: "nvarchar"),
                    State = c.Int(nullable: false),
                    TrainingUserId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.TaskTypeId)
                .ForeignKey("wg_traininguser", t => t.TrainingUserId, cascadeDelete: true)
                .Index(t => t.TrainingUserId);

        }

        public override void Down()
        {
            DropForeignKey("wg_traininguser", "TrainingId", "wg_humandangertraining");
            DropForeignKey("wg_trainingtype", "TrainingUserId", "wg_traininguser");
            DropForeignKey("wg_trainingmeasure", "TrainingUserId", "wg_traininguser");
            DropIndex("wg_trainingtype", new[] { "TrainingUserId" });
            DropIndex("wg_trainingmeasure", new[] { "TrainingUserId" });
            DropIndex("wg_traininguser", new[] { "TrainingId" });
            DropIndex("wg_traininguser", new[] { "UserId" });
            DropIndex("wg_humandangertraining", new[] { "DeptId" });
            DropIndex("wg_humandangertraining", new[] { "HumanDangerId" });
            DropTable("wg_trainingtype");
            DropTable("wg_trainingmeasure");
            DropTable("wg_traininguser");
            DropTable("wg_humandangertraining");
        }
    }
}
