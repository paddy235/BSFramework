using BSFramework.Application.Entity.Activity;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class ActivityMap : EntityTypeConfiguration<ActivityEntity>
    {
        public ActivityMap()
        {
            #region ������
            //��
            this.ToTable("WG_ACTIVITY");
            //����
            this.HasKey(t => t.ActivityId);
            #endregion

            #region ���ù�ϵ

            #endregion
        }

    }

    public class SubActivity : EntityTypeConfiguration<SubActivityEntity>
    {
        public SubActivity()
        {
            this.ToTable("WG_SUBACTIVITY");
            this.HasKey(t => t.SubActivityId);
        }
    }

}
