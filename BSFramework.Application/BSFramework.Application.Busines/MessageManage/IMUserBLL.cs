using BSFramework.Application.Entity.MessageManage;
using BSFramework.Application.IService.MessageManage;
using BSFramework.Application.Service.MessageManage;
using System.Collections.Generic;
using System.Data;

namespace BSFramework.Application.Busines.MessageManage
{
    /// <summary>
    /// 描 述：用户管理
    /// </summary>
    public class IMUserBLL
    {
        private IMsgUserService service = new IMUserService();
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IMUserModel> GetList(string OrganizeId)
        {
            return service.GetList(OrganizeId);
        }
        public DataTable GetUsers(string OrganizeId)
        {
            return service.GetUsers(OrganizeId);
        }
        public DataTable GetDepts(string OrganizeId)
        {
            return service.GetDepts(OrganizeId);
        }
        public IEnumerable<IMUserInfoEntity> GetUserList(string OrganizeId,string userId="")
        {
            return service.GetUserList(OrganizeId, userId);
        }
    }
}
