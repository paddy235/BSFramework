using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    public class JobDangerousMap : EntityTypeConfiguration<JobDangerousEntity>
    {
        public JobDangerousMap()
        {
            #region ������
            //��
            this.ToTable("WG_JOBDANGEROUS");
            //����
            this.HasKey(t => t.JobDangerousId);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
