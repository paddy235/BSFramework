using BSFramework.Entity.EvaluateAbout;
using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;
using System;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class EvaluateCategoryMap : EntityTypeConfiguration<EvaluateCategoryEntity>
    {
        public EvaluateCategoryMap()
        {
            #region ������
            //��
            this.ToTable("WG_EVALUATECATEGORY");
            //����
            this.HasKey(t => t.CategoryId);
            // this.Ignore(t => t.Jobs);
            #endregion

            #region ���ù�ϵ
            #endregion
        }

    }
}
