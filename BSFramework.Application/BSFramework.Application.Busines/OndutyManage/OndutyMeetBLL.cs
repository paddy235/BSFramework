using BSFramework.Application.Entity.OndutyManage;
using BSFramework.Application.IService.OndutyManage;
using BSFramework.Application.Service.OndutyManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.OndutyManage
{
    public class OndutyMeetBLL
    {
        private IOndutyMeetService service = new OndutyMeetService();

        /// <summary>
        /// 获取台账分页数据
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public DataTable GetPageList(Pagination pagination, string queryJson)
        {
            return service.GetPageList(pagination,queryJson);
        }
        /// <summary>
        /// 获取台账分页数据
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public List<OndutyMeetEntity> GetPagesList(Pagination pagination, string queryJson, string userid)
        {
            return service.GetPagesList(pagination, queryJson,userid);

        }
        public List<OndutyMeetEntity> GetList(DateTime start,DateTime end,string userid,string deptid) {
            return service.GetList(start, end, userid, deptid);
        }
        public List<OndutyMeetEntity> GetList(string other) {
            return service.GetList(other);
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public OndutyMeetEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue"></param>
        public void RemoveForm(string keyValue)
        {
             service.RemoveForm(keyValue);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        public void SaveForm(string keyValue, OndutyMeetEntity entity)
        {
            service.SaveForm(keyValue, entity);
        }
    }
}
