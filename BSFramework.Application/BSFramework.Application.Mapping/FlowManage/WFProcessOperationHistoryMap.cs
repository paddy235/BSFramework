using BSFramework.Application.Entity.FlowManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.FlowManage
{
    public class WFProcessOperationHistoryMap: EntityTypeConfiguration<WFProcessOperationHistoryEntity>
    {
        /// <summary>
        /// 描 述：工作流实例实例操作历史记录表
        /// </summary>
        public WFProcessOperationHistoryMap()
        {
            #region 表、主键
            //表
            this.ToTable("WF_PROCESSOPERATIONHISTORY");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
