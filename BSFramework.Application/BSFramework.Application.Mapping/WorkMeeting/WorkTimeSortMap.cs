using BSFramework.Application.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.WorkMeeting
{
   public class WorkTimeSortMap : EntityTypeConfiguration<WorkTimeSortEntity>
    {
        public WorkTimeSortMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_WORKTIMESORT");
            //主键
            this.HasKey(t => t.worktimesortid);
            // this.Ignore(t => t.Jobs);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
