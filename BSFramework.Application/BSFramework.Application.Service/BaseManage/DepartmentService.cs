using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.IService.BaseManage;
using BSFramework.Data;
using BSFramework.Data.EF;
using BSFramework.Data.Repository;
using BSFramework.Util.Extension;
using Bst.Bzzd.Sync;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace BSFramework.Application.Service.BaseManage
{
    /// <summary>
    /// 描 述：部门管理
    /// </summary>
    public class DepartmentService : RepositoryFactory<DepartmentEntity>, IDepartmentService
    {
        #region 获取数据
        /// <summary>
        /// 部门列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DepartmentEntity> GetList()
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<DepartmentEntity>()
                        join q2 in db.IQueryable<DepartmentEntity>() on q1.ParentId equals q2.DepartmentId into into1
                        from q3 in into1.DefaultIfEmpty()
                        orderby q3.EnCode, q1.EnCode
                        select q1;
            var data = query.ToList();
            return data;
        }
        public List<DepartmentEntity> GetChildGroup(string code)
        {
            var db = new RepositoryFactory().BaseRepository();
            //var sql = "select departmentid,encode,fullname from base_department where encode like '" + code + "%' and nature = '班组'";



            //DataTable dt = db.FindTable(sql);
            var data = db.IQueryable<DepartmentEntity>(p => p.EnCode.StartsWith(code) && p.Nature == "班组").ToList();
            return data;
        }
        /// <summary>
        /// 部门列表通讯录
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DepartmentEntity> GetDepartmentList(string keyValue)
        {
            var db = new RepositoryFactory().BaseRepository();
            if (string.IsNullOrEmpty(keyValue))
            {
                var query = db.IQueryable<DepartmentEntity>().Where(x => x.ParentId == "0").ToList();
                return query;
            }
            else
            {
                var query = from q1 in db.IQueryable<DepartmentEntity>().Where(x => x.ParentId == keyValue)
                            orderby q1.EnCode
                            select q1;
                return query.ToList();
            }
        }
        /// <summary>
        /// 获取部门人数
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public int GetPeopleNumber(string keyValue)
        {

            var db = new RepositoryFactory().BaseRepository();

            var categorygroups = from q1 in db.IQueryable<DepartmentEntity>().Where(x => x.DepartmentId == keyValue)
                                 join q2 in db.IQueryable<DepartmentEntity>() on q1.ParentId equals q2.DepartmentId
                                 select q1;
            var categories = from q1 in categorygroups
                             select new { DepartmentId = q1.DepartmentId, ParentId = q1.DepartmentId };

            var current = from q1 in db.IQueryable<DepartmentEntity>()
                          join q2 in categorygroups on q1.ParentId equals q2.DepartmentId
                          select new { DepartmentId = q1.DepartmentId, ParentId = q2.DepartmentId };

            while (current.Count() > 0)
            {
                categories = categories.Concat(current);

                current = from q1 in current
                          join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.ParentId
                          select new { DepartmentId = q2.DepartmentId, ParentId = q1.ParentId };
            }
            var i = from q1 in db.IQueryable<UserEntity>()
                    join q2 in categories on q1.DepartmentId equals q2.DepartmentId
                    select new { q1.DepartmentId, q2.ParentId };
            return i.ToList().Count();
        }
        /// <summary>
        /// 部门实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public DepartmentEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 部门编号不能重复
        /// </summary>
        /// <param name="enCode">编号</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistEnCode(string enCode, string keyValue)
        {
            var expression = LinqExtensions.True<DepartmentEntity>();
            expression = expression.And(t => t.EnCode == enCode);
            if (!string.IsNullOrEmpty(keyValue))
            {
                expression = expression.And(t => t.DepartmentId != keyValue);
            }
            return this.BaseRepository().IQueryable(expression).Count() == 0 ? true : false;
        }
        /// <summary>
        /// 部门名称不能重复
        /// </summary>
        /// <param name="fullName">名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistFullName(string fullName, string keyValue)
        {
            var expression = LinqExtensions.True<DepartmentEntity>();
            expression = expression.And(t => t.FullName == fullName);
            if (!string.IsNullOrEmpty(keyValue))
            {
                expression = expression.And(t => t.DepartmentId != keyValue);
            }
            return this.BaseRepository().IQueryable(expression).Count() == 0 ? true : false;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            int count = this.BaseRepository().IQueryable(t => t.ParentId == keyValue || t.OrganizeId == keyValue).Count();
            //if (count > 0)
            //{
            //    throw new Exception("当前所选数据有子节点数据！");
            //}
            this.BaseRepository().Delete(keyValue);

            new UserService().removemore(keyValue);
        }
        /// <summary>
        /// 保存部门表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="departmentEntity">机构实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, DepartmentEntity departmentEntity)
        {
            //培训平台同步所有部门
            var syncall = false;
            if (departmentEntity.Description == "syncall")
            {
                syncall = true;
                departmentEntity.Description = null;
            }

            if (!string.IsNullOrEmpty(keyValue))
            {
                DepartmentEntity curEntity = this.BaseRepository().FindEntity(keyValue);  //获取父对象
                //if (curEntity.OrganizeId != departmentEntity.OrganizeId || string.IsNullOrEmpty(curEntity.EnCode))
                //{
                //    departmentEntity.EnCode = GetDepartmentCode(departmentEntity);
                //}
                if (curEntity == null)
                {
                    departmentEntity.EnCode = string.IsNullOrEmpty(departmentEntity.EnCode) ? GetDepartmentCode(departmentEntity) : departmentEntity.EnCode;
                    departmentEntity.Create();
                    this.BaseRepository().Insert(departmentEntity);
                }
                else
                {
                    departmentEntity.Modify(keyValue);
                    this.BaseRepository().Update(departmentEntity);
                }


            }
            else
            {
                departmentEntity.EnCode = GetDepartmentCode(departmentEntity);
                departmentEntity.Create();
                this.BaseRepository().Insert(departmentEntity);
            }

            //培训平台同步
            if (syncall)
                this.SyncTraing(null);
            else
                this.SyncTraing(keyValue);
        }

        private void SyncTraing(string keyValue)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<DepartmentEntity>()
                        select q;

            if (!string.IsNullOrEmpty(keyValue))
            {
                query = from q in query
                        where q.DepartmentId == keyValue
                        select q;
            }

            var depts = query.ToList();

            var dict = new Dictionary<DepartmentEntity, DepartmentEntity>();
            foreach (var item in depts)
            {
                var pdept = (from q in db.IQueryable<DepartmentEntity>()
                             where q.DepartmentId == item.ParentId
                             select q).FirstOrDefault();
                dict.Add(item, pdept);
            }

            var sync = new TrainingSystem();
            sync.SyncDept(dict);
        }
        #endregion


        /// <summary>
        /// 根据当前机构获取对应的机构代码  机构代码 2-6-8-10  位
        /// </summary>
        /// <param name="organizeEntity"></param>
        /// <returns></returns>
        public string GetDepartmentCode(DepartmentEntity departmentEntity)
        {
            string maxCode = string.Empty;

            OrganizeEntity oEntity = new OrganizeService().BaseRepository().FindEntity(departmentEntity.OrganizeId);  //获取父对象

            //查询是否存在平级部门
            var maxObj = BaseRepository().IQueryable(x => x.OrganizeId == departmentEntity.OrganizeId && x.ParentId == departmentEntity.ParentId && !string.IsNullOrWhiteSpace(x.EnCode)).OrderByDescending(x => x.EnCode).ToList();
            //确定是否存在上级部门,非部门根节点
            if (departmentEntity.ParentId != "0")
            {
                //存在，则取编码最大的那一个
                if (maxObj.Count() > 0)
                {
                    maxCode = maxObj.FirstOrDefault().EnCode;  //获取最大的Code 
                    if (!string.IsNullOrEmpty(maxCode))
                    {
                        maxCode = new OrganizeService().QueryOrganizeCodeByCondition(maxCode);
                    }
                }
                else
                {
                    DepartmentEntity parentEntity = this.BaseRepository().FindEntity(departmentEntity.ParentId);  //获取父对象

                    maxCode = parentEntity.EnCode + "001";  //固定值,非可变
                }
            }
            else  //部门根节点的操作
            {
                //该机构id 并且 parentid = 0（属于部门根节点）
                maxObj = BaseRepository().IQueryable(x => x.OrganizeId == departmentEntity.OrganizeId && x.ParentId == "0" && !string.IsNullOrWhiteSpace(x.EnCode)).OrderByDescending(x => x.EnCode).ToList();
                //do somethings 
                if (maxObj.Count() > 0)
                {
                    maxCode = maxObj.FirstOrDefault().EnCode;  //获取最大的Code 

                    if (!string.IsNullOrEmpty(maxCode))
                    {
                        maxCode = new OrganizeService().QueryOrganizeCodeByCondition(maxCode);
                    }
                    else
                    {
                        maxCode = oEntity.EnCode + "001";
                    }
                }
                else
                {
                    maxCode = oEntity.EnCode + "001";  //固定值,非可变,加机构编码，组合新编码
                }
            }
            //先判断当前编码是否存在于数据库表中
            return maxCode;
        }

        public DepartmentEntity GetRootDepartment()
        {
            var dept = this.BaseRepository().IQueryable().FirstOrDefault(x => x.ParentId == "0");
            return dept;
        }

        public List<DepartmentEntity> GetSubDepartments(string id, string category)
        {
            var list = default(List<string>);

            if (!string.IsNullOrEmpty(category))
                list = category.Split(',').ToList();

            var db = new RepositoryFactory().BaseRepository();

            var current = from q in db.IQueryable<DepartmentEntity>()
                          where q.DepartmentId == id
                          select q;

            var subquery = from q1 in current
                           join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.ParentId
                           select q2;

            while (subquery.Count() > 0)
            {
                current = current.Concat(subquery);
                subquery = from q1 in subquery
                           join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.ParentId
                           select q2;
            }

            if (list != null && list.Count > 0) current = current.Where(x => list.Contains(x.Nature));

            return current.OrderBy(x => x.EnCode).ToList();

        }

        public IList<DepartmentEntity> GetAllGroups()
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<DepartmentEntity>()
                        join q2 in db.IQueryable<DepartmentEntity>() on q1.ParentId equals q2.DepartmentId into into1
                        from q3 in into1.DefaultIfEmpty()
                        where q1.Nature == "班组"
                        orderby q3.EnCode, q1.EnCode
                        select q1;
            return query.ToList();
        }

        public string GetFactory(string userId)
        {
            var db = new RepositoryFactory().BaseRepository();

            var dept = (from q1 in db.IQueryable<DepartmentEntity>()
                        join q2 in db.IQueryable<UserEntity>() on q1.DepartmentId equals q2.DepartmentId
                        where q2.UserId == userId
                        select q1).FirstOrDefault();

            if (dept == null) return string.Empty;

            while (dept != null && dept.Nature != "厂级")
            {
                dept = (from q in db.IQueryable<DepartmentEntity>()
                        where q.DepartmentId == dept.ParentId
                        select q).FirstOrDefault();
            }

            return dept == null ? string.Empty : dept.FullName;
        }

        public List<DepartmentEntity> GetDepartments(string deptid)
        {
            var db = new RepositoryFactory().BaseRepository();

            var current = from q in db.IQueryable<DepartmentEntity>()
                          where q.DepartmentId == deptid
                          select q;

            var subquery = from q1 in current
                           join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.ParentId
                           select q2;

            while (subquery.Count() > 0)
            {
                current = current.Concat(subquery);
                subquery = from q1 in subquery
                           join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.ParentId
                           select q2;
            }

            return current.ToList();
        }

        public DepartmentEntity GetAuthorizationDepartment(string deptid)
        {
            var db = new RepositoryFactory().BaseRepository();

            var current = from q in db.IQueryable<DepartmentEntity>()
                          where q.DepartmentId == deptid
                          select q;

            if (deptid == "0")
            {
                current = from q in db.IQueryable<DepartmentEntity>()
                          where q.ParentId == "0"
                          select q;
            }

            var dept = current.FirstOrDefault();
            if (dept == null) return null;

            if (dept.IsSpecial)
            {
                while (dept.Nature != "厂级" && dept.ParentId != "0")
                {
                    current = from q in db.IQueryable<DepartmentEntity>()
                              where q.DepartmentId == dept.ParentId
                              select q;
                    dept = current.FirstOrDefault();
                }
            }

            if (!string.IsNullOrEmpty(dept.ParentId) && (dept.Nature != "厂级" && dept.Nature != "省级" && dept.Nature != "集团"))
            {
                var parent = db.IQueryable<DepartmentEntity>().FirstOrDefault(x => x.DepartmentId == dept.ParentId);
                if (parent.Nature == "省级" || parent.Nature == "集团")
                {
                    dept = parent;
                }
            }

            return dept;
        }

        public DepartmentEntity GetAuthorizationDepartmentApp(string deptid)
        {
            var db = new RepositoryFactory().BaseRepository();

            var current = from q in db.IQueryable<DepartmentEntity>()
                          where q.DepartmentId == deptid
                          select q;

            if (deptid == "0")
            {
                current = from q in db.IQueryable<DepartmentEntity>()
                          where q.ParentId == "0"
                          select q;
            }

            var dept = current.FirstOrDefault();
            if (dept == null) return null;


            while (dept.Nature != "厂级" && dept.ParentId != "0")
            {
                current = from q in db.IQueryable<DepartmentEntity>()
                          where q.DepartmentId == dept.ParentId
                          select q;
                dept = current.FirstOrDefault();
            }


            return dept;
        }
        public List<DepartmentEntity> GetChildDepartments(string deptid)
        {
            var depts = this.BaseRepository().IQueryable().Where(x => x.ParentId == deptid).ToList();
            return depts;
        }

        public int GetSafeDays(string deptid)
        {
            var dept = this.GetEntity(deptid);
            if (dept == null) return 0;

            return (DateTime.Now - dept.StartDate).Days;
        }

        public object GetSubTeams(string departmentId)
        {
            var db = new RepositoryFactory().BaseRepository();

            var current = from q in db.IQueryable<DepartmentEntity>()
                          where q.DepartmentId == departmentId
                          select q;

            var subquery = from q1 in current
                           join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.ParentId
                           select q2;

            while (subquery.Count() > 0)
            {
                current = current.Concat(subquery);
                subquery = from q1 in subquery
                           join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.ParentId
                           select q2;
            }

            current = current.Where(x => x.Nature == "班组");

            var query = from q1 in current
                        join q2 in db.IQueryable<DepartmentEntity>() on q1.ParentId equals q2.DepartmentId
                        orderby q1.EnCode
                        select new { q1.DepartmentId, DeptName = q1.FullName, q1.ParentId, ParentName = q2.FullName };

            return query.ToList();
        }

        /// <summary>
        /// 获取某单位下所有的班组
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public List<DepartmentEntity> GetSubGroups(string departmentId)
        {
            var db = new RepositoryFactory().BaseRepository();

            var current = from q in db.IQueryable<DepartmentEntity>()
                          where q.DepartmentId == departmentId
                          select q;

            var subquery = from q1 in current
                           join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.ParentId
                           select q2;

            while (subquery.Count() > 0)
            {
                current = current.Concat(subquery);
                subquery = from q1 in subquery
                           join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.ParentId
                           select q2;
            }

            current = current.Where(x => x.Nature == "班组");

            var query = from q1 in current
                        join q2 in db.IQueryable<DepartmentEntity>() on q1.ParentId equals q2.DepartmentId
                        orderby q1.EnCode
                        select q1;

            return query.ToList();
        }

        public List<DepartmentEntity> GetSubDepartments(string[] depts)
        {
            var db = new RepositoryFactory().BaseRepository();

            var current = from q in db.IQueryable<DepartmentEntity>()
                          where depts.Contains(q.DepartmentId)
                          select q;

            var subquery = from q1 in current
                           join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.ParentId
                           select q2;

            while (subquery.Count() > 0)
            {
                current = current.Concat(subquery);
                subquery = from q1 in subquery
                           join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.ParentId
                           select q2;
            }

            return current.OrderBy(x => x.EnCode).ToList();
        }

        public List<DepartmentEntity> GetDepartments(string[] depts)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<DepartmentEntity>()
                        where depts.Contains(q.DepartmentId)
                        orderby q.EnCode
                        select q;

            return query.ToList();
        }

        /// <summary>
        /// 根据用户所在部门获取该部门下所有的班组
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<DepartmentEntity> GetGroupsByUser(string userId)
        {
            UserEntity user = new RepositoryFactory().BaseRepository().FindEntity<UserEntity>(userId);
            DepartmentEntity userDept = this.BaseRepository().FindEntity(user.DepartmentId);
            if (user == null) return null;
            List<DepartmentEntity> deptlist;
            //判断是否是特殊部门
            if (userDept.IsSpecial)
            {
                var db = new RepositoryFactory().BaseRepository();
                var dept = userDept;
                //特殊部门查全厂
                while (dept != null && dept.Nature != "厂级")
                {
                    dept = (from q in db.IQueryable<DepartmentEntity>()
                            where q.DepartmentId == dept.ParentId
                            select q).FirstOrDefault();
                }
                //查询所有的班组
                deptlist = db.IQueryable<DepartmentEntity>(p => p.EnCode.StartsWith(dept.EnCode) && p.Nature == "班组").ToList();
            }
            else
            {
                deptlist = this.BaseRepository().IQueryable(p => p.EnCode.StartsWith(user.DepartmentCode) && p.Nature == "班组").ToList();
            }
            return deptlist;
        }

        public void Save(List<DepartmentEntity> depts)
        {
            var db = DbFactory.Base() as Database;
            var dbset = db.dbcontext.Set<DepartmentEntity>();

            foreach (var item in depts)
            {
                var entity = dbset.Find(item.DepartmentId);
                if (entity == null) dbset.Add(item);
                else
                {
                    db.dbcontext.Entry(entity).State = System.Data.Entity.EntityState.Modified;

                    entity.ParentId = item.ParentId;
                    entity.FullName = item.FullName;
                    entity.EnCode = item.EnCode;
                    entity.Nature = item.Nature;
                    entity.IsOrg = item.IsOrg;
                    entity.SortCode = item.SortCode;
                    entity.TeamType = item.TeamType;
                }
            }
            db.dbcontext.SaveChanges();
        }

        public DepartmentEntity Delete(string id)
        {
            var db = DbFactory.Base() as Database;
            var dbset = db.dbcontext.Set<DepartmentEntity>();

            var entity = dbset.Find(id);
            db.dbcontext.Entry(entity).State = System.Data.Entity.EntityState.Deleted;

            db.dbcontext.SaveChanges();
            return entity;
        }

        /// <summary>
        /// 获取所有的部门信息
        /// </summary>
        /// <returns></returns>
        public List<DepartmentEntity> GetAll()
        {
            return this.BaseRepository().IQueryable().ToList();
        }

        public DepartmentEntity GetCompany(string deptId)
        {
            var db = DbFactory.Base() as Database;
            var dbset = db.dbcontext.Set<DepartmentEntity>();

            var query = from q in dbset
                        where q.DepartmentId == deptId
                        select q;

            var dept = query.FirstOrDefault();
            while (dept != null && dept.Nature != "厂级")
            {
                query = from q1 in dbset
                        join q2 in query on q1.DepartmentId equals q2.ParentId
                        select q1;
                dept = query.FirstOrDefault();
            }

            if (dept == null) dept = (from q in db.IQueryable<DepartmentEntity>()
                                      where q.ParentId == "0"
                                      select q).FirstOrDefault();

            return dept;
        }
    }
}
