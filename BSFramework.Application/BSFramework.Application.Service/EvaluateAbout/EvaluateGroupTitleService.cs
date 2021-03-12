using BSFramework.Application.Entity.EvaluateAbout;
using BSFramework.Application.IService.EvaluateAbout;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.EvaluateAbout
{
    public class EvaluateGroupTitleService : RepositoryFactory<EvaluateGroupTitleEntity>, IEvaluateGroupTitleService
    {
        /// <summary>
        /// 查找称号信息
        /// </summary>
        /// <param name="groupId">班组ID</param>
        /// <param name="evaluateId">考评ID</param>
        /// <returns></returns>
        public EvaluateGroupTitleEntity GetEntity(string titleId )
        {
            return this.BaseRepository().FindEntity(titleId);
        }
        /// <summary>
        /// 根据班组的ID，获取班组的称号
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="evaluateId">考评Id</param>
        /// <returns></returns>
        public string GetTitleNameByGroupId(string deptId,string evaluateId)
        {
            //var queryTitle = new RepositoryFactory().BaseRepository().IQueryable<DesignationEntity>();
            //var nameQuery = from t1 in this.BaseRepository().IQueryable()
            //            join t2 in queryTitle on t1.TitleId equals t2.Id
            //            where t1.GroupId == deptId && t2.IsFiring == 1
            //            select new { TitleName = t2.ClassName };

            //var title= this.BaseRepository().IQueryable().Where(x => x.GroupId == deptId).Join(queryTitle, a => a.TitleId, b => b.Id, (a, b) => new { TitleName = b.ClassName }).FirstOrDefault();

            var db = new RepositoryFactory().BaseRepository();
            var query = from a in db.IQueryable<EvaluateGroupTitleEntity>()
                        join b in db.IQueryable<DesignationEntity>() on a.TitleId equals b.Id
                        where a.GroupId == deptId && b.IsFiring == 1 && a.EvaluateId == evaluateId
                        select new { b.ClassName };

            //  string sql = "SELECT ClassName from WG_EvaluateGroupTitle a inner join wg_Designation b on a.TitleId=b.Id where a.GroupId='" + deptId + "' and b.IsFiring=1 AND A.BK1='"+evaluateId+"'";
            //object obj =   new RepositoryFactory().BaseRepository().FindObject(sql);
            if (query.Count()==0)
            {
                return null;
            }
            else
            {
                return query.First().ClassName;
            }
            //var title = nameQuery.FirstOrDefault();
            //return obj == null ? "" : obj.ToString();
        }

        public void Insert(EvaluateGroupTitleEntity entity)
        {
            entity.Create();
            this.BaseRepository().Insert(entity);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="groupId">班组的Id</param>
        /// <param name="bk1">考评的ID</param>
        public void Remove(string groupId,string bk1)
        {
            this.BaseRepository().Delete(x=>x.EvaluateId==bk1 && x.GroupId==groupId);
        }

        public void Remove(string titleId)
        {
            this.BaseRepository().Delete(titleId);
        }

        public void Update(EvaluateGroupTitleEntity oldEntity)
        {
            oldEntity.Modify(oldEntity.GroupId);
            this.BaseRepository().Update(oldEntity);
        }
    }
}
