using BSFramework.Application.Entity.Activity;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class ActivityEvaluateMap : EntityTypeConfiguration<ActivityEvaluateEntity>
    {
        public ActivityEvaluateMap()
        {
            #region ������
            //��
            this.ToTable("WG_ACTIVITYEVALUATE");
            //����
            this.HasKey(t => t.ActivityEvaluateId);
            #endregion

            #region ���ù�ϵ

            #endregion
        }
    }
    public class ToEvaluateMap : EntityTypeConfiguration<ToEvaluateEntity>
    {
        public ToEvaluateMap()
        {
            #region ������
            //��
            this.ToTable("WG_TOEVALUATE");
            //����
            this.HasKey(t => t.ToEvaluateId);
            #endregion

            #region ���ù�ϵ

            #endregion
        }
    }
}
