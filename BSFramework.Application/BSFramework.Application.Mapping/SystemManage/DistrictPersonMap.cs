using BSFramework.Application.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.SystemManage
{
    /// <summary>
    /// 区域责任人
    /// </summary>
    public class DistrictPersonMap : EntityTypeConfiguration<DistrictPersonEntity>
    {
        public DistrictPersonMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_DISTRICTPERSON");
            //主键
            this.HasKey(t => t.DistrictPersonId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
