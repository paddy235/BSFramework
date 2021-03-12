using BSFramework.Application.Entity.Activity;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class ActivityPersonMap : EntityTypeConfiguration<ActivityPersonEntity>
    {
        public ActivityPersonMap()
        {
            #region ������
            //��
            this.ToTable("WG_ACTIVITYPERSON");
            //����
            this.HasKey(t => t.ActivityPersonId);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
