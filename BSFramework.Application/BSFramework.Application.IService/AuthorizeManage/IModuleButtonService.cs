using BSFramework.Application.Entity.AuthorizeManage;
using System.Collections.Generic;
using System.Data;

namespace BSFramework.Application.IService.AuthorizeManage
{
    /// <summary>
    /// 描 述：功能按钮
    /// </summary>
    public interface IModuleButtonService
    {
        #region 获取数据
        /// <summary>
        /// 按钮列表
        /// </summary>
        /// <returns></returns>
        List<ModuleButtonEntity> GetList();
        DataTable GetButtonList();
        /// <summary>
        /// 按钮列表
        /// </summary>
        /// <param name="moduleId">功能Id</param>
        /// <returns></returns>
        List<ModuleButtonEntity> GetList(string moduleId);

        DataTable GetDataList(string moduleId, string objectId, string itemId);
        /// <summary>
        /// 按钮实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        ModuleButtonEntity GetEntity(string keyValue);
        #endregion

        #region 提交数据
        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="moduleButtonEntity">按钮实体</param>
        void AddEntity(ModuleButtonEntity moduleButtonEntity);
        #endregion
    }
}
