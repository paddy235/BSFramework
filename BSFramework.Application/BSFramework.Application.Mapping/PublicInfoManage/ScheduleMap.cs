using BSFramework.Application.Entity.PublicInfoManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.PublicInfoManage
{
    /// <summary>
    /// 描 述：日程管理
    /// </summary>
    public class ScheduleMap : EntityTypeConfiguration<ScheduleEntity>
    {
        public ScheduleMap()
        {
            #region 表、主键
            //表
            this.ToTable("BASE_SCHEDULE");
            //主键
            this.HasKey(t => t.ScheduleId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
