
using BSFramework.Application.Entity.FlowManage;
namespace BSFramework.Application.IService.FlowManage
{
    /// <summary>
    /// 描 述：工作流实例模板内容表操作接口
    /// </summary>
    public interface WFProcessSchemeIService
    {
        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        WFProcessSchemeEntity GetEntity(string keyValue);

        /// <summary>
        /// 保存实体数据
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        void SaveEntity(string keyValue, WFProcessSchemeEntity entity);
    }
}
