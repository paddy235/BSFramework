using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// 描 述：班次
    /// </summary>
    public class WorkSettingMap : EntityTypeConfiguration<WorkSettingEntity>
    {
        public WorkSettingMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_WORKSETTING");
            //主键
            this.HasKey(t => t.WorkSettingId);
            // this.Ignore(t => t.Jobs);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
