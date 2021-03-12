using BSFramework.Application.Entity.Activity;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class ActivityMap : EntityTypeConfiguration<ActivityEntity>
    {
        public ActivityMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_ACTIVITY");
            //主键
            this.HasKey(t => t.ActivityId);
            #endregion

            #region 配置关系

            #endregion
        }

    }

    public class SubActivity : EntityTypeConfiguration<SubActivityEntity>
    {
        public SubActivity()
        {
            this.ToTable("WG_SUBACTIVITY");
            this.HasKey(t => t.SubActivityId);
        }
    }

}
