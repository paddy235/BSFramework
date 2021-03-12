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
    public class ToolTypeBLL
    {
        private IToolTypeService service = new ToolTypeService();

        //public IEnumerable<ToolTypeEntity> GetList(string deptid)
        //{
        //    return service.GetList(deptid);
        //}
        /// <summary>
        /// 获取某班组下所有的分类列表
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public IEnumerable<ToolTypeEntity> GetList(string deptId)
        {
            return service.GetList(deptId);
        }
        public ToolTypeEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }
        public IEnumerable<ToolTypeEntity> GetPageList(string name, string deptid, int page, int pagesize, out int total)
        {
            return service.GetPageList(name, deptid, page, pagesize, out total);
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
        public void SaveForm(string keyValue, ToolTypeEntity entity)
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
