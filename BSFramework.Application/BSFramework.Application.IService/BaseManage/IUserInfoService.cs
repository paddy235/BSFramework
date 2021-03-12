using BSFramework.Application.Entity.BaseManage;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Data;

namespace BSFramework.Application.IService.BaseManage
{
    /// <summary>
    /// 描 述：用户基本信息
    /// </summary>
    public interface IUserInfoService
    {
        #region 获取数据
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        IEnumerable<UserInfoEntity> GetPageList(Pagination pagination, string queryJson);
        /// <summary>
        /// 用户基本信息
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        UserInfoEntity GetUserInfoEntity(string keyValue);
   
        UserInfoEntity GetUserInfoByAccount(string account);

        #endregion

    }
}
