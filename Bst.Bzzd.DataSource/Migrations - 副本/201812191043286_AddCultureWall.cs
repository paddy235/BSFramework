namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCultureWall : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.wg_culturewall",
                c => new
                    {
                        wallinfoid = c.Guid(nullable: false),
                        departmentid = c.String(maxLength: 36, storeType: "nvarchar"),
                        departmentname = c.String(maxLength: 100, storeType: "nvarchar"),
                        summary = c.String(unicode: false),
                        slogan = c.String(unicode: false),
                        vision = c.String(unicode: false),
                        concept = c.String(unicode: false),
                        createtime = c.DateTime(precision: 0),
                        createuserid = c.String(maxLength: 50, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.wallinfoid);
            
            AlterColumn("dbo.base_reportsetting", "Start", c => c.Int(nullable: false));
            AlterColumn("dbo.base_reportsetting", "End", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.base_reportsetting", "End", c => c.String(maxLength: 20, storeType: "nvarchar"));
            AlterColumn("dbo.base_reportsetting", "Start", c => c.String(maxLength: 20, storeType: "nvarchar"));
            DropTable("dbo.wg_culturewall");
        }
    }
}
