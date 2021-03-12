using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.DeptCycleTaskManage;
using BSFramework.Application.IService.DeptCycleTaskManage;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.DeptCycleTaskManage
{

    /// <summary>
    ///部门任务
    /// </summary>
    public class DeptWorkCycleTaskService : IDeptWorkCycleTaskServcie
    {
        #region 查询数据
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <param name="userid">用户</param>
        /// <returns></returns>
        public List<DeptWorkCycleTaskEntity> GetPageList(Pagination pagination, string queryJson, string userid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var queryParam = queryJson.ToJObject();
            var Expression = LinqExtensions.True<DeptWorkCycleTaskEntity>();
            var ExpressionOther = LinqExtensions.True<DeptWorkCycleTaskEntity>();

            var user = db.FindEntity<UserEntity>(userid);
            if (user.UserId == "System")
            {
                user.DepartmentCode = "0";
            }
            Expression = Expression.And(x => x.deptcode.StartsWith(user.DepartmentCode));
            if (!queryParam["content"].IsEmpty())
            {
                var content = queryParam["content"].ToString();
                Expression = Expression.And(x => x.content.Contains(content));
            }
            if (!queryParam["dutyuserid"].IsEmpty())
            {
                var dutyuserid = queryParam["dutyuserid"].ToString();
                Expression = Expression.And(x => x.dutyuserid == dutyuserid);
            }
            if (!queryParam["starttime"].IsEmpty())
            {
                var starttime = Convert.ToDateTime(queryParam["starttime"].ToString());
                if (!queryParam["endtime"].IsEmpty())
                {
                    var endtime = Convert.ToDateTime(queryParam["endtime"].ToString());
                    Expression = Expression.And(x => (x.starttime >= starttime && x.endtime >= starttime)||(x.starttime <= endtime && x.endtime >= endtime));
                }
                else
                {
                    Expression = Expression.And(x => (x.starttime >= starttime && x.endtime >= starttime));

                }

            }
            else
            {
                if (!queryParam["endtime"].IsEmpty())
                {
                    var endtime = Convert.ToDateTime(queryParam["endtime"].ToString());
                    Expression = Expression.And(x => x.starttime <= endtime && x.endtime >= endtime);
                }
            }

        
            if (!queryParam["workstate"].IsEmpty())
            {
                var workstate = queryParam["workstate"].ToString();
                Expression = Expression.And(x => x.workstate == workstate);
            }
            ExpressionOther = Expression;
            ExpressionOther = ExpressionOther.And(x => x.dutyuserid != userid);
            Expression = Expression.And(x => x.dutyuserid == userid);

            var entityUser = db.FindList(Expression).OrderByDescending(x => x.endtime).ThenByDescending(x => x.starttime).ToList();
            var entityOther = db.FindList(ExpressionOther).OrderByDescending(x => x.endtime).ToList();
            entityUser.AddRange(entityOther);
            pagination.records = entityUser.Count();
            var entity = entityUser.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
            return entity.ToList();
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public DeptWorkCycleTaskEntity getEntity(string keyvalue)
        {
            var db = new RepositoryFactory().BaseRepository();
            var entity = db.FindEntity<DeptWorkCycleTaskEntity>(keyvalue);
            return entity;
        }

        #endregion

        #region 操作数据

        /// <summary>
        ///批量  新增 修改
        /// </summary>
        /// <param name="Listentity"></param>
        /// <param name="userid"></param>
        public void SaveForm(List<DeptWorkCycleTaskEntity> Listentity, string userid)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var user = db.FindEntity<UserEntity>(userid);
                foreach (var entity in Listentity)
                {
                    if (entity.id.IsEmpty())
                    {
                        entity.id = Guid.NewGuid().ToString();
                        entity.modifytime = DateTime.Now;
                        entity.modifyuser = user.RealName;
                        entity.modifyuserid = user.UserId;

                        db.Insert(entity);
                    }
                    else
                    {
                        var oldentity = db.FindEntity<DeptWorkCycleTaskEntity>(entity.id);
                        if (oldentity != null)
                        {
                            oldentity.content = entity.content;

                            oldentity.modifytime = DateTime.Now;
                            oldentity.modifyuser = user.RealName;
                            oldentity.modifyuserid = user.UserId;
                            db.Update(oldentity);

                        }
                    }
                }

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }

        }
        /// <summary>
        /// 新增 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        public void SaveForm(DeptWorkCycleTaskEntity entity, string userid)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var user = db.FindEntity<UserEntity>(userid);
                if (entity.id.IsEmpty())
                {
                    entity.id = Guid.NewGuid().ToString();
                    entity.deptcode = user.DepartmentCode;
                    entity.deptid = user.DepartmentId;
                    entity.deptname = user.DepartmentName;
                    entity.modifytime = DateTime.Now;
                    entity.modifyuser = user.RealName;
                    entity.modifyuserid = user.UserId;

                    db.Insert(entity);
                }
                else
                {
                    var oldentity = db.FindEntity<DeptWorkCycleTaskEntity>(entity.id);
                    if (oldentity != null)
                    {

                        oldentity.workstate = entity.workstate;
                        oldentity.modifytime = DateTime.Now;
                        oldentity.modifyuser = user.RealName;
                        oldentity.modifyuserid = user.UserId;
                        db.Update(oldentity);

                    }
                }
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }

        }
        /// <summary>
        /// 删除
        /// </summary>
        public void deleteEntity(string keyvalue)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = db.FindEntity<DeptWorkCycleTaskEntity>(keyvalue);
                if (entity != null)
                {
                    db.Delete(entity);
                }
                db.Commit();

            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }

        }
        #endregion
    }
}
