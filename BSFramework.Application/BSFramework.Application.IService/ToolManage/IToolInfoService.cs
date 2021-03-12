using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.ToolManage;

namespace BSFramework.Application.IService.ToolManage
{
    public interface IToolInfoService
    {
        IEnumerable<ToolInfoEntity> GetList(string id);
        IEnumerable<ToolNumberEntity> GetToolNumberList(string id);

        IEnumerable<ToolInfoEntity> GetPageList(string name,string tid, int page, int pagesize, out int total);
        ToolInfoEntity GetEntity(string keyValue);
        ToolNumberEntity GetToolNumberEntity(string keyValue);

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        void RemoveFormList(List<ToolInfoEntity> list);
        void RemoveToolNumber(string keyValue);
        void RemoveToolNumberList(List<ToolNumberEntity> list);
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        void SaveForm(string keyValue, ToolInfoEntity entity);
        void SaveToolNumber(string keyValue, ToolNumberEntity entity);
        void Insert(params ToolInfoEntity[] insertTools);
        #endregion
    }
}
