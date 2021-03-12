using BSFramework.Application.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.WorkMeeting
{
    public class DistrictJobMap : EntityTypeConfiguration<DistrictJobEntity>
    {
        public DistrictJobMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_DISTRICTJOB");
            //主键
            this.HasKey(t => t.DistrictJobId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
