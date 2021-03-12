
using BSFramework.Application.Entity.FlowManage;
namespace BSFramework.Application.IService.FlowManage
{
    /// <summary>
    /// 描 述：工作流实例操作记录表操作接口
    /// </summary>
    public interface WFProcessOperationHistoryIService
    {
        /// <summary>
        /// 保存或更新实体对象
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        void SaveEntity(string keyValue, WFProcessOperationHistoryEntity entity);
    }
}
