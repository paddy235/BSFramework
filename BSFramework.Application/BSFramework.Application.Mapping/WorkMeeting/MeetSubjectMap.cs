using BSFramework.Application.Entity;
using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkOrderMap
{
    /// <summary>
    /// 班前一课
    /// </summary>
    public class MeetSubjectMap : EntityTypeConfiguration<MeetSubjectEntity>
    {
        public MeetSubjectMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_MEETSUBJECT");
            //主键
            this.HasKey(t => t.ID);
            // this.Ignore(t => t.Jobs);
            #endregion

            #region 配置关系
            #endregion
        }


    }
}



