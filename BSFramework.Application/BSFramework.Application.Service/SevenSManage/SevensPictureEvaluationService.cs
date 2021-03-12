using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.SevenSManage;
using BSFramework.Application.IService.SevenSManage;
using BSFramework.Data.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.SevenSManage
{
    public class SevensPictureEvaluationService : RepositoryFactory<SevensPictureEvaluationEntity>, ISevensPictureEvaluationService
    {
        /// <summary>
        /// 根据Id，获取所有的评论
        /// </summary>
        /// <param name="dataid"></param>
        /// <returns></returns>
        public IList GetActivityEvaluateList(string dataid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<ActivityEvaluateEntity>()
                        join q2 in db.IQueryable<UserEntity>() on q1.CREATEUSERID equals q2.UserId into into1
                        from t1 in into1.DefaultIfEmpty()
                        where q1.Activityid == dataid
                        orderby q1.CREATEDATE descending
                        select new { DepartmentName=q1.DeptName, CreateUser = t1.RealName, q1.Score, q1.EvaluateContent, CreateDate = q1.CREATEDATE };

            return query.ToList();
        }

        /// <summary>
        /// 查询用户的历史记录
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="deptid">部门Id</param>
        /// <returns></returns>
        public List<SevensPictureEvaluationEntity> GetHistory(string userId, string deptid)
        {
            var data = this.BaseRepository().IQueryable(p => p.CreateUser == userId && p.EvaluateDataId == deptid);
            return data.ToList();
        }

        public IList GetHistory(string userId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<ActivityEvaluateEntity>()
                        join q2 in db.IQueryable<UserEntity>() on q1.CREATEUSERID equals q2.UserId into into1
                        from t1 in into1.DefaultIfEmpty()
                        where q1.CREATEUSERID == userId
                        orderby q1.CREATEDATE descending
                        select new { DepartmentName = q1.DeptName, CreateUser = t1.RealName, q1.Score, q1.EvaluateContent, CreateDate = q1.CREATEDATE };

            return query.Take(10) .ToList();
        }

        /// <summary>
        /// 获取某条定点拍照数据的所有评论
        /// </summary>
        /// <param name="dataid"></param>
        /// <returns></returns>
        public List<SevensPictureEvaluationEntity> GetList(string dataid)
        {
            var data = this.BaseRepository().IQueryable(p => p.EvaluateDataId == dataid);
            return data.ToList();
        }
      
        public void Insert(SevensPictureEvaluationEntity evaluationEntity)
        {
            this.BaseRepository().Insert(evaluationEntity);
        }
    }
}
