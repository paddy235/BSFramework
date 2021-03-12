using BSFramework.Application.Entity.PublicInfoManage;
using System.Data.Entity.ModelConfiguration;


namespace BSFramework.Application.Mapping.PublicInfoManage
{
    public class NewFileInfoMap : EntityTypeConfiguration<NewFileInfoEntity>
    {
        public NewFileInfoMap() 
        {
            this.ToTable("WG_NEWFILEINFO");
            //主键
            this.HasKey(t => t.ID);
        }
    }
}
