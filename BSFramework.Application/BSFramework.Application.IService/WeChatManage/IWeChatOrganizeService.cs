using BSFramework.Application.Entity.WeChatManage;
using System.Collections.Generic;

namespace BSFramework.Application.IService.WeChatManage
{
    /// <summary>
    /// 描 述：企业号部门
    /// </summary>
    public interface IWeChatOrganizeService
    {
        #region 获取数据
        /// <summary>
        /// 部门列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<WeChatDeptRelationEntity> GetList();
        /// <summary>
        /// 部门实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        WeChatDeptRelationEntity GetEntity(string keyValue);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        /// <summary>
        /// 部门（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="weChatDeptRelationEntity">部门实体</param>
        /// <returns></returns>
        void SaveForm(string keyValue, WeChatDeptRelationEntity weChatDeptRelationEntity);
        #endregion
    }
}
