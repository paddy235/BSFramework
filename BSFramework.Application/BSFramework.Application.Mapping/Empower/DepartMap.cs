using BSFramework.Application.Entity.Empower;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.Empower
{
    public class DepartMap : EntityTypeConfiguration<DepartEntity>
    {
        public DepartMap() 
        {
            #region 表、主键
            //表
            this.ToTable("WG_DEPART");
            //主键
            this.HasKey(t => t.ID);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
