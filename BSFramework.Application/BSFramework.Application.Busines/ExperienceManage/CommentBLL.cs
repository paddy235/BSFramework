using BSFramework.Application.Entity.ExperienceManage;

using BSFramework.Application.IService.ExperienceManage;

using BSFramework.Application.Service.ExperienceManage;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.ExperienceManage
{
    public class CommentBLL
    {
        private ICommentService service = new CommentService();
        public IEnumerable<CommentEntity> GetList(string id)
        {
            return service.GetList(id);
        }

        public IEnumerable<CommentEntity> GetPageList(string from, string to, string id, string deptid, int page, int pagesize, out int total)
        {
            return service.GetPageList(from, to,id, deptid, page, pagesize, out total);
        }
        public CommentEntity GetEntity(string keyValue)
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
        public void SaveForm(string keyValue, CommentEntity entity)
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
