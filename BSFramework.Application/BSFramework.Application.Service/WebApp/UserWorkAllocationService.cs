using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PeopleManage;
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
    /// <summary>
    /// UserWorkAllocationEntity 用户转岗记录表
    /// </summary>
    public class UserWorkAllocationService : RepositoryFactory<UserWorkAllocationEntity>, IUserWorkAllocationService
    {
        /// <summary>
        /// 获取转移到部门的成员
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserWorkAllocationEntity> GetSendUser(string deptId)
        {
            return this.BaseRepository().IQueryable().Where(x => x.iscomplete == false && x.departmentid == deptId);
        }
        /// <summary>
        /// 获取未推送完成的成员
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public IEnumerable<UserWorkAllocationEntity> GetIsSendUser(string deptId)
        {
            return this.BaseRepository().IQueryable().Where(x => x.iscomplete == false && x.olddepartmentid == deptId);
        }


        /// <summary>
        /// 获取详情
        /// </summary>
        /// <returns></returns>
        public UserWorkAllocationEntity GetDetail(string id)
        {
            return this.BaseRepository().IQueryable().FirstOrDefault(x => x.Id == id);
        }


        /// <summary>
        /// 获取详情
        /// </summary>
        /// <returns></returns>
        public UserWorkAllocationEntity GetDetailByUser(string id)
        {
            return this.BaseRepository().IQueryable().FirstOrDefault(x => x.userId == id && x.iscomplete == false);
        }



        /// <summary>
        /// 增改实体
        /// </summary>
        /// <param name="entity"></param>
        public string OperationEntity(UserWorkAllocationEntity entity, string TID)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                if (string.IsNullOrEmpty(entity.Id))
                {
                    entity.Id = TID;
                    db.Insert(entity);
                }
                else
                {
                    db.Update(entity);
                }

                db.Commit();
                return entity.Id;
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 获取部门树
        /// </summary>
        /// <param name="id"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<DepartmentEntity> GetSubDepartments(string id, string category)
        {
            var db = new RepositoryFactory().BaseRepository();

            //var current = from q in db.IQueryable<DepartmentEntity>()
            //              where q.DepartmentId == id
            //              select q;
            IEnumerable<DepartmentEntity> current = new List<DepartmentEntity>();
            var subquery = from q in db.IQueryable<DepartmentEntity>()
                           where q.ParentId == id
                           select q;

            var list = default(List<string>);

            if (!string.IsNullOrEmpty(category))
            {
                list = category.Split(',').ToList();
                // subquery = subquery.Where(x => list.Contains(x.Nature));
            }

            while (subquery.Count() > 0)
            {
                current = current.Concat(subquery);
                subquery = from q in subquery
                           join q1 in db.IQueryable<DepartmentEntity>() on q.DepartmentId equals q1.ParentId
                           select q1;

                // if (list != null && list.Count > 0) subquery = subquery.Where(x => list.Contains(x.Nature));
            }
            if (list != null && list.Count > 0) current = current.Where(x => list.Contains(x.Nature));
            return current.ToList();
        }

        /// <summary>
        /// 离厂数据操作
        /// </summary>
        public void Operationleave(UserWorkAllocationEntity entity)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {

                var people = db.IQueryable<PeopleEntity>().FirstOrDefault(x => x.ID == entity.userId);
                var user = db.IQueryable<UserEntity>().FirstOrDefault(x => x.UserId == entity.userId);
                //user.DepartmentId = "";
                //user.DepartmentId = "";
                //user.EnabledMark = 0;
                //离厂修改用户状态
                db.Delete(user);
                //db.Update(user);
                //离厂删除people信息
                db.Delete(people);
                if (!string.IsNullOrEmpty(entity.Id))
                {
                    db.Update(entity);
                }

                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

        }
        /// <summary>
        /// 转岗数据操作
        /// </summary>
        public void OperationJobs(UserWorkAllocationEntity entity)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var dept = db.IQueryable<DepartmentEntity>().FirstOrDefault(x => x.DepartmentId == entity.departmentid);
                //修改people
                var people = db.IQueryable<PeopleEntity>().FirstOrDefault(x => x.ID == entity.userId);
                people.Quarters = entity.quarters;
                people.QuarterId = entity.quartersid;
                people.Planer = entity.Code;
                people.RoleDutyName = entity.RoleDutyName;
                people.RoleDutyId = entity.RoleDutyId;
                people.Files = null;
                entity.Code = null;
                //修改user
                var user = db.IQueryable<UserEntity>().FirstOrDefault(x => x.UserId == entity.userId);
                if (dept.Nature == "班组")
                {
                    people.BZID = entity.departmentid;
                    people.BZName = entity.department;
                    var parent = db.IQueryable<DepartmentEntity>().FirstOrDefault(x => x.DepartmentId == dept.ParentId);
                    people.DeptId = parent.DepartmentId;
                    people.DeptName = parent.FullName; people.DeptCode = parent.EnCode;
                    user.DepartmentId = entity.departmentid;
                    user.DepartmentCode = dept.EnCode;
                }
                else
                {
                    //转部门
                    people.BZID = string.Empty;
                    people.BZName = string.Empty;
                    people.DeptId = dept.DepartmentId;
                    people.DeptName = dept.FullName;
                    people.DeptCode = dept.EnCode;
                    user.DepartmentId = dept.DepartmentId;
                    user.DepartmentCode = dept.EnCode;
                }
                db.Update(user);
                db.Update(people);
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
