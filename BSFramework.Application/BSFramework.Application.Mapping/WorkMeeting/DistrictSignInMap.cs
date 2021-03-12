using BSFramework.Application.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.WorkMeeting
{
    public class DistrictSignInMap : EntityTypeConfiguration<DistrictSignInEntity>
    {
        public DistrictSignInMap()
        {
            #region ������
            //��
            this.ToTable("WG_DISTRICTSIGNIN");
            //����
            this.HasKey(t => t.SigninId);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
