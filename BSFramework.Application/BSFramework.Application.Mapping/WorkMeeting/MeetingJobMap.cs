using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class MeetingJobMap : EntityTypeConfiguration<MeetingJobEntity>
    {
        public MeetingJobMap()
        {
            #region ������
            //��
            this.ToTable("WG_MEETINGJOB");
            //����
            this.HasKey(t => t.JobId);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }

    public class MeetingAndJobMap : EntityTypeConfiguration<MeetingAndJobEntity>
    {
        public MeetingAndJobMap()
        {
            #region ������
            //��
            this.ToTable("WG_MEETINGANDJOB");
            //����
            this.HasKey(t => t.MeetingJobId);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
