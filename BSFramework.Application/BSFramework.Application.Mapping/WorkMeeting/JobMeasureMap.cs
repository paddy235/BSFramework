using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    public class JobMeasureMap : EntityTypeConfiguration<JobMeasureEntity>
    {
        public JobMeasureMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_JOBMEASURE");
            //主键
            this.HasKey(t => t.JobMeasureId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
