using BSFramework.Application.Entity.Activity;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class ActivityEvaluateMap : EntityTypeConfiguration<ActivityEvaluateEntity>
    {
        public ActivityEvaluateMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_ACTIVITYEVALUATE");
            //主键
            this.HasKey(t => t.ActivityEvaluateId);
            #endregion

            #region 配置关系

            #endregion
        }
    }
    public class ToEvaluateMap : EntityTypeConfiguration<ToEvaluateEntity>
    {
        public ToEvaluateMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_TOEVALUATE");
            //主键
            this.HasKey(t => t.ToEvaluateId);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
