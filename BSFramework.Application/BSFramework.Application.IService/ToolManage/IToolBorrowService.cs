using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.ToolManage;
using System.Data;
using BSFramework.Util.WebControl;

namespace BSFramework.Application.IService.ToolManage
{
    public interface IToolBorrowService
    {
        IEnumerable<ToolBorrowEntity> GetList(string userid, string deptid, DateTime? from, DateTime? to, string name);

        IEnumerable<ToolBorrowEntity> GetListByUser(string userid, string tid);

        IEnumerable<ToolBorrowEntity> GetListByUserId(string userid, string deptid, string tid, int page, int pagesize, out int total);

        List<ToolBorrowEntity> GetToolBorrowPageList(Pagination pagination, string queryJson);
        ToolBorrowEntity GetEntity(string keyValue);

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        void SaveForm(string keyValue, ToolBorrowEntity entity);
        #endregion
    }
}
