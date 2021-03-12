using BSFramework.Application.Entity.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.WebApp
{
  public interface IUserExperiencService
    {

        /// <summary>
        /// 根据userid查询
        /// </summary>
        /// <returns></returns>
        List<UserExperiencEntity> SelectByUserId(string userid);
        /// <summary>
        /// 获取明细
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        UserExperiencEntity SelectDetail(string Id);
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        void add(UserExperiencEntity entity);
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        void update(UserExperiencEntity entity);
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        void delete(string id);
    }
}
