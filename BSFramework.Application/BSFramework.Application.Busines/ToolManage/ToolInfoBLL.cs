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
   
    
    public class ToolInfoBLL
    {
        private IToolInfoService service = new ToolInfoService();
        public IEnumerable<ToolInfoEntity> GetList(string id)
        {
            return service.GetList(id);
        }
        public IEnumerable<ToolNumberEntity> GetToolNumberList(string id) 
        {
            return service.GetToolNumberList(id);
        }
        public ToolNumberEntity GetToolNumberEntity(string keyValue) 
        {
            return service.GetToolNumberEntity(keyValue);
        }
        public void SaveToolNumber(string keyValue, ToolNumberEntity entity) 
        {
            service.SaveToolNumber(keyValue, entity);
        }
        public IEnumerable<ToolInfoEntity> GetPageList(string name,string tid, int page, int pagesize, out int total)
        {
            return service.GetPageList(name,tid, page, pagesize, out total);
        }
        public ToolInfoEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
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
        public void RemoveFormList(List<ToolInfoEntity> list)
        {
            try
            {
                service.RemoveFormList(list);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void RemoveToolNumber(string keyValue)
        {
            service.RemoveToolNumber(keyValue);
        }
        public void RemoveToolNumberList(List<ToolNumberEntity> list) 
        {
            service.RemoveToolNumberList(list);
        }
        

        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, ToolInfoEntity entity)
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
        /// <summary>
        /// 新增单条或多条工器具
        /// </summary>
        /// <param name="insertTools">单个实体或数组</param>
        public void Insert(params ToolInfoEntity[] insertTools)
        {
             service.Insert(insertTools);
        }
    }
}
