using BSFramework.Entity.EvaluateAbout;
using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;
using System;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class EvaluateCategoryItemMap : EntityTypeConfiguration<EvaluateCategoryItemEntity>
    {
        public EvaluateCategoryItemMap()
        {
            #region ������
            //��
            this.ToTable("WG_EVALUATECATEGORYITEM");
            //����
            this.HasKey(t => t.ItemId);
            // this.Ignore(t => t.Jobs);
            #endregion

            #region ���ù�ϵ
            #endregion
        }

    }
}
