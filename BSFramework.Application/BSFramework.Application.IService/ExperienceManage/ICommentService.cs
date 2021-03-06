using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.ExperienceManage;

namespace BSFramework.Application.IService.ExperienceManage
{
    public interface ICommentService
    {
        IEnumerable<CommentEntity> GetList(string id);

        IEnumerable<CommentEntity> GetPageList(string from, string to, string id, string bzid, int page, int pagesize, out int total);
        CommentEntity GetEntity(string keyValue);
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
        void SaveForm(string keyValue, CommentEntity entity);
        #endregion
    }
}
