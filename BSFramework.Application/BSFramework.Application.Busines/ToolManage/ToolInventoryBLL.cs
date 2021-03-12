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
    public class ToolInventoryBLL
    {
        private IToolInventoryService service = new ToolInventoryService();

        public IEnumerable<ToolInventoryEntity> GetList()
        {
            return service.GetList();
        }
        public ToolInventoryEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }
        public IEnumerable<ToolInventoryEntity> GetPageList(string deptcode, string name, string deptid, int page, int pagesize, out int total)
        {
            return service.GetPageList(deptcode, name, deptid, page, pagesize, out total);
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
        public void SaveForm(string keyValue, ToolInventoryEntity entity)
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
