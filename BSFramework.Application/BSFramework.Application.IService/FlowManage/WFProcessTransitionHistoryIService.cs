using BSFramework.Application.Entity.FlowManage;

namespace BSFramework.Application.IService.FlowManage
{
    /// <summary>
    /// 描 述：工作流实例节点转化记录操作接口
    /// </summary>
    public interface WFProcessTransitionHistoryIService
    {
        #region 获取数据
        /// <summary>
        /// 获取流转实体
        /// </summary>
        /// <param name="processId">流程实例ID</param>
        /// <param name="toNodeId">流转到的节点Id</param>
        /// <returns></returns>
        WFProcessTransitionHistoryEntity GetEntity(string processId, string toNodeId);
        #endregion

        #region 提交数据
        /// <summary>
        /// 保存实例
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        int SaveEntity(string keyValue, WFProcessTransitionHistoryEntity entity);
        #endregion
    }
}
