using BSFramework.Application.Entity.EducationManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.EducationManage
{
    public interface IEduAnswerService
    {
        IEnumerable<EduAnswerEntity> GetList(string eduid);

        IEnumerable<EduAnswerEntity> GetPageList(string deptid, int page, int pagesize, out int total);
        EduAnswerEntity GetEntity(string keyValue);

        List<TestQuestionsEntity> FindTrainings(string key, int limit);
        void Add(EduAnswerEntity entity);
        EduAnswerEntity Get(string id);
        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        void Edit(EduAnswerEntity entity);
        void Delete(EduAnswerEntity entity);
        List<EduAnswerEntity> List(string baseId);

        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        void SaveForm(string keyValue, EduAnswerEntity entity);

        /// <summary>
        /// 保存技术问答评价
        /// </summary>
        /// <param name="data"></param>
        void SaveAnswerComment(List<EduAnswerEntity> data, EduBaseInfoEntity entity);
        #endregion
    }
}
