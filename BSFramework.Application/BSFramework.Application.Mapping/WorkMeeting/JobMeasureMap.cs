using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    public class JobMeasureMap : EntityTypeConfiguration<JobMeasureEntity>
    {
        public JobMeasureMap()
        {
            #region ������
            //��
            this.ToTable("WG_JOBMEASURE");
            //����
            this.HasKey(t => t.JobMeasureId);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
