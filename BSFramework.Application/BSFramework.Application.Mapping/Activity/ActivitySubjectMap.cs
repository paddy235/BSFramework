using BSFramework.Application.Entity.Activity;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class ActivitySubjectMap : EntityTypeConfiguration<ActivitySubjectEntity>
    {
        public ActivitySubjectMap()
        {
            #region ������
            //��
            this.ToTable("WG_ACTIVITYSUBJECT");
            //����
            this.HasKey(t => t.ActivitySubjectId);
            #endregion

            #region ���ù�ϵ

            #endregion
        }
    }
}
