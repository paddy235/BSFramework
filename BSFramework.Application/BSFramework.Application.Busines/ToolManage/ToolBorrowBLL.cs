using BSFramework.Application.Entity.ToolManage;
using BSFramework.Application.IService.ToolManage;
using BSFramework.Application.Service.ToolManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.ToolManage
{
    public class ToolBorrowBLL
    {
        private IToolBorrowService service = new ToolBorrowService();

        public IEnumerable<ToolBorrowEntity> GetList(string userid, string deptid, DateTime? from, DateTime? to, string name)
        {
            return service.GetList(userid, deptid, from, to, name);
        }
        public List<ToolBorrowEntity> GetToolBorrowPageList(Pagination pagination, string queryJson)
        {

            return service.GetToolBorrowPageList(pagination, queryJson);
        }
        public IEnumerable<ToolBorrowEntity> GetListByUser(string userid, string tid)
        {
            return service.GetListByUser(userid, tid);
        }
        public IEnumerable<ToolBorrowEntity> GetListByUserId(string userid, string deptid, string tid, int page, int pagesize, out int total)
        {
            return service.GetListByUserId(userid, deptid, tid, page, pagesize, out total);
        }
        public ToolBorrowEntity GetEntity(string keyValue)
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
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, ToolBorrowEntity entity)
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
