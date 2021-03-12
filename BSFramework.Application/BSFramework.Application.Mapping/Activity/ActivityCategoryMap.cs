using BSFramework.Application.Entity.Activity;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class ActivityCategoryMap : EntityTypeConfiguration<ActivityCategoryEntity>
    {
        public ActivityCategoryMap()
        {
            #region ������
            //��
            this.ToTable("WG_ACTIVITYCATEGORY");
            //����
            this.HasKey(t => t.ActivityCategoryId);
            #endregion

            #region ���ù�ϵ

            #endregion
        }
    }
}
