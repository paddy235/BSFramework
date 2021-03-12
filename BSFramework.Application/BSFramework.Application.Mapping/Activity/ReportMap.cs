using BSFramework.Application.Entity.Activity;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class ReportMap : EntityTypeConfiguration<ReportEntity>
    {
        public ReportMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_REPORT");
            //主键
            this.HasKey(t => t.ReportId);
            #endregion

            #region 配置关系

            #endregion
        }

    }


    public class ReportTaskMap : EntityTypeConfiguration<TaskEntity>
    {
        public ReportTaskMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_REPORTTASK");
            //主键
            this.HasKey(t => t.ReportTaskId);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
