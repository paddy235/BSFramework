using BSFramework.Application.Entity.Activity;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class SafetydayMaterialEntityMap : EntityTypeConfiguration<SafetydayMaterialEntity>
    {
        public SafetydayMaterialEntityMap()
        {
            #region ������
            //��
            this.ToTable("WG_SAFETYDAYMATERIAL");
            //����
            this.HasKey(t => t.Id);
            #endregion

            #region ���ù�ϵ

            #endregion
        }
    }
}
