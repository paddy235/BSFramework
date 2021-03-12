using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    public class JobDangerousMap : EntityTypeConfiguration<JobDangerousEntity>
    {
        public JobDangerousMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_JOBDANGEROUS");
            //主键
            this.HasKey(t => t.JobDangerousId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
