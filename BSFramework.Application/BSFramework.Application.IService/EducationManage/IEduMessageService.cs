using BSFramework.Application.Entity.EducationManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.EducationManage
{
    public interface IEduMessageService
    {
        IEnumerable<EduMessageEntity> GetList(string deptid, string eduid);
        IEnumerable<EduMessageEntity> GetListByUser(string userid);
        IEnumerable<EduMessageEntity> GetPageList(string deptid, int page, int pagesize, out int total);
        EduMessageEntity GetEntity(string keyValue);
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
        void SaveForm(string keyValue, EduMessageEntity entity);
        #endregion
    }
}
