using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.IService.SystemManage;
using BSFramework.Application.Service.SystemManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.SystemManage
{
  public  class WorkStandardBLL
    {
        private IWorkStandardService service;
        public WorkStandardBLL()
        {
            service = new WorkStandardService();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pagination">分页信息</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public List<WorkStandardEntity> GetPageList(Pagination pagination, string queryJson)
        {
            return service.GetPageList(pagination, queryJson);
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public WorkStandardEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }
        /// <summary>
        /// 保存表单
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        public void SaveForm(string keyValue, WorkStandardEntity entity)
        {
            service.SaveForm(keyValue, entity);
        }

        /// <summary>
        /// 删除单条数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            service.RemoveForm(keyValue);
        }

        /// <summary>
        /// 获取所有的工作标准
        /// </summary>
        /// <returns></returns>
        public List<WorkStandardEntity> GetAllList()
        {
          return service.GetAllList();
        }
    }
}
