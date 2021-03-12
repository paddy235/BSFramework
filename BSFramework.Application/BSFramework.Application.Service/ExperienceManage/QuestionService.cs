
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.IService.ExperienceManage;
using BSFramework.Application.Entity.ExperienceManage;
using BSFramework.Application.Entity.PublicInfoManage;

namespace BSFramework.Application.Service.ExperienceManage
{
    public class QuestionService : RepositoryFactory<QuestionEntity>, IQuestionService
    {
        public IEnumerable<QuestionEntity> GetList(string deptid)
        {
            //string deptid = OperatorProvider.Provider.Current().DeptId;
            var query = this.BaseRepository().IQueryable().Where(x => x.BZId == deptid);
            //if (!string.IsNullOrEmpty(name))
            //{
            //    query = query.Where(x => x.Name.Contains(name));
            //}

            return query.OrderByDescending(x => x.CreateDate).ToList();
        }

        public IEnumerable<QuestionEntity> GetPageList(string from, string to, string title, string remark, string deptid, int page, int pagesize, out int total)
        {
            var query = this.BaseRepository().IQueryable();
            //query = query.Where(x => x.BZId == deptid);
            if (!string.IsNullOrEmpty(remark)) query = query.Where(x => x.Remark == remark);
            if (!string.IsNullOrEmpty(title)) query = query.Where(x => x.Title.Contains(title));
            if (!string.IsNullOrEmpty(from))
            {
                DateTime time1 = DateTime.Parse(from);
                DateTime time2 = time1.AddDays(1);
                query = query.Where(x => x.CreateDate >= time1);
            }

            if (!string.IsNullOrEmpty(to))
            {
                DateTime time2 = DateTime.Parse(to).AddDays(1).AddMinutes(-1);
                query = query.Where(x => x.CreateDate <= time2);
            }
            total = query.Count();
            return query.OrderByDescending(x => x.CreateDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList();

        }


        public QuestionEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }


        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, QuestionEntity entity)
        {
            var entity1 = this.GetEntity(keyValue);
            if (string.IsNullOrEmpty(keyValue))
            {
                entity.CreateDate = DateTime.Now;
                this.BaseRepository().Insert(entity);
                //new Repository<FileInfoEntity>(DbFactory.Base()).Insert(entity.Files.ToList());

            }
            else
            {
                entity1.ClickCount = entity.ClickCount;
                entity1.Files = null;
                entity1.commentCount = entity.commentCount;
                this.BaseRepository().Update(entity1);
            }
        }
        #endregion
    }
}
