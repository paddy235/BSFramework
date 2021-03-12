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
    public class QuestionBLL
    {
        private IQuestionService service = new QuestionService();
        public IEnumerable<QuestionEntity> GetList(string deptid)
        {
            return service.GetList(deptid);
        }

        public IEnumerable<QuestionEntity> GetPageList(string from, string to, string title, string remark, string deptid, int page, int pagesize, out int total)
        {
            return service.GetPageList(from, to, title, remark, deptid, page, pagesize, out total);
        }
        public QuestionEntity GetEntity(string keyValue)
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
        public void SaveForm(string keyValue, QuestionEntity entity)
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
