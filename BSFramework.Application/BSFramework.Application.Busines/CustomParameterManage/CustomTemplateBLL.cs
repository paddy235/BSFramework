
using BSFramework.Application.Entity.CustomParameterManage;
using BSFramework.Application.IService.CustomParameterManage;
using BSFramework.Application.Service.CustomParameterManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.CustomParameterManage
{
    public class CustomTemplateBLL
    {
        private ICustomTemplateService CPService = new CustomTemplateService();

        #region 查询数据
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <param name="userid">用户</param>
        /// <returns></returns>
        public List<CustomTemplateEntity> GetPageList(Pagination pagination, string queryJson, string userid)
        {
            return CPService.GetPageList(pagination,queryJson,userid);
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public CustomTemplateEntity getEntity(string keyvalue)
        {
            return CPService.getEntity(keyvalue);
        }


        /// <summary>
        /// 获取部门班组
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public List<CustomTemplateEntity> setSelect(string keyvalue) {
            return CPService.setSelect(keyvalue);

        }
        #endregion
        #region 操作数据

        /// <summary>
        /// 新增 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        public void SaveForm(CustomTemplateEntity entity, string userid)
        {
             CPService.SaveForm(entity,userid);

        }

        /// <summary>
        /// 删除
        /// </summary>
        public void deleteEntity(string keyvalue)
        {
            CPService.deleteEntity(keyvalue);

        }
        #endregion

    }
}
