namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplyMysql : DbMigration
    {
        public override void Up()
        {
            AlterColumn("WG_DANGERCATEGORY", "CATEGORYID", c => c.String(nullable: false, maxLength: 36, storeType: "nvarchar"));
            AlterColumn("WG_DANGERCATEGORY", "PARENTCATEGORYID", c => c.String(maxLength: 36, storeType: "nvarchar"));
            AlterColumn("WG_DANGERMEASURE", "MEASUREID", c => c.String(nullable: false, maxLength: 36, storeType: "nvarchar"));
            AlterColumn("WG_DANGERMEASURE", "CATEGORYID", c => c.String(maxLength: 36, storeType: "nvarchar"));
            AlterColumn("WG_HUMANDANGER", "HUMANDANGERID", c => c.String(nullable: false, maxLength: 36, storeType: "nvarchar"));
            AlterColumn("WG_HUMANDANGER", "DEPTID", c => c.String(maxLength: 2000, storeType: "nvarchar"));
            AlterColumn("WG_HUMANDANGER", "DEPTNAME", c => c.String(maxLength: 2000, storeType: "nvarchar"));
            AlterColumn("WG_HUMANDANGERMEASURE", "HUMANDANGERMEASUREID", c => c.String(nullable: false, maxLength: 36, storeType: "nvarchar"));
            AlterColumn("WG_HUMANDANGERMEASURE", "CATEGORYID", c => c.String(maxLength: 36, storeType: "nvarchar"));
            AlterColumn("WG_HUMANDANGERMEASURE", "MEASUREID", c => c.String(maxLength: 36, storeType: "nvarchar"));
            AlterColumn("WG_HUMANDANGERMEASURE", "HUMANDANGERID", c => c.String(maxLength: 36, storeType: "nvarchar"));
            AlterColumn("WG_HUMANDANGERTRAINING", "TRAININGID", c => c.String(nullable: false, maxLength: 36, storeType: "nvarchar"));
            AlterColumn("WG_HUMANDANGERTRAINING", "HUMANDANGERID", c => c.String(maxLength: 36, storeType: "nvarchar"));
            AlterColumn("WG_TRAININGUSER", "TRAININGUSERID", c => c.String(nullable: false, maxLength: 36, storeType: "nvarchar"));
            AlterColumn("WG_TRAININGUSER", "TRAININGID", c => c.String(maxLength: 36, storeType: "nvarchar"));
            AlterColumn("WG_TRAININGMEASURE", "MEASUREID", c => c.String(maxLength: 36, storeType: "nvarchar"));
            AlterColumn("WG_TRAININGMEASURE", "CATEGORYID", c => c.String(maxLength: 36, storeType: "nvarchar"));
            AlterColumn("WG_TRAININGMEASURE", "TRAININGUSERID", c => c.String(maxLength: 36, storeType: "nvarchar"));
            AlterColumn("WG_TRAININGTYPE", "TRAININGUSERID", c => c.String(maxLength: 36, storeType: "nvarchar"));
            DropPrimaryKey("WG_DANGERCATEGORY");
            DropPrimaryKey("WG_DANGERMEASURE");
            DropPrimaryKey("WG_HUMANDANGER");
            DropPrimaryKey("WG_HUMANDANGERMEASURE");
            DropPrimaryKey("WG_HUMANDANGERTRAINING");
            DropPrimaryKey("WG_TRAININGUSER");
            DropForeignKey("wg_dangermeasure", "CategoryId", "wg_dangercategory");
            DropForeignKey("wg_humandangermeasure", "HumanDangerId", "wg_humandanger");
            DropForeignKey("wg_traininguser", "TrainingId", "wg_humandangertraining");
            DropForeignKey("wg_trainingmeasure", "TrainingUserId", "wg_traininguser");
            DropForeignKey("wg_trainingtype", "TrainingUserId", "wg_traininguser");
            DropForeignKey("wg_dangercategory", "ParentCategoryId", "wg_dangercategory");
            DropIndex("WG_BUDGETDETAIL", new[] { "BudgetId" });
            DropIndex("WG_COSTITEM", new[] { "CostRecordId" });
            DropIndex("WG_DANGERCATEGORY", new[] { "ParentCategoryId" });
            DropIndex("WG_DANGERMEASURE", new[] { "CategoryId" });
            DropIndex("WG_HUMANDANGERMEASURE", new[] { "HumanDangerId" });
            DropIndex("WG_HUMANDANGERTRAINING", new[] { "HumanDangerId" });
            DropIndex("WG_TRAININGUSER", new[] { "TrainingId" });
            DropIndex("WG_TRAININGMEASURE", new[] { "TrainingUserId" });
            DropIndex("WG_TRAININGTYPE", new[] { "TrainingUserId" });
            DropIndex("WG_REPORTCOMMENT", new[] { "ReportId" });
            DropIndex("WG_REPORTNOTICE", new[] { "ReportId" });
            AddPrimaryKey("WG_DANGERCATEGORY", "CATEGORYID");
            AddPrimaryKey("WG_DANGERMEASURE", "MEASUREID");
            AddPrimaryKey("WG_HUMANDANGER", "HUMANDANGERID");
            AddPrimaryKey("WG_HUMANDANGERMEASURE", "HUMANDANGERMEASUREID");
            AddPrimaryKey("WG_HUMANDANGERTRAINING", "TRAININGID");
            AddPrimaryKey("WG_TRAININGUSER", "TRAININGUSERID");
            CreateIndex("WG_BUDGETDETAIL", "BUDGETID");
            CreateIndex("WG_COSTITEM", "COSTRECORDID");
            CreateIndex("WG_DANGERCATEGORY", "PARENTCATEGORYID");
            CreateIndex("WG_DANGERMEASURE", "CATEGORYID");
            CreateIndex("WG_HUMANDANGERMEASURE", "HUMANDANGERID");
            CreateIndex("WG_HUMANDANGERTRAINING", "HUMANDANGERID", clustered: true);
            CreateIndex("WG_TRAININGUSER", "TRAININGID");
            CreateIndex("WG_TRAININGMEASURE", "TRAININGUSERID");
            CreateIndex("WG_TRAININGTYPE", "TRAININGUSERID");
            CreateIndex("WG_REPORTCOMMENT", "REPORTID");
            CreateIndex("WG_REPORTNOTICE", "REPORTID");
            AddForeignKey("WG_DANGERMEASURE", "CATEGORYID", "WG_DANGERCATEGORY", "CATEGORYID");
            AddForeignKey("WG_HUMANDANGERMEASURE", "HUMANDANGERID", "WG_HUMANDANGER", "HUMANDANGERID");
            AddForeignKey("WG_TRAININGUSER", "TRAININGID", "WG_HUMANDANGERTRAINING", "TRAININGID");
            AddForeignKey("WG_TRAININGMEASURE", "TRAININGUSERID", "WG_TRAININGUSER", "TRAININGUSERID");
            AddForeignKey("WG_TRAININGTYPE", "TRAININGUSERID", "WG_TRAININGUSER", "TRAININGUSERID");
            AddForeignKey("WG_DANGERCATEGORY", "PARENTCATEGORYID", "WG_DANGERCATEGORY", "CATEGORYID");
        }
        
        public override void Down()
        {
            DropForeignKey("WG_DANGERCATEGORY", "PARENTCATEGORYID", "WG_DANGERCATEGORY");
            DropForeignKey("WG_TRAININGTYPE", "TRAININGUSERID", "WG_TRAININGUSER");
            DropForeignKey("WG_TRAININGMEASURE", "TRAININGUSERID", "WG_TRAININGUSER");
            DropForeignKey("WG_TRAININGUSER", "TRAININGID", "WG_HUMANDANGERTRAINING");
            DropForeignKey("WG_HUMANDANGERMEASURE", "HUMANDANGERID", "WG_HUMANDANGER");
            DropForeignKey("WG_DANGERMEASURE", "CATEGORYID", "WG_DANGERCATEGORY");
            DropIndex("WG_REPORTNOTICE", new[] { "REPORTID" });
            DropIndex("WG_REPORTCOMMENT", new[] { "REPORTID" });
            DropIndex("WG_TRAININGTYPE", new[] { "TRAININGUSERID" });
            DropIndex("WG_TRAININGMEASURE", new[] { "TRAININGUSERID" });
            DropIndex("WG_TRAININGUSER", new[] { "TRAININGID" });
            DropIndex("WG_HUMANDANGERTRAINING", new[] { "HUMANDANGERID" });
            DropIndex("WG_HUMANDANGERMEASURE", new[] { "HUMANDANGERID" });
            DropIndex("WG_DANGERMEASURE", new[] { "CATEGORYID" });
            DropIndex("WG_DANGERCATEGORY", new[] { "PARENTCATEGORYID" });
            DropIndex("WG_COSTITEM", new[] { "COSTRECORDID" });
            DropIndex("WG_BUDGETDETAIL", new[] { "BUDGETID" });
            DropPrimaryKey("WG_TRAININGUSER");
            DropPrimaryKey("WG_HUMANDANGERTRAINING");
            DropPrimaryKey("WG_HUMANDANGERMEASURE");
            DropPrimaryKey("WG_HUMANDANGER");
            DropPrimaryKey("WG_DANGERMEASURE");
            DropPrimaryKey("WG_DANGERCATEGORY");
            CreateIndex("WG_REPORTNOTICE", "ReportId");
            CreateIndex("WG_REPORTCOMMENT", "ReportId");
            CreateIndex("WG_TRAININGTYPE", "TrainingUserId");
            CreateIndex("WG_TRAININGMEASURE", "TrainingUserId");
            CreateIndex("WG_TRAININGUSER", "TrainingId");
            CreateIndex("WG_HUMANDANGERTRAINING", "HumanDangerId", clustered: true);
            CreateIndex("WG_HUMANDANGERMEASURE", "HumanDangerId");
            CreateIndex("WG_DANGERMEASURE", "CategoryId");
            CreateIndex("WG_DANGERCATEGORY", "ParentCategoryId");
            CreateIndex("WG_COSTITEM", "CostRecordId");
            CreateIndex("WG_BUDGETDETAIL", "BudgetId");
            AddForeignKey("wg_dangercategory", "ParentCategoryId", "wg_dangercategory", "CategoryId");
            AddForeignKey("wg_trainingtype", "TrainingUserId", "wg_traininguser", "TrainingUserId", cascadeDelete: true);
            AddForeignKey("wg_trainingmeasure", "TrainingUserId", "wg_traininguser", "TrainingUserId", cascadeDelete: true);
            AddForeignKey("wg_traininguser", "TrainingId", "wg_humandangertraining", "TrainingId", cascadeDelete: true);
            AddForeignKey("wg_humandangermeasure", "HumanDangerId", "wg_humandanger", "HumanDangerId", cascadeDelete: true);
            AddForeignKey("wg_dangermeasure", "CategoryId", "wg_dangercategory", "CategoryId", cascadeDelete: true);
            AddPrimaryKey("WG_TRAININGUSER", "TrainingUserId");
            AddPrimaryKey("WG_HUMANDANGERTRAINING", "TrainingId");
            AddPrimaryKey("WG_HUMANDANGERMEASURE", "HumanDangerMeasureId");
            AddPrimaryKey("WG_HUMANDANGER", "HumanDangerId");
            AddPrimaryKey("WG_DANGERMEASURE", "MeasureId");
            AddPrimaryKey("WG_DANGERCATEGORY", "CategoryId");
            AlterColumn("WG_TRAININGTYPE", "TRAININGUSERID", c => c.Guid(nullable: false));
            AlterColumn("WG_TRAININGMEASURE", "TRAININGUSERID", c => c.Guid(nullable: false));
            AlterColumn("WG_TRAININGMEASURE", "CATEGORYID", c => c.Guid(nullable: false));
            AlterColumn("WG_TRAININGMEASURE", "MEASUREID", c => c.Guid());
            AlterColumn("WG_TRAININGUSER", "TRAININGID", c => c.Guid(nullable: false));
            AlterColumn("WG_TRAININGUSER", "TRAININGUSERID", c => c.Guid(nullable: false));
            AlterColumn("WG_HUMANDANGERTRAINING", "HUMANDANGERID", c => c.Guid());
            AlterColumn("WG_HUMANDANGERTRAINING", "TRAININGID", c => c.Guid(nullable: false));
            AlterColumn("WG_HUMANDANGERMEASURE", "HUMANDANGERID", c => c.Guid(nullable: false));
            AlterColumn("WG_HUMANDANGERMEASURE", "MEASUREID", c => c.Guid(nullable: false));
            AlterColumn("WG_HUMANDANGERMEASURE", "CATEGORYID", c => c.Guid(nullable: false));
            AlterColumn("WG_HUMANDANGERMEASURE", "HUMANDANGERMEASUREID", c => c.Guid(nullable: false));
            AlterColumn("WG_HUMANDANGER", "DEPTNAME", c => c.String(unicode: false, storeType: "text"));
            AlterColumn("WG_HUMANDANGER", "DEPTID", c => c.String(unicode: false, storeType: "text"));
            AlterColumn("WG_HUMANDANGER", "HUMANDANGERID", c => c.Guid(nullable: false));
            AlterColumn("WG_DANGERMEASURE", "CATEGORYID", c => c.Guid(nullable: false));
            AlterColumn("WG_DANGERMEASURE", "MEASUREID", c => c.Guid(nullable: false));
            AlterColumn("WG_DANGERCATEGORY", "PARENTCATEGORYID", c => c.Guid());
            AlterColumn("WG_DANGERCATEGORY", "CATEGORYID", c => c.Guid(nullable: false));
        }
    }
}
