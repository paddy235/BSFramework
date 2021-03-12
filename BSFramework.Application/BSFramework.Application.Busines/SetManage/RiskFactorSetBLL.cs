using BSFramework.Application.Entity.SetManage;
using BSFramework.Application.IService.SetManage;
using BSFramework.Application.Service.SetManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.SetManage
{
    public class RiskFactorSetBLL
    {
        IRiskFactorSetService service = new RiskFactorSetService();
        /// <summary>
        /// 得到危险因素及防范措施列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public DataTable GetPageList(Pagination pagination, string queryJson)
        {
            return service.GetPageList(pagination, queryJson);
        }

        /// <summary>
        /// 得到当前班组下的危险因素列表
        /// </summary>
        /// <param name="deptid">部门id 必传</param>
        /// <param name="content">危险因素 （模糊查询）</param>
        /// <returns></returns>
        public IEnumerable<RiskFactorSetEntity> GetList(string deptid, string content)
        {
            return service.GetList(deptid, content);
        }

        /// <summary>
        /// 得到危险因素实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public RiskFactorSetEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }

        public List<MeasureSetEntity> GetList(string id)
        {
            return service.GetList(id);
        }

        /// <summary>
        /// 得到危险因素实体
        /// </summary>
        /// <param name="content">危险因素</param>
        /// <returns></returns>
        public RiskFactorSetEntity GetEntityByContent(string content)
        {
            return service.GetEntityByContent(content);
        }

        /// <summary>
        /// 删除危险因素,会把危险因素关联的防范措施一起删除
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            service.RemoveForm(keyValue);
        }

        /// <summary>
        /// 保存危险因素及防范措施
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="userEntity">用户实体</param>
        /// <returns></returns>
        public void SaveForm(RiskFactorSetEntity entity)
        {
            service.SaveForm(entity);
        }
    }
}
