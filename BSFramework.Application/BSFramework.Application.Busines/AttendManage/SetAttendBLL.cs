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
    public class SetAttendBLL
    {
        private ISetAttendService service = new SetAttendService();

        public IEnumerable<SetAttendEntity> GetList()
        {
            return service.GetList();
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
        public void SaveForm(string keyValue, SetAttendEntity entity)
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
