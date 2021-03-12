using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Data;
using BSFramework.Application.Entity.SystemManage;

namespace BSFramework.Application.IService.SystemManage
{
    /// <summary>
    /// 描 述：数据设置
    /// </summary>
    public interface IDataSetService
    {
        #region 获取数据
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<DataSetEntity> GetList(string deptCode);
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        List<DataSetEntity> GetPageList(Pagination pagination, string queryJson);
         /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        DataSetEntity GetEntity(string keyValue);
        #endregion

        #region 验证数据
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        /// <summary>
        /// 保存角色表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="roleEntity">角色实体</param>
        /// <returns></returns>
        void SaveForm(string keyValue, DataSetEntity ds);
        #endregion
    }
}
