using BSFramework.Application.Entity.Activity;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.Activity
{
    /// <summary>
    /// �������Ԥ�ؿ�
    /// </summary>
    public class DangerCategoryMap : EntityTypeConfiguration<DangerCategoryEntity>
    {
        public DangerCategoryMap()
        {
            #region ������
            //��
            this.ToTable("WG_DANGERCATEGORY");
            //����
            this.HasKey(t => t.CategoryId);
            #endregion

            #region ���ù�ϵ

            #endregion
        }
    }
}
