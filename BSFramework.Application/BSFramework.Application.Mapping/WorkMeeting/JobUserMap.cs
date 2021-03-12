using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class JobUserMap : EntityTypeConfiguration<JobUserEntity>
    {
        public JobUserMap()
        {
            #region ������
            //��
            this.ToTable("WG_JOBUSER");
            //����
            this.HasKey(t => t.JobUserId);
            // this.Ignore(t => t.Jobs);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
