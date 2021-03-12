using BSFramework.Application.Entity.Activity;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.Activity
{
    public class SupplyPeopleMap : EntityTypeConfiguration<SupplyPeopleEntity>
    {
        public SupplyPeopleMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_SUPPLYPEOPLE");
            //主键
            this.HasKey(t => t.ID);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
