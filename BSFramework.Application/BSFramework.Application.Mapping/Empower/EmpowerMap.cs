using BSFramework.Application.Entity.Empower;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.Empower
{
    public class EmpowerMap : EntityTypeConfiguration<EmpowerEntity>
    {
        public EmpowerMap() 
        {
            #region 表、主键
            //表
            this.ToTable("WG_EMPOWER");
            //主键
            this.HasKey(t => t.ID);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
