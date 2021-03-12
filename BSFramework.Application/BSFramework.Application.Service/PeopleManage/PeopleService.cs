using BSFramework.Entity.WorkMeeting;
using BSFramework.Application.IService.PeopleManage;
using BSFramework.Data.Repository;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Linq;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.BaseManage;
using System.Text;
using System.Data;
using BSFramework.Data;
using BSFramework.Application.Service.ExperienceManage;
using BSFramework.Application.IService.WebApp;
using BSFramework.Application.Service.WebApp;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Application.Entity.WebApp;
using System;
using BSFramework.Application.Entity.Activity;
using BSFramework.Data.EF;
using BSFramework.Util.Extension;

namespace BSFramework.Application.Service.PeopleManage
{
    /// <summary>
    /// 成员管理
    /// </summary>
    public class PeopleService : RepositoryFactory<PeopleEntity>, IPeopleService
    {
        private System.Data.Entity.DbContext _context;


        public PeopleService()
        {
            _context = (DbFactory.Base() as Database).dbcontext;
        }

        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public IEnumerable<PeopleEntity> GetList(string deptid, int page, int pagesize, out int total)
        {
            //var user = OperatorProvider.Provider.Current();
            var query = this.BaseRepository().IQueryable().Where(x => x.FingerMark == "yes");
            if (deptid != null) query = query.Where(x => x.BZID == deptid);
            total = query.Count();
            var list = query.ToList();

            //string[] property = new string[] { "Planer", "Name" };
            //bool[] sort = new bool[] { true, true };

            //list = new IListSort<PeopleEntity>(list, property, sort).Sort().ToList();

            //list[0].Planer = "01,04,06";
            //list[1].Planer = "01,02,03";
            //list[2].Planer = "01,02,03";
            //list[3].Planer = "01";

            //var list1 = new List<PeopleEntity>();
            //list1.Add(list[0]);
            //list1.Add(list[1]);
            //list1.Add(list[2]);
            //list1.Add(list[3]);
            list = list.OrderBy(x => x.Planer).ThenBy(x => x.Name).ToList();

            return list.Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }

        public IEnumerable<LoginInfo> GetLogins()
        {
            var query = new Repository<LoginInfo>(DbFactory.Base()).IQueryable();
            return query.ToList();
        }
        public void dellogin(LoginInfo l)
        {
            new Repository<LoginInfo>(DbFactory.Base()).Delete(l);

        }

        public IEnumerable<PeopleEntity> GetListByDept(string deptid)
        {
            var query = from q1 in _context.Set<PeopleEntity>()
                        join q2 in _context.Set<UserEntity>() on q1.ID equals q2.UserId
                        where q2.DepartmentId == deptid
                        select new { q1, q2 };

            var data = query.ToList();
            foreach (var item in data)
            {
                if (item.q2 != null) item.q1.IsEpiboly = item.q2.IsEpiboly;
            }
            return data.Select(x => x.q1).ToList();

            //try
            //{
            //    var query = from q in this.BaseRepository().IQueryable()
            //                where q.BZID == deptid && q.FingerMark == "yes"
            //                orderby q.Planer, q.Name
            //                select q;

            //    var people = query.ToList();
            //    var userIds = people.Select(x => x.ID).ToList();
            //    var userList = new RepositoryFactory().BaseRepository().IQueryable<UserEntity>(x => userIds.Contains(x.UserId)).ToList();
            //    people.ForEach(p =>
            //    {
            //        var user = userList.FirstOrDefault(x => x.UserId == p.ID);
            //        if (user != null) p.IsEpiboly = user.IsEpiboly;
            //        //p.IsEpiboly = userList.FirstOrDefault(x => x.UserId == p.ID).IsEpiboly;
            //    });
            //    return people;

            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}

            //var query = this.BaseRepository().IQueryable().Where(x => x.BZID == deptid && x.FingerMark == "yes");
            //total = query.Count();
            //var list = query.ToList();
            //list[0].Planer = "01,04,06";
            //list[1].Planer = "01,02,03";
            //list[2].Planer = "01,03,04";
            //list[3].Planer = "01";
            // return list.OrderBy(x => x.Planer).ThenBy(x => x.Name).ToList();
            //return query.OrderBy(x => x.Planer).ThenBy(x => x.Name).ToList();

        }
        public DataTable GetPeopleJson(Pagination pagination)
        {

            DatabaseType dataType = DbHelper.DbType;
            DataTable dt = this.BaseRepository().FindTableByProcPager(pagination, dataType);
            return dt;
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public PeopleEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }

        public LoginInfo GetLoginInfo(string keyValue)
        {
            return new Repository<LoginInfo>(DbFactory.Base()).FindEntity(keyValue);

        }
        public PeopleDutyEntity GetPeopleDuty(string keyValue)
        {
            return new Repository<PeopleDutyEntity>(DbFactory.Base()).FindEntity(keyValue);

        }
        public PeopleDutyEntity GetPeopleDuty(string peopleid, string year, string typeid)
        {
            var query = new Repository<PeopleDutyEntity>(DbFactory.Base()).IQueryable();
            query = query.Where(x => x.PeopleId == peopleid && x.Year.Contains(year) && x.TypeId == typeid);
            return query.ToList().FirstOrDefault();
        }

        public IEnumerable<PeopleDutyEntity> GetPeopleDutyList(string bzid)
        {
            var query = new Repository<PeopleDutyEntity>(DbFactory.Base()).IQueryable();
            query = query.Where(x => x.BZId == bzid);
            return query.ToList();
        }
        public void RemovePeopleDuty(string keyValue)
        {
            new Repository<PeopleDutyEntity>(DbFactory.Base()).Delete(keyValue);
        }
        public void SavePeopleDuty(string keyValue, PeopleDutyEntity entity)
        {
            var entity1 = this.GetPeopleDuty(keyValue);
            if (string.IsNullOrEmpty(keyValue))
            {
                new Repository<PeopleDutyEntity>(DbFactory.Base()).Insert(entity);

                // new Repository<FileInfoEntity>(DbFactory.Base()).Insert(entity.files.ToList());

            }
            else
            {

                entity1.DutyMan = entity.DutyMan;
                entity1.Name = entity.Name;
                entity1.ParentDutyMan = entity.ParentDutyMan;
                entity1.SignDate = entity.SignDate;
                entity1.State = entity.State;
                entity1.TypeName = entity.TypeName;
                entity1.TypeId = entity.TypeId;
                //if (entity.files.Count > 0)
                //{
                //    new Repository<FileInfoEntity>(DbFactory.Base()).Insert(entity.files.ToList());
                //}
                entity1.Files = null;

                new Repository<PeopleDutyEntity>(DbFactory.Base()).Update(entity1);
            }
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }

        public DataTable insertBZZ()
        {
            var strSql = new StringBuilder();
            strSql.Append(@"SELECT  userid,account, gender , mobile , dutyname ,degrees ,IDENTIFYID ,realname,DEPARTMENTID,DEPARTMENTCODE
                            from base_user where dutyname='班组长'");
            DataTable dt = this.BaseRepository().FindTable(strSql.ToString());
            return dt;
        }
        public int CheckTel(string tel)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT  count(*) from wg_people where linkway='" + tel + "'");
            DataTable dt = this.BaseRepository().FindTable(strSql.ToString());
            return int.Parse(dt.Rows[0][0].ToString());
        }
        public DataTable GetTel(string tel)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT id,name,BZID as deptid   from wg_people where linkway='" + tel + "'");
            DataTable dt = this.BaseRepository().FindTable(strSql.ToString());
            return dt;
        }
        public int CheckTel1(string tel, string id)
        {
            //var strSql = new StringBuilder();
            //tel = tel.Trim();
            //strSql.Append("SELECT  count(*) from wg_people where linkway='" + tel + "' and id !='" + id + "'");
            //DataTable dt = this.BaseRepository().FindTable(strSql.ToString());
            //return int.Parse(dt.Rows[0][0].ToString());

            var peopleSet = _context.Set<PeopleEntity>();
            var linq = from q in peopleSet
                       where q.LinkWay == tel && q.ID != id
                       select q;

            return linq.Count();
        }

        public int CheckNo(string labour)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT  count(*) from wg_people where LabourNo='" + labour + "'");
            DataTable dt = this.BaseRepository().FindTable(strSql.ToString());
            return int.Parse(dt.Rows[0][0].ToString());
        }
        public int CheckNo(string labour, string id)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT  count(*) from wg_people where LabourNo='" + labour + "' and id !='" + id + "'");
            DataTable dt = this.BaseRepository().FindTable(strSql.ToString());
            return int.Parse(dt.Rows[0][0].ToString());
        }
        public int CheckIdentity(string identity, string id)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT  count(*) from wg_people where IdentityNo='" + identity + "' and id !='" + id + "'");
            DataTable dt = this.BaseRepository().FindTable(strSql.ToString());
            return int.Parse(dt.Rows[0][0].ToString());
        }
        public int CheckIdentity(string identity)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT  count(*) from wg_people where IdentityNo='" + identity + "'");
            DataTable dt = this.BaseRepository().FindTable(strSql.ToString());
            return int.Parse(dt.Rows[0][0].ToString());
        }
        /// <summary>
        /// 工作经历添加引用
        /// </summary>
        private UserExperiencService userex = new UserExperiencService();
        private DepartmentService deptservice = new DepartmentService();

        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, PeopleEntity entity)
        {
            var entity1 = this.GetEntity(keyValue);
            string oldRoleDutyId = string.Empty;
            if (entity1 == null)
            {
                entity.FingerMark = "yes";
                this.BaseRepository().Insert(entity);

                new Repository<FileInfoEntity>(DbFactory.Base()).Insert(entity.Files.ToList());

            }
            else
            {
                oldRoleDutyId = entity1.RoleDutyId;
                entity1.Name = entity.Name;
                entity1.Sex = entity.Sex;
                entity1.IdentityNo = entity.IdentityNo;
                entity1.Folk = entity.Folk;
                entity1.Age = entity.Age;
                entity1.Visage = entity.Visage;

                string qs = "";
                if (!string.IsNullOrEmpty(entity.Quarters))
                {
                    var quarters = entity.Quarters.Split(',');
                    foreach (string q in quarters)
                    {
                        qs += q.Trim() + ",";
                    }
                    if (qs.EndsWith(",")) qs = qs.Substring(0, qs.Length - 1);
                    entity1.Quarters = qs;
                }
                entity1.Degree = entity.Degree;
                entity1.LinkWay = entity.LinkWay;
                entity1.Address = entity.Address;
                entity1.LabourNo = entity.LabourNo;
                entity1.BZName = entity.BZName;
                entity1.BZCode = entity.BZCode;
                entity1.BZID = entity.BZID;
                entity1.TecLevel = entity.TecLevel;
                entity1.EntryDate = entity.EntryDate;
                entity1.HealthStatus = entity.HealthStatus;
                entity1.FingerMark = entity.FingerMark;
                entity1.EleIdiograph = entity.EleIdiograph;
                entity1.Planer = entity.Planer;
                entity1.JobName = entity.JobName;
                entity1.Remark = entity.Remark;
                entity1.Photo = entity.Photo;
                entity1.Birthday = entity.Birthday;
                entity1.Native = entity.Native;
                entity1.OldDegree = entity.OldDegree;
                entity1.NewDegree = entity.NewDegree;
                entity1.WorkKind = entity.WorkKind;
                entity1.WorkAge = entity.WorkAge;
                entity1.CurrentWorkAge = entity.CurrentWorkAge;
                entity1.WorkType = entity.WorkType;
                entity1.QuarterId = entity.QuarterId;
                entity1.RoleDutyId = entity.RoleDutyId;
                entity1.RoleDutyName = entity.RoleDutyName;
                entity1.IsSpecial = entity.IsSpecial;
                entity1.IsSpecialEquipment = entity.IsSpecialEquipment;
                entity1.SpecialtyType = entity.SpecialtyType;
                entity1.IsEpiboly = entity.IsEpiboly;
                entity1.IsFourPerson = entity.IsFourPerson;
                entity1.Signature = entity.Signature == null ? entity1.Signature : entity.Signature;
                if (entity.Files != null)
                {
                    if (entity.Files.Count > 0)
                    {
                        new Repository<FileInfoEntity>(DbFactory.Base()).Insert(entity.Files.ToList());
                    }
                }
                entity1.Files = null;

                this.BaseRepository().Update(entity1);
            }
            #region 转岗 新增变更工作记录
            UserExperiencEntity userEx = new UserExperiencEntity();
            if (entity1 != null)
            {
                if (oldRoleDutyId != entity.RoleDutyId)
                {
                    userEx.ExperiencId = Guid.NewGuid().ToString();
                    userEx.StartTime = DateTime.Now.ToString("yyyy-MM-dd");
                    userEx.EndTime = "至今";
                    userEx.Isend = true;
                    userEx.isupdate = false;
                    userEx.createuserid = entity.ID;
                    userEx.createtime = DateTime.Now;
                    userEx.createuser = entity.Name;
                    var userDept = deptservice.GetEntity(entity1.DeptId);
                    //获取部门和班组
                    if (userDept.Nature == "班组")
                    {
                        var userParent = deptservice.GetEntity(userDept.ParentId);
                        var company = deptservice.GetEntity(userParent.ParentId);
                        userEx.Commpany = company.FullName;
                        userEx.Department = userParent.FullName + "/" + userDept.FullName;
                    }
                    else
                    {
                        var company = deptservice.GetEntity(userDept.ParentId);
                        userEx.Commpany = company.FullName;
                        userEx.Department = userDept.FullName;
                    }
                    userEx.Jobs = entity.Quarters;
                    userEx.Position = entity.RoleDutyName;
                    userex.add(userEx);
                }
            }


            #endregion
        }


        public void SaveLoginInfo(string keyValue, LoginInfo entity)
        {
            var entity1 = this.GetLoginInfo(keyValue);
            if (entity1 == null)
            {
                new Repository<LoginInfo>(DbFactory.Base()).Insert(entity);

            }
            else
            {
                entity1.Face = entity.Face;
                entity1.Finger = entity.Finger;
                new Repository<LoginInfo>(DbFactory.Base()).Update(entity);
            }
        }
        #endregion
        public void Update(PeopleEntity entity)
        {
            this.BaseRepository().Update(entity);
        }
        public void UpdatePeopleDuty(PeopleDutyEntity entity)
        {
            new Repository<PeopleDutyEntity>(DbFactory.Base()).Update(entity);
        }
        public void DelDuty(PeopleDutyEntity entity)
        {
            new Repository<PeopleDutyEntity>(DbFactory.Base()).Delete(entity);
        }
        public DutyEntity GetDutyEntity(string keyValue)
        {
            return new Repository<DutyEntity>(DbFactory.Base()).FindEntity(keyValue);
        }
        public DutyEntity GetDutyEntityByRole(string roleId)
        {
            var query = new Repository<DutyEntity>(DbFactory.Base()).IQueryable();
            query = query.Where(x => x.RoleId == roleId);
            return query.ToList().FirstOrDefault();
        }
        public DutyDangerEntity GetDutyDangerEntityByRole(string roleId)
        {
            var query = new Repository<DutyDangerEntity>(DbFactory.Base()).IQueryable();
            query = query.Where(x => x.RoleId == roleId);
            return query.ToList().FirstOrDefault();
        }
        public DutyDangerEntity GetDutyDangerEntity(string keyValue)
        {
            return new Repository<DutyDangerEntity>(DbFactory.Base()).FindEntity(keyValue);
        }

        public void SaveDuty(string id, DutyEntity entity)
        {
            var entity1 = this.GetDutyEntity(id);
            if (entity1 == null)
            {
                new Repository<DutyEntity>(DbFactory.Base()).Insert(entity);
            }
            else
            {
                entity1.DutyContent = entity.DutyContent;
                entity1.ReviseDate = entity.ReviseDate;
                entity1.ReviseUserId = entity.ReviseUserId;
                entity1.ReviseUserName = entity.ReviseUserName;
                new Repository<DutyEntity>(DbFactory.Base()).Update(entity1);
            }
        }

        public void SaveDutyDanger(string id, DutyDangerEntity entity)
        {
            var entity1 = this.GetDutyDangerEntity(id);
            if (entity1 == null)
            {
                new Repository<DutyDangerEntity>(DbFactory.Base()).Insert(entity);
            }
            else
            {
                entity1.Danger = entity.Danger;
                entity1.Measure = entity.Measure;
                entity1.ReviseDate = entity.ReviseDate;
                entity1.ReviseUserId = entity.ReviseUserId;
                entity1.ReviseUserName = entity.ReviseUserName;
                entity1.DangerReviseDate = entity.DangerReviseDate;
                entity1.DangerReviseUserId = entity.DangerReviseUserId;
                entity1.DangerReviseUserName = entity.DangerReviseUserName;
                entity1.DutyContent = entity.DutyContent;
                new Repository<DutyDangerEntity>(DbFactory.Base()).Update(entity1);
            }
        }
        public PeopleDutyTypeEntity GetDutyTypeEntity(string keyValue)
        {
            return new Repository<PeopleDutyTypeEntity>(DbFactory.Base()).FindEntity(keyValue);
        }
        public List<PeopleDutyTypeEntity> GetDutyTypes()
        {
            var query = new Repository<PeopleDutyTypeEntity>(DbFactory.Base()).IQueryable();
            return query.ToList();
        }
        public void DelDutyType(string keyValue)
        {
            new Repository<PeopleDutyTypeEntity>(DbFactory.Base()).Delete(keyValue);
        }
        public void SaveDutyType(PeopleDutyTypeEntity entity)
        {
            var entity1 = this.GetDutyTypeEntity(entity.ID);
            if (entity1 == null)
            {
                new Repository<PeopleDutyTypeEntity>(DbFactory.Base()).Insert(entity);
            }
            else
            {
                new Repository<PeopleDutyTypeEntity>(DbFactory.Base()).Update(entity);
            }
        }
        /// <summary>
        /// 根据部门编码获取用户信息，包含ActivityPersonId
        /// </summary>
        /// <param name="userDeptCode">部门编码</param>
        /// <returns></returns>
        public System.Collections.IList GetList(string userDeptCode)
        {
            var db = new RepositoryFactory().BaseRepository();
            var url = BSFramework.Util.Config.GetValue("AppUrl");
            var query = from q2 in db.IQueryable<UserEntity>()
                        where q2.DepartmentCode.StartsWith(userDeptCode)
                        select new
                        {
                            q2.UserId,
                            q2.DepartmentCode,
                            q2.DepartmentId,
                            q2.RealName,
                            q2.Mobile,
                            q2.Gender,
                            HeadIcon = !string.IsNullOrEmpty(q2.HeadIcon) ? q2.HeadIcon.Replace("~/", url) : null,
                            q2.IsEpiboly,
                            q2.IDENTIFYID,
                        };
            var data = query.ToList();
            return data;
        }
    }
}
