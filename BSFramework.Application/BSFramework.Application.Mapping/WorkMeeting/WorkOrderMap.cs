using BSFramework.Application.Entity;
using BSFramework.Entity.WorkMeeting;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Mapping.WorkOrderMap
{
    /// <summary>
    /// 排班关联部门
    /// </summary>
    public class WorkOrderMap : EntityTypeConfiguration<WorkOrderEntity>
    {
        public WorkOrderMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_WORKORDER");
            //主键
            this.HasKey(t => t.WorkOrderId);
            // this.Ignore(t => t.Jobs);
            #endregion

            #region 配置关系
            #endregion
        }


    }
}



