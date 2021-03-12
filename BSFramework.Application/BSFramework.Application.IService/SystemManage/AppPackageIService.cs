using BSFramework.Application.Entity.SystemManage;
using BSFramework.Util.WebControl;
using System.Collections.Generic;

namespace BSFramework.Application.IService.SystemManage
{
    /// <summary>
    /// 描 述：app版本
    /// </summary>
    public interface AppPackageIService
    {
        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        IEnumerable<AppPackageEntity> GetList(string queryJson);
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        AppPackageEntity GetEntity(string keyValue);

        IEnumerable<AppPackageEntity> GetPageList(Pagination pagination, string queryJson);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        void SaveForm(string keyValue, AppPackageEntity entity);
        #endregion
    }
}
