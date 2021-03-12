using BSFramework.Application.Entity.ToolManage;
using BSFramework.Application.IService.ToolManage;
using BSFramework.Application.Service.ToolManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.ToolManage
{
    public class ToolRepairBLL
    {
        private IToolRepairService service = new ToolRepairService();

        public IEnumerable<ToolRepairEntity> GetList()
        {
            return service.GetList();
        }
        public ToolRepairEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }
        public IEnumerable<ToolRepairEntity> GetPageList(string from, string to, int page, int pagesize, out int total)
        {
            return service.GetPageList(from, to, page, pagesize, out total);
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
        public void SaveForm(string keyValue, ToolRepairEntity entity)
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
