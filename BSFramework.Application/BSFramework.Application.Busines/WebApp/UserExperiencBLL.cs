using BSFramework.Application.Entity.WebApp;
using BSFramework.Application.IService.WebApp;
using BSFramework.Application.Service.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.WebApp
{
    public class UserExperiencBLL
    {

        IUserExperiencService service = new UserExperiencService();

        /// <summary>
        /// 根据userid查询
        /// </summary>
        /// <returns></returns>
        public List<UserExperiencEntity> SelectByUserId(string userid)
        {
            return service.SelectByUserId(userid);

        }
        /// <summary>
        /// 获取明细
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public UserExperiencEntity SelectDetail(string Id)
        {
            return service.SelectDetail(Id);

        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        public void add(UserExperiencEntity entity)
        {
            service.add(entity);

        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        public void update(UserExperiencEntity entity)
        {
            service.update(entity);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        public void delete(string id)
        {
            service.delete(id);
        }
    }

}
