using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class WorkmeetingMap : EntityTypeConfiguration<WorkmeetingEntity>
    {
        public WorkmeetingMap()
        {
            #region ������
            //��
            this.ToTable("WG_WORKMEETING");
            //����
            this.HasKey(t => t.MeetingId);
           // this.Ignore(t => t.Jobs);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
