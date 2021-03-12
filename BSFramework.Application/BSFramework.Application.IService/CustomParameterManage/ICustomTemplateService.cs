using BSFramework.Application.Entity.CustomParameterManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.CustomParameterManage
{
   public interface ICustomTemplateService 
    {
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <param name="userid">用户</param>
        /// <returns></returns>
        List<CustomTemplateEntity> GetPageList(Pagination pagination, string queryJson, string userid);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        CustomTemplateEntity getEntity(string keyvalue);

        /// <summary>
        /// 获取部门班组
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        List<CustomTemplateEntity> setSelect(string keyvalue);
        /// <summary>
        /// 新增 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        void SaveForm(CustomTemplateEntity entity, string userid);

        /// <summary>
        /// 删除
        /// </summary>
        void deleteEntity(string keyvalue);
    }
}
