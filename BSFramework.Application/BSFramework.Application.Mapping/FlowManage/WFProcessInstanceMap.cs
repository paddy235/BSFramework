using BSFramework.Application.Entity.FlowManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.FlowManage
{
    /// <summary>
    /// 描 述：工作流实例进程表
    /// </summary>
    public class WFProcessInstanceMap : EntityTypeConfiguration<WFProcessInstanceEntity>
    {
        public WFProcessInstanceMap()
        {
            #region 表、主键
            //表
            this.ToTable("WF_PROCESSINSTANCE");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
