using BSFramework.Application.Entity.WorkPlan;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.WorkPlanManage
{
    public class WorkPlanContentMap : EntityTypeConfiguration<WorkPlanContentEntity>
    {
        public WorkPlanContentMap() 
        {
            #region 表、主键
            //表
            this.ToTable("WG_WORKPLANCONTENT");
            //主键
            this.HasKey(t => t.ID);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
