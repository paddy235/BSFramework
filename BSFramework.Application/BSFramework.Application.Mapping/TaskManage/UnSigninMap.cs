using BSFramework.Application.Entity.TaskManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.TaskManage
{
    public class UnSigninMap : EntityTypeConfiguration<UnSigninEntity>
    {
        public UnSigninMap()
        {
            #region ������
            //��
            this.ToTable("WG_UNSIGNIN");
            //����
            this.HasKey(t => t.UnSigninId);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
