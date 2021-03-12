using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.WebApp;
using BSFramework.Application.IService.WebApp;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.WebApp
{
    public class UserExperiencService : RepositoryFactory<UserExperiencEntity>, IUserExperiencService
    {
        /// <summary>
        /// 根据userid查询
        /// </summary>
        /// <returns></returns>
        public List<UserExperiencEntity> SelectByUserId(string userid)
        {
            try
            {
                return this.BaseRepository().IQueryable().Where(x => x.createuserid == userid).ToList();
            }
            catch (Exception)
            {

                throw;
            }

        }
        /// <summary>
        /// 获取明细
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public UserExperiencEntity SelectDetail(string Id)
        {
            try
            {
                return this.BaseRepository().IQueryable().FirstOrDefault(x => x.ExperiencId == Id);
            }
            catch (Exception)
            {

                throw;
            }

        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        public void add(UserExperiencEntity entity)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {

                if (!entity.isupdate)
                {
                    var get = db.IQueryable<UserExperiencEntity>().Where(x => x.isupdate == false & x.createuserid == entity.createuserid).OrderByDescending(x => x.createtime).ToList();
                    if (get.Count() > 0)
                    {
                        var one = get[0];
                        one.EndTime = entity.StartTime;
                        db.Update(one);
                    }
                    //var user = db.IQueryable<UserEntity>().FirstOrDefault(x => x.UserId == entity.createuserid);
                    //获取父节点  部门  人员在班组中 获取到上级部门 则再上一层为厂级   
                    //var dept = db.IQueryable<DepartmentEntity>().FirstOrDefault(x => x.DepartmentId == user.DepartmentId);
                    //if (dept.Nature=="厂级")
                    //{
                    //    entity.Commpany = dept.FullName;
                    //}
                    //else
                    //{
                    //    var Pdept = db.IQueryable<DepartmentEntity>().FirstOrDefault(x => x.DepartmentId == dept.DepartmentId);
                    //    entity.Commpany = Pdept.FullName;
                    //}
                }
                entity.Create();
                db.Insert(entity);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 获取明细
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public void delete(string Id)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {

                var one = db.IQueryable<UserExperiencEntity>().FirstOrDefault(x => x.ExperiencId == Id);
                db.Delete(one);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        public void update(UserExperiencEntity entity)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {

                db.Update(entity);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }



    }
}
