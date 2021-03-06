using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class JobTemplateMap : EntityTypeConfiguration<JobTemplateEntity>
    {
        public JobTemplateMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_JOBTEMPLATE");
            //主键
            this.HasKey(t => t.JobId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
