namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyHumanDanger : DbMigration
    {
        public override void Up()
        {
            AlterColumn("WG_HUMANDANGER", "DEPTID", c => c.String(unicode: false, storeType: "text"));
            AlterColumn("WG_HUMANDANGER", "DEPTNAME", c => c.String(unicode: false, storeType: "text"));
        }
        
        public override void Down()
        {
            AlterColumn("WG_HUMANDANGER", "DEPTNAME", c => c.String(maxLength: 2000, storeType: "nvarchar"));
            AlterColumn("WG_HUMANDANGER", "DEPTID", c => c.String(maxLength: 2000, storeType: "nvarchar"));
        }
    }
}
