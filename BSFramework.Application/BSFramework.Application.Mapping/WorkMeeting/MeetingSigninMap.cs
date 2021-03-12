
using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;
using System;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// 描 述：签到表
    /// </summary>
    public class MeetingSigninMap : EntityTypeConfiguration<MeetingSigninEntity>
    {
        public MeetingSigninMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_MEETINGSIGNIN");
            //主键
            this.HasKey(t => t.SigninId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
    /// <summary>
    /// 描 述：缺勤记录表
    /// </summary>
    public class UnSignRecordMap : EntityTypeConfiguration<UnSignRecordEntity>
    {
        public UnSignRecordMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_UNSIGNRECORD");
            //主键
            this.HasKey(t => t.UnSignRecordId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
