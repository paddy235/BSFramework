using BSFramework.Application.Entity.AttendManage;
using BSFramework.Application.Entity.ToolManage;
using BSFramework.Application.IService.AttendManage;
using BSFramework.Application.IService.ToolManage;
using BSFramework.Application.Service.AttendManage;
using BSFramework.Application.Service.ToolManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.AttendManage
{
    public class AttendBLL
    {
        private IAttendSerivce service = new AttendService();
        public IEnumerable<AttendEntity> GetList(string id)
        {
            return service.GetList(id);
        }

        public IEnumerable<AttendEntity> GetPageList(string from, string to, string name, int page, int pagesize, out int total) 
        {
            return service.GetPageList(from, to, name, page, pagesize, out total);
        }
        public AttendEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }
        public string GetCount(string deptid,DateTime attenddate)
        {
            return service.GetCount(deptid, attenddate);
        }
        public void RemoveForm(string keyValue)
        {
            try
            {
                service.RemoveForm(keyValue);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, AttendEntity entity)
        {
            try
            {
                service.SaveForm(keyValue, entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
