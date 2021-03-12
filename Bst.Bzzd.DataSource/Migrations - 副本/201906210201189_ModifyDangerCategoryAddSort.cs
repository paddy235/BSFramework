namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyDangerCategoryAddSort : DbMigration
    {
        public override void Up()
        {
            AddColumn("wg_dangercategory", "Sort", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("wg_dangercategory", "Sort");
        }
    }
}
