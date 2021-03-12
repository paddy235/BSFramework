using BSFramework.Application.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.WorkMeeting
{
    public class DistrictSignInMap : EntityTypeConfiguration<DistrictSignInEntity>
    {
        public DistrictSignInMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_DISTRICTSIGNIN");
            //主键
            this.HasKey(t => t.SigninId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
