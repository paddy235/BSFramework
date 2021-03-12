using BSFramework.Application.Entity.MessageManage;
using System.Collections.Generic;
using System.Data;

namespace BSFramework.Application.IService.MessageManage
{
    /// <summary>
    /// 描 述：即时通信用户管理
    /// </summary>
    public interface IMsgUserService
    {
        /// <summary>
        /// 获取联系人列表（即时通信）
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMUserModel> GetList(string OrganizeId);
        DataTable GetUsers(string OrganizeId);
        DataTable GetDepts(string OrganizeId);
        IEnumerable<IMUserInfoEntity> GetUserList(string OrganizeId,string userId="");
    }
}
