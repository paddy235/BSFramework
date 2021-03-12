using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PerformanceManage;
using BSFramework.Application.IService.PerformanceManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.PerformanceManage
{
    /// <summary>
    /// 绩效管理配置
    /// </summary>
    public class PerformancesetupSecondService : RepositoryFactory<PerformancesetupSecondEntity>, IPerformancesetupSecondService
    {

        /// <summary>
        /// 获取所有配置
        /// </summary>
        /// <returns></returns>
        public List<PerformancesetupSecondEntity> AllTitle(string departmentid)
        {
            return this.BaseRepository().IQueryable().Where(x => x.departmentid == departmentid).OrderBy(x => x.sort).ToList();
        }
        /// <summary>
        /// 操作配置  对应修改当前月标题和数据
        /// </summary>
        public void operation(List<PerformancesetupSecondEntity> add, List<PerformancesetupSecondEntity> del, List<PerformancesetupSecondEntity> Listupdate, PerformancetitleSecondEntity title, List<PerformanceSecondEntity> Score, PerformancePersonSecondEntity person)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                if (add.Count > 0)
                {
                    db.Insert(add);
                }
                if (del.Count > 0)
                {
                    db.Delete(del);
                }
                if (Listupdate.Count > 0)
                {
                    db.Update(Listupdate);
                }

                //标题头数据操作
                if (title != null)
                {
                    if (title.name != null)
                    {
                        var one = db.FindEntity<PerformancetitleSecondEntity>(title.titleid);
                        if (one != null)
                        {
                            one.name = title.name;
                            one.sort = title.sort;
                            db.Update(one);

                        }
                        else
                        {

                            db.Insert(title);
                        }

                    }


                }
                //数据列表数据操作
                if (Score.Count > 0)
                {
                    foreach (var item in Score)
                    {
                        if (string.IsNullOrEmpty(item.performanceid))
                        {
                            item.performanceid = Guid.NewGuid().ToString();
                            db.Insert(item);
                        }
                        else
                        {
                            db.Update(item);
                        }
                    }


                }
                if (person != null)
                {
                    if (string.IsNullOrEmpty(person.Id))
                    {
                        person.Id = Guid.NewGuid().ToString();
                        db.Insert(person);
                    }
                    else
                    {
                        db.Update(person);
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
        /// 清理数据
        /// </summary>
        /// <param name="del"></param>
        public void remove(List<PerformanceSecondEntity> del)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Delete(del);
                db.Commit();
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// 获取所有人员配置
        /// </summary>
        /// <returns></returns>
        public PerformancePersonSecondEntity getDeptPerson(string departmentid)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.FindEntity<PerformancePersonSecondEntity>(x => x.departmentid == departmentid);
        }
        /// <summary>
        /// 修改新增人员信息
        /// </summary>
        /// <param name="entity"></param>
        public void SetDeptPerson(PerformancePersonSecondEntity entity, UserEntity user)
        {

            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                entity.modifyuser = user.RealName;
                entity.modifydate = DateTime.Now;
                entity.modifyuserid = user.UserId;
                if (string.IsNullOrEmpty(entity.Id))
                {
                    entity.Id = Guid.NewGuid().ToString();
                    entity.createtime = DateTime.Now;
                    entity.departmentid = user.DepartmentId;
                    db.Insert(entity);
                }
                else
                {
                    db.Update(entity);
                }
                db.Commit();
            }
            catch (Exception)
            {

                db.Rollback();
            }
        }
        /// <summary>
        ///获取特殊配置列表
        /// </summary>
        /// <returns></returns>
        public List<PerformanceMethodSecondEntity> getisspecial(string performancetypeid)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.FindList<PerformanceMethodSecondEntity>(x => x.performancetypeid == performancetypeid).ToList();
        }
        /// <summary>
        /// 新增特殊配置
        /// </summary>
        /// <param name="entity"></param>
        public void Setisspecial(List<PerformanceMethodSecondEntity> entity)
        {

            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                foreach (var item in entity)
                {
                    if (string.IsNullOrEmpty(item.Id))
                    {
                        item.Id = Guid.NewGuid().ToString();
                        db.Insert(entity);
                    }
                    else
                    {
                        db.Update(entity);
                    }
                }

                db.Commit();
            }
            catch (Exception)
            {

                db.Rollback();
            }
        }
    }
}
