using BSFramework.Application.Entity.PublicInfoManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.PublicInfoManage
{
    /// <summary>
    /// �� ����app�汾
    /// </summary>
    public class PackageMap : EntityTypeConfiguration<PackageEntity>
    {
        public PackageMap()
        {
            #region ������
            //��
            this.ToTable("WG_PACKAGE");
            //����
            this.HasKey(t => t.ID);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
