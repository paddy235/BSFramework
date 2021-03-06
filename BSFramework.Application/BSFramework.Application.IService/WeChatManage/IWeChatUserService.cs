using BSFramework.Application.Entity.WeChatManage;
using System.Collections.Generic;

namespace BSFramework.Application.IService.WeChatManage
{
    /// <summary>
    /// 描 述：企业号成员
    /// </summary>
    public interface IWeChatUserService
    {
        #region 获取数据
        /// <summary>
        /// 成员列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<WeChatUserRelationEntity> GetList();
        /// <summary>
        /// 成员实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        WeChatUserRelationEntity GetEntity(string keyValue);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除成员
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        /// <summary>
        /// 成员（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="weChatUserRelationEntity">用户实体</param>
        /// <returns></returns>
        void SaveForm(string keyValue, WeChatUserRelationEntity weChatUserRelationEntity);
        #endregion
    }
}
