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
    public class ToolCheckBLL
    {
        private IToolCheckService service = new ToolCheckService();

        public IEnumerable<ToolCheckEntity> GetList()
        {
            return service.GetList();
        }
        public ToolCheckEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }
        public IEnumerable<ToolCheckEntity> GetPageList(string from, string to, string checkstate, int page, int pagesize, out int total)
        {
            return service.GetPageList(from, to, checkstate,page, pagesize, out total);
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
        public void SaveForm(string keyValue, ToolCheckEntity entity)
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
