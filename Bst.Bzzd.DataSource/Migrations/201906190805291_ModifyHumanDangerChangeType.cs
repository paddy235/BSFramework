namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyHumanDangerChangeType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("wg_humandanger", "DeptId", c => c.String(unicode: false, storeType: "text"));
            AlterColumn("wg_humandanger", "DeptName", c => c.String(unicode: false, storeType: "text"));
        }
        
        public override void Down()
        {
            AlterColumn("wg_humandanger", "DeptName", c => c.String(maxLength: 2000, storeType: "nvarchar"));
            AlterColumn("wg_humandanger", "DeptId", c => c.String(maxLength: 2000, storeType: "nvarchar"));
        }
    }
}
