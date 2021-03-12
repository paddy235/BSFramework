using BSFramework.Application.Entity.AttendManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.AttendManage
{
    public interface IAttendSerivce
    {
        IEnumerable<AttendEntity> GetList(string id);

        IEnumerable<AttendEntity> GetPageList(string from, string to, string name, int page, int pagesize, out int total);
        AttendEntity GetEntity(string keyValue);
        string GetCount(string deptid,DateTime attenddate);
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
        void SaveForm(string keyValue, AttendEntity entity);
        #endregion
    }
}
