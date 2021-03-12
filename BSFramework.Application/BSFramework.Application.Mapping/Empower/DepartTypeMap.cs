using BSFramework.Application.Entity.Empower;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.Empower
{
    public class DepartTypeMap : EntityTypeConfiguration<DepartTypeEntity>
    {
        public DepartTypeMap() 
        {
            #region 表、主键
            //表
            this.ToTable("WG_DEPARTTYPE");
            //主键
            this.HasKey(t => t.ID);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
