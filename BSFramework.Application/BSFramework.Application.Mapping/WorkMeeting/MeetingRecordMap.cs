using BSFramework.Application.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.WorkMeeting
{
   public class MeetingRecordMap : EntityTypeConfiguration<MeetingRecordEntity>
    {
        public MeetingRecordMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_MEETINGRECORD");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
