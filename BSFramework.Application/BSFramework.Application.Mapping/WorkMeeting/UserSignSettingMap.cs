using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class UserSignSettingMap : EntityTypeConfiguration<UserSignSettingEntity>
    {
        public UserSignSettingMap()
        {
            #region ������
            //��
            this.ToTable("WG_USERSIGNSETTING");
            //����
            this.HasKey(t => t.UserId);
           // this.Ignore(t => t.Jobs);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
