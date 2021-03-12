
using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;
using System;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// �� ����ǩ����
    /// </summary>
    public class MeetingSigninMap : EntityTypeConfiguration<MeetingSigninEntity>
    {
        public MeetingSigninMap()
        {
            #region ������
            //��
            this.ToTable("WG_MEETINGSIGNIN");
            //����
            this.HasKey(t => t.SigninId);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
    /// <summary>
    /// �� ����ȱ�ڼ�¼��
    /// </summary>
    public class UnSignRecordMap : EntityTypeConfiguration<UnSignRecordEntity>
    {
        public UnSignRecordMap()
        {
            #region ������
            //��
            this.ToTable("WG_UNSIGNRECORD");
            //����
            this.HasKey(t => t.UnSignRecordId);
            #endregion

            #region ���ù�ϵ
            #endregion
        }
    }
}
