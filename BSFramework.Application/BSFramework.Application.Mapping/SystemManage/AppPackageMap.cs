using BSFramework.Application.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.SystemManage
{
    /// <summary>
    /// �� ����app�汾
    /// </summary>
    public class AppPackageMap : EntityTypeConfiguration<AppPackageEntity>
    {
        public AppPackageMap()
        {
            #region ������
            //��
            this.ToTable("BIS_PACKAGE");
            //����
            this.HasKey(t => t.ID);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
