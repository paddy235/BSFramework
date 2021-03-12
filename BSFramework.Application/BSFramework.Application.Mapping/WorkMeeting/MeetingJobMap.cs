using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkMeeting
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class MeetingJobMap : EntityTypeConfiguration<MeetingJobEntity>
    {
        public MeetingJobMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_MEETINGJOB");
            //主键
            this.HasKey(t => t.JobId);
            #endregion

            #region 配置关系
            #endregion
        }
    }

    public class MeetingAndJobMap : EntityTypeConfiguration<MeetingAndJobEntity>
    {
        public MeetingAndJobMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_MEETINGANDJOB");
            //主键
            this.HasKey(t => t.MeetingJobId);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
