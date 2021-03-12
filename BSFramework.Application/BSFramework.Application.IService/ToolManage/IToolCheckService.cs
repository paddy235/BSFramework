using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.ToolManage;

namespace BSFramework.Application.IService.ToolManage
{
    public interface IToolCheckService
    {
        IEnumerable<ToolCheckEntity> GetList();

        IEnumerable<ToolCheckEntity> GetPageList(string from, string to, string checkstate, int page, int pagesize, out int total);
        ToolCheckEntity GetEntity(string keyValue);

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
        void SaveForm(string keyValue, ToolCheckEntity entity);
    }
}
