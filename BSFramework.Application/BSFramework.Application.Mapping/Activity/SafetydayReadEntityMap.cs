using BSFramework.Application.Entity.Activity;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class SafetydayReadEntityMap : EntityTypeConfiguration<SafetydayReadEntity>
    {
        public SafetydayReadEntityMap()
        {
            #region ������
            //��
            this.ToTable("WG_SAFETYDAYREAD");
            //����
            this.HasKey(t => t.SafetydayReadId);
            #endregion

            #region ���ù�ϵ

            #endregion
        }
    }
}
