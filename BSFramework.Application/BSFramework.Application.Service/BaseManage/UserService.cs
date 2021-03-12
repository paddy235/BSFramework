using BSFramework.Data;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using BSFramework.Util.Extension;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Linq;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.IService.BaseManage;
using BSFramework.Application.IService.AuthorizeManage;
using BSFramework.Application.Service.AuthorizeManage;
using System;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Service.PeopleManage;
using BSFramework.Application.Entity.ToolManage;
using Bst.Bzzd.Sync;
using BSFramework.Application.Entity.CertificateManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Data.EF;

namespace BSFramework.Application.Service.BaseManage
{
    /// 描 述：用户管理
    /// </summary>
    public class UserService : RepositoryFactory<UserEntity>, IUserService
    {
        //private IAuthorizeService<UserEntity> iauthorizeservice = new AuthorizeService<UserEntity>();
        private System.Data.Entity.DbContext _context;

        public UserService()
        {
            _context = (DbFactory.Base() as Database).dbcontext;
        }

        #region 获取数据
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <returns></returns>
        public List<UserEntity> GetTable()
        {
            var db = new RepositoryFactory().BaseRepository();

            var strSql = new StringBuilder();
            strSql.Append(@"SELECT  u.*,
                                    d.FullName AS DepartmentName 
                            FROM    Base_User u
                                    LEFT JOIN Base_Department d ON d.DepartmentId = u.DepartmentId
                            WHERE   1=1");
            strSql.Append(" AND u.UserId <> 'System' AND u.EnabledMark = 1 AND u.DeleteMark=0");

            var query = from q1 in db.IQueryable<UserEntity>()
                        join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.DepartmentId
                        where q1.UserId != "System" && q1.EnabledMark == 1 && q1.DeleteMark == 0
                        select new { q1, q2.FullName };

            var data = query.ToList().Select(x =>
            {
                var user = x.q1;
                user.DepartmentName = x.FullName;
                return user;
            }).ToList();
            return data;
        }

        public UserEntity GetUserByAccount(string account)
        {
            return _context.Set<UserEntity>().FirstOrDefault(x => x.Account == account);

            //var strSql = new StringBuilder();
            //strSql.Append(@"SELECT  userid
            //                FROM    Base_User u
            //                WHERE   account='" + account + "'");
            //return this.BaseRepository().FindTable(strSql.ToString());

            //return this.BaseRepository().IQueryable().Where(x => x.Account == account).FirstOrDefault();
        }
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserEntity> GetList()
        {
            var expression = LinqExtensions.True<UserEntity>();
            expression = expression.And(t => t.EnabledMark == 1).And(t => t.DeleteMark == 0);
            return this.BaseRepository().IQueryable(expression).OrderByDescending(t => t.CreateDate).ToList();
        }
        public IQueryable<UserEntity> GetUserList()
        {
            var expression = LinqExtensions.True<UserEntity>();
            expression = expression.And(t => t.EnabledMark == 1).And(t => t.DeleteMark == 0);
            return this.BaseRepository().IQueryable(expression).OrderByDescending(t => t.CreateDate);
        }



        ///// <summary>
        ///// 用户列表
        ///// </summary>
        ///// <param name="pagination">分页</param>
        ///// <param name="queryJson">查询参数</param>
        ///// <returns></returns>
        //public DataTable GetPageList(Pagination pagination, string queryJson)
        //{

        //    DatabaseType dataType = DbHelper.DbType;

        //    var queryParam = queryJson.ToJObject();
        //    //公司主键
        //    if (!queryParam["organizeId"].IsEmpty())
        //    {
        //        string organizeId = queryParam["organizeId"].ToString();
        //        pagination.conditionJson += string.Format(" and ORGANIZEID = '{0}'", organizeId);
        //    }
        //    //部门主键
        //    if (!queryParam["departmentId"].IsEmpty())
        //    {
        //        string departmentId = queryParam["departmentId"].ToString();
        //        pagination.conditionJson += string.Format(" and DEPARTMENTID = '{0}'", departmentId);
        //    }
        //    //查询条件
        //    if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
        //    {
        //        string condition = queryParam["condition"].ToString();
        //        string keyord = queryParam["keyword"].ToString();
        //        switch (condition)
        //        {
        //            case "Account":            //账户
        //                pagination.conditionJson += string.Format(" and ACCOUNT  like '%{0}%'", keyord);
        //                break;
        //            case "RealName":          //姓名
        //                pagination.conditionJson += string.Format(" and REALNAME  like '%{0}%'", keyord);
        //                break;
        //            case "Mobile":          //手机
        //                pagination.conditionJson += string.Format(" and MOBILE like '%{0}%'", keyord);
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //    if (!queryParam["code"].IsEmpty() && !queryParam["isOrg"].IsEmpty())
        //    {
        //        if (queryParam["isOrg"].ToString() == "Organize")
        //            pagination.conditionJson += string.Format(" and organizeid  in (select organizeid from base_organize where encode like '{0}%')", queryParam["code"].ToString());
        //        else
        //            pagination.conditionJson += string.Format(" and departmentid  in (select departmentid from base_department where encode like '{0}%')", queryParam["code"].ToString());
        //    }

        //    DataTable dt = this.BaseRepository().FindTableByProcPager(pagination, dataType);

        //    return dt;

        //}


        /// <summary>
        /// 用户列表（ALL）
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllTable()
        {
            var strSql = new StringBuilder();
            strSql.Append(@"SELECT  u.UserId ,
                                    u.EnCode ,
                                    u.Account ,
                                    u.RealName ,
                                    u.Gender ,
                                    u.Birthday ,
                                    u.Mobile ,
                                    u.Manager ,
                                    u.OrganizeId,
                                    u.DepartmentId,
                                    o.FullName AS OrganizeName ,
                                    d.FullName AS DepartmentName ,
                                    u.RoleId ,
                                    u.DutyName ,
                                    u.PostName ,
                                    u.EnabledMark ,
                                    u.CreateDate,
                                    u.Description
                            FROM    Base_User u
                                    LEFT JOIN Base_Organize o ON o.OrganizeId = u.OrganizeId
                                    LEFT JOIN Base_Department d ON d.DepartmentId = u.DepartmentId
                            WHERE   1=1");
            strSql.Append(" AND u.UserId <> 'System' AND u.EnabledMark = 1 AND u.DeleteMark=0 order by o.FullName,d.FullName,u.RealName");
            return this.BaseRepository().FindTable(strSql.ToString());
        }
        /// <summary>
        /// 用户列表（导出Excel）
        /// </summary>
        /// <returns></returns>
        public DataTable GetExportList(string condition, string keyword, string code, string isOrg)
        {

            //if (queryParam["isOrg"].ToString() == "Organize")
            //    pagination.conditionJson += string.Format(" and organizeid  in (select organizeid from base_organize where encode like '{0}%')", queryParam["code"].ToString());
            //else
            //    pagination.conditionJson += string.Format(" and departmentid  in (select departmentid from base_department where encode like '{0}%')", queryParam["code"].ToString());
            var strSql = new StringBuilder();
            //strSql.Append(@"SELECT [Account]
            //              ,[RealName]
            //              ,CASE WHEN Gender=1 THEN '男' ELSE '女' END AS Gender
            //              ,[Birthday]
            //              ,[Mobile]
            //              ,[Telephone]
            //              ,u.[Email]
            //              ,[WeChat]
            //              ,[MSN]
            //              ,u.[Manager]
            //              ,o.FullName AS Organize
            //              ,d.FullName AS Department
            //              ,u.[Description]
            //              ,u.[CreateDate]
            //              ,u.[CreateUserName]
            //          FROM Base_User u
            //          INNER JOIN Base_Department d ON u.DepartmentId=d.DepartmentId
            //          INNER JOIN Base_Organize o ON u.OrganizeId=o.OrganizeId");
            strSql.Append(@"SELECT u.account
                                  ,u.realname
                                  ,CASE WHEN u.gender=1 THEN '男' ELSE '女' END AS gender
                                  ,u.birthday
                                  ,u.mobile
                                  ,u.telephone
                                  ,u.email
                                  ,u.wechat
                                  ,u.msn
                                  ,u.manager
                                  ,o.FullName AS Organize
                                  ,d.FullName AS Department
                                  ,u.description
                                  ,u.createdate
                                  ,u.createUserName
                              FROM Base_User u
                              INNER JOIN Base_Department d ON u.DepartmentId=d.DepartmentId
                              INNER JOIN Base_Organize o ON u.OrganizeId=o.OrganizeId
                                where 1=1");
            switch (condition)
            {
                case "Account":            //账户
                    strSql.Append(string.Format(" and u.ACCOUNT  like '%{0}%'", keyword));
                    break;
                case "RealName":          //姓名
                    strSql.Append(string.Format(" and u.REALNAME  like '%{0}%'", keyword));
                    break;
                case "Mobile":          //手机
                    strSql.Append(string.Format(" and u.MOBILE like '%{0}%'", keyword));
                    break;
                default:
                    break;
            }
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(isOrg))
            {
                if (isOrg == "Organize")
                    strSql.Append(string.Format(" and u.organizeid  in (select organizeid from base_organize where encode like '{0}%')", code));
                else
                    strSql.Append(string.Format(" and u.departmentid  in (select departmentid from base_department where encode like '{0}%')", code));
            }
            return this.BaseRepository().FindTable(strSql.ToString());
        }
        /// <summary>
        /// 用户实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public UserEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns></returns>
        public UserEntity CheckLogin(string username)
        {
            var expression = LinqExtensions.True<UserEntity>();
            expression = expression.And(t => t.Account == username);
            expression = expression.Or(t => t.Mobile == username);
            expression = expression.Or(t => t.Email == username);
            return this.BaseRepository().FindEntity(expression);
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 账户不能重复
        /// </summary>
        /// <param name="account">账户值</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistAccount(string account, string keyValue)
        {
            var expression = LinqExtensions.True<UserEntity>();
            expression = expression.And(t => t.Account == account);
            if (!string.IsNullOrEmpty(keyValue))
            {
                expression = expression.And(t => t.UserId != keyValue);
            }
            return this.BaseRepository().IQueryable(expression).Count() == 0 ? true : false;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {

            PeopleEntity p = new PeopleService().GetEntity(keyValue);
            if (p != null)
            {
                new PeopleService().RemoveForm(keyValue);
            }
            this.BaseRepository().Delete(keyValue);
        }
        public void removemore(string deptid)
        {
            var list = this.GetList().Where(x => x.DepartmentId == deptid).Select(x => x.UserId);
            var ps = new PeopleService();
            foreach (string id in list)
            {
                ps.RemoveForm(id);
                this.BaseRepository().Delete(id);
            }
        }
        /// <summary>
        /// user表主键与people关联，直接插入数据
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="userEntity"></param>
        public string InsertUser(UserEntity userEntity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                #region 基本信息

                userEntity.CreateDate = DateTime.Now;
                // userEntity.CreateUserId = OperatorProvider.Provider.Current().UserId;
                // userEntity.CreateUserName = OperatorProvider.Provider.Current().UserName;
                userEntity.DeleteMark = 0;
                userEntity.EnabledMark = 1;

                userEntity.Secretkey = Md5Helper.MD5(CommonHelper.CreateNo(), 16).ToLower();
                userEntity.Password = Md5Helper.MD5(DESEncrypt.Encrypt(Md5Helper.MD5(userEntity.Password, 32).ToLower(), userEntity.Secretkey).ToLower(), 32).ToLower();
                db.Insert(userEntity);

                //userEntity.Secretkey = Md5Helper.MD5(CommonHelper.CreateNo(), 16).ToLower();

                //userEntity.Password = Md5Helper.MD5(DESEncrypt.Encrypt(Password, userEntity.Secretkey).ToLower(), 32).ToLower();

                #endregion

                #region 默认添加 角色、岗位、职位
                db.Delete<UserRelationEntity>(t => t.IsDefault == 1 && t.UserId == userEntity.UserId);
                List<UserRelationEntity> userRelationEntitys = new List<UserRelationEntity>();
                //角色
                if (!string.IsNullOrEmpty(userEntity.RoleId))
                {
                    userRelationEntitys.Add(new UserRelationEntity
                    {
                        Category = 2,
                        UserRelationId = Guid.NewGuid().ToString(),
                        UserId = userEntity.UserId,
                        ObjectId = userEntity.RoleId,
                        CreateDate = DateTime.Now,
                        //  CreateUserId = OperatorProvider.Provider.Current().UserId,
                        // CreateUserName = OperatorProvider.Provider.Current().UserName,
                        IsDefault = 1,
                    });
                }
                //岗位
                if (!string.IsNullOrEmpty(userEntity.DutyId))
                {
                    userRelationEntitys.Add(new UserRelationEntity
                    {
                        Category = 3,
                        UserRelationId = Guid.NewGuid().ToString(),
                        UserId = userEntity.UserId,
                        ObjectId = userEntity.DutyId,
                        CreateDate = DateTime.Now,
                        //  CreateUserId = OperatorProvider.Provider.Current().UserId,
                        // CreateUserName = OperatorProvider.Provider.Current().UserName,
                        IsDefault = 1,
                    });
                }
                //职位
                if (!string.IsNullOrEmpty(userEntity.PostId))
                {
                    userRelationEntitys.Add(new UserRelationEntity
                    {
                        Category = 4,
                        UserRelationId = Guid.NewGuid().ToString(),
                        UserId = userEntity.UserId,
                        ObjectId = userEntity.PostId,
                        CreateDate = DateTime.Now,
                        //  CreateUserId = OperatorProvider.Provider.Current().UserId,
                        // CreateUserName = OperatorProvider.Provider.Current().UserName,
                        IsDefault = 1,
                    });
                }
                db.Insert(userRelationEntitys);
                #endregion

                db.Commit();

                //this.SyncTraing(userEntity.UserId);

                return string.Empty;
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 保存用户表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="userEntity">用户实体</param>
        /// <returns></returns>
        public string SaveForm(string keyValue, UserEntity userEntity)
        {
            var syncall = false;
            if (userEntity.RealName == "syncall")
            {
                syncall = true;
            }
            if (userEntity.Birthday == null) userEntity.Birthday = new DateTime(1990, 01, 01);
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            //var current = OperatorProvider.Provider.Current();
            //string userid = "";
            //string username = "";
            //if (current != null)
            //{
            //    userid = current.UserId;
            //    username = current.UserName;
            //}
            try
            {
                #region 基本信息
                if (!string.IsNullOrEmpty(keyValue))
                {
                    UserEntity user = BaseRepository().FindEntity(keyValue);
                    if (user == null)
                    {
                        if (userEntity.Password != null)
                        {
                            userEntity.Password = userEntity.Password.Replace("&nbsp;", "");
                        }
                        userEntity.Password = string.IsNullOrEmpty(userEntity.Password) ? "123456" : userEntity.Password;
                        userEntity.CreateDate = DateTime.Now;
                        //userEntity.CreateUserId = OperatorProvider.Provider.Current().UserId;
                        //userEntity.CreateUserName = OperatorProvider.Provider.Current().UserName;
                        userEntity.DeleteMark = 0;
                        userEntity.EnabledMark = 1;
                        if (userEntity.Secretkey == null)
                        {
                            userEntity.Secretkey = Md5Helper.MD5(CommonHelper.CreateNo(), 16).ToLower();
                        }
                        if (userEntity.Password != null)
                        {
                            userEntity.Password = Md5Helper.MD5(DESEncrypt.Encrypt(Md5Helper.MD5(userEntity.Password, 32).ToLower(), userEntity.Secretkey).ToLower(), 32).ToLower();
                        }
                        db.Insert(userEntity);
                    }
                    else
                    {
                        userEntity.Modify(keyValue);
                        userEntity.Password = null;
                        user.Account = userEntity.Account == null ? user.Account : userEntity.Account;
                        user.OrganizeCode = userEntity.OrganizeCode == null ? user.OrganizeCode : userEntity.OrganizeCode;
                        user.OrganizeId = userEntity.OrganizeId == null ? user.OrganizeId : userEntity.OrganizeId;
                        user.DepartmentCode = userEntity.DepartmentCode == null ? user.DepartmentCode : userEntity.DepartmentCode;
                        user.DepartmentId = userEntity.DepartmentId == null ? user.DepartmentId : userEntity.DepartmentId;
                        user.RoleId = userEntity.RoleId == null ? user.RoleId : userEntity.RoleId;
                        user.RoleName = userEntity.RoleName == null ? user.RoleName : userEntity.RoleName;
                        user.Mobile = userEntity.Mobile == null ? user.Mobile : userEntity.Mobile;
                        user.Telephone = userEntity.Telephone == null ? user.Telephone : userEntity.Telephone;
                        user.Gender = userEntity.Gender == null ? user.Gender : userEntity.Gender;
                        user.RealName = userEntity.RealName == null ? user.RealName : userEntity.RealName;
                        user.SpecialtyType = userEntity.SpecialtyType;
                        db.Update(user);
                    }
                }
                else
                {
                    userEntity.UserId = Guid.NewGuid().ToString();
                    userEntity.CreateDate = DateTime.Now;
                    //userEntity.CreateUserId = OperatorProvider.Provider.Current().UserId;
                    // userEntity.CreateUserName = OperatorProvider.Provider.Current().UserName;
                    userEntity.DeleteMark = 0;
                    userEntity.EnabledMark = 1;

                    if (userEntity.Secretkey == null)
                    {
                        userEntity.Secretkey = Md5Helper.MD5(CommonHelper.CreateNo(), 16).ToLower();
                    }
                    if (userEntity.Password == null)
                    {
                        userEntity.Password = Md5Helper.MD5(DESEncrypt.Encrypt(Md5Helper.MD5(userEntity.Password, 32).ToLower(), userEntity.Secretkey).ToLower(), 32).ToLower();
                    }

                    db.Insert(userEntity);

                }
                #endregion

                #region 默认添加 角色、岗位、职位
                db.Delete<UserRelationEntity>(t => t.IsDefault == 1 && t.UserId == userEntity.UserId);
                List<UserRelationEntity> userRelationEntitys = new List<UserRelationEntity>();

                var roles = new List<RoleEntity>();
                //角色
                if (!string.IsNullOrEmpty(userEntity.RoleId))
                {
                    var ids = userEntity.RoleId.Split(',');
                    foreach (var item in ids)
                    {
                        userRelationEntitys.Add(new UserRelationEntity
                        {
                            Category = 2,
                            UserRelationId = Guid.NewGuid().ToString(),
                            UserId = userEntity.UserId,
                            ObjectId = item,
                            CreateDate = DateTime.Now,
                            // CreateUserId = OperatorProvider.Provider.Current().UserId,
                            // CreateUserName = OperatorProvider.Provider.Current().UserName,
                            IsDefault = 1,
                        });

                        var role = db.FindEntity<RoleEntity>(item);
                        if (role != null)
                            roles.Add(role);
                    }
                }
                if (keyValue.ToLower() != "system")
                {
                    if (string.IsNullOrEmpty(keyValue))
                    {
                        if (roles.Count(x => x.FullName == "班组长" || x.FullName == "班组成员") > 0)
                        {
                            var dept = db.FindEntity<DepartmentEntity>(userEntity.DepartmentId);
                            var pdetp = db.FindEntity<DepartmentEntity>(dept.ParentId);
                            var people = new PeopleEntity() { ID = userEntity.UserId, Name = string.IsNullOrEmpty(userEntity.RealName) ? userEntity.Account : userEntity.RealName, BZID = userEntity.DepartmentId, LinkWay = userEntity.Mobile, EntryDate = DateTime.Now, BZName = dept.FullName, DeptId = pdetp.DepartmentId, DeptName = pdetp.FullName, Sex = userEntity.Gender == 1 ? "男" : "女", Birthday = userEntity.Birthday.Value, Files = null };
                            //if (roles.Count(x => x.FullName == "班组成员") > 0)
                            //{
                            //    people.Quarters = "其他成员";
                            //    people.Planer = getplaner(people.Quarters, userEntity.OrganizeId);
                            //}
                            //if (roles.Count(x => x.FullName == "班组长") > 0)
                            //{
                            //    people.Quarters = "班长";
                            //    people.Planer = getplaner(people.Quarters, userEntity.OrganizeId);
                            //}
                            people.FingerMark = "yes";
                            db.Insert(people);
                        }
                    }
                    else   //修改
                    {
                        if (roles.Count(x => x.FullName == "班组长" || x.FullName == "班组成员") > 0)
                        {
                            var dept = db.FindEntity<DepartmentEntity>(userEntity.DepartmentId);
                            var pdetp = db.FindEntity<DepartmentEntity>(dept.ParentId);

                            var people = new PeopleEntity() { ID = userEntity.UserId, Name = string.IsNullOrEmpty(userEntity.RealName) ? userEntity.Account : userEntity.RealName, BZID = userEntity.DepartmentId, LinkWay = userEntity.Mobile, EntryDate = DateTime.Now, BZName = dept.FullName, DeptId = pdetp.DepartmentId, DeptName = pdetp.FullName, Sex = userEntity.Gender == 1 ? "男" : "女", Birthday = userEntity.Birthday.Value, Files = null };
                            PeopleEntity p = new PeopleService().GetEntity(people.ID);

                            if (p != null)   //该人员已存在，
                            {

                                //if (roles.Count(x => x.FullName == "班组成员") > 0) //
                                //{
                                //    if (p.Quarters != "班长")
                                //    {
                                //        people.Quarters = p.Quarters;
                                //        people.Planer = getplaner(people.Quarters, userEntity.OrganizeId);
                                //    }
                                //    else
                                //    {
                                //        people.Quarters = "其他成员";
                                //        people.Planer = getplaner(people.Quarters, userEntity.OrganizeId);
                                //    }
                                //}
                                //if (roles.Count(x => x.FullName == "班组长") > 0)
                                //{
                                //    people.Quarters = "班长";
                                //    people.Planer = getplaner(people.Quarters, userEntity.OrganizeId);
                                //}
                                people.EntryDate = p.EntryDate;
                                people.FingerMark = "yes";
                                db.Update(people);
                            }
                            else   //第一次将非班组成员修改为班组成员，不存在  执行插入
                            {
                                //if (roles.Count(x => x.FullName == "班组成员") > 0)
                                //{
                                //    people.Quarters = "其他成员";
                                //    people.Planer = getplaner(people.Quarters, userEntity.OrganizeId);
                                //}
                                //if (roles.Count(x => x.FullName == "班组长") > 0)
                                //{
                                //    people.Quarters = "班长";
                                //    people.Planer = getplaner(people.Quarters, userEntity.OrganizeId);
                                //}
                                people.FingerMark = "yes";
                                db.Insert(people);
                            }
                        }
                        //else    //
                        //{
                        //    var dept = db.FindEntity<DepartmentEntity>(userEntity.DepartmentId);
                        //    //var pdetp = db.FindEntity<DepartmentEntity>(dept.ParentId);

                        //    var people = new PeopleEntity() { ID = userEntity.UserId, Name = string.IsNullOrEmpty(userEntity.RealName) ? userEntity.Account : userEntity.RealName, BZID = userEntity.DepartmentId, LinkWay = userEntity.Mobile, EntryDate = DateTime.Now, BZName = dept.FullName, DeptId = dept.DepartmentId, DeptName = dept.FullName, Sex = userEntity.Gender == 1 ? "男" : "女", Birthday = userEntity.Birthday.Value, Files = null };
                        //    PeopleEntity p = new PeopleService().GetEntity(people.ID);
                        //    if (p != null)
                        //    {
                        //        people.EntryDate = p.EntryDate;
                        //        people.FingerMark = "no";
                        //        db.Update(people);

                        //    }

                        //}
                    }
                }
                //岗位
                if (!string.IsNullOrEmpty(userEntity.DutyId))
                {
                    userRelationEntitys.Add(new UserRelationEntity
                    {
                        Category = 3,
                        UserRelationId = Guid.NewGuid().ToString(),
                        UserId = userEntity.UserId,
                        ObjectId = userEntity.DutyId,
                        CreateDate = DateTime.Now,
                        // CreateUserId = OperatorProvider.Provider.Current().UserId,
                        // CreateUserName = OperatorProvider.Provider.Current().UserName,
                        IsDefault = 1,
                    });
                }
                //职位
                if (!string.IsNullOrEmpty(userEntity.PostId))
                {
                    userRelationEntitys.Add(new UserRelationEntity
                    {
                        Category = 4,
                        UserRelationId = Guid.NewGuid().ToString(),
                        UserId = userEntity.UserId,
                        ObjectId = userEntity.PostId,
                        CreateDate = DateTime.Now,
                        // CreateUserId = OperatorProvider.Provider.Current().UserId,
                        // CreateUserName = OperatorProvider.Provider.Current().UserName,
                        IsDefault = 1,
                    });
                }
                db.Insert(userRelationEntitys);
                #endregion

                db.Commit();

                if (syncall)
                    this.SyncTraing(null);
                else
                    this.SyncTraing(userEntity.UserId);

                return userEntity.UserId;
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }
        }
        public static string getplaner(string quarter, string orgid)
        {
            string val = "";


            //var user = OperatorProvider.Provider.Current();    注意注意注意，业务代码里面禁止使用用户上下文代码（依赖登录）
            var data = new PostService().GetQuartersList(orgid);
            var role = data.Where(x => x.FullName == quarter).FirstOrDefault();
            if (role != null) val = role.EnCode;
            return val;
        }
        private void SyncTraing(string userid)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<UserEntity>()
                        select q;

            if (!string.IsNullOrEmpty(userid))
            {
                query = from q in query
                        where q.UserId == userid
                        select q;
            }

            var users = query.ToList();

            var sync = new TrainingSystem();
            sync.SyncUser(users);
        }

        /// <summary>
        /// 前台修改成员，更新user，只更新部分字段，不操作角色岗位等
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="userEntity"></param>
        /// <returns></returns>
        public string SaveFormNew(string keyValue, UserEntity userEntity)
        {
            var syncall = false;
            if (userEntity.RealName == "syncall")
            {
                syncall = true;
            }
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            //IRepository<UserEntity> db1 = this.BaseRepository().BeginTrans();
            //IRepository<UserEntity> db2 = new RepositoryFactory<UserEntity>().BaseRepository().BeginTrans();
            try
            {
                userEntity.Modify(keyValue);
                userEntity.Password = null;
                userEntity.Secretkey = null;
                UserEntity user = BaseRepository().FindEntity(keyValue);
                user.Account = userEntity.Account == null ? user.Account : userEntity.Account;
                user.OrganizeCode = userEntity.OrganizeCode == null ? user.OrganizeCode : userEntity.OrganizeCode;
                user.OrganizeId = userEntity.OrganizeId == null ? user.OrganizeId : userEntity.OrganizeId;
                user.DepartmentCode = userEntity.DepartmentCode == null ? user.DepartmentCode : userEntity.DepartmentCode;
                user.DepartmentId = userEntity.DepartmentId == null ? user.DepartmentId : userEntity.DepartmentId;
                user.RoleId = userEntity.RoleId == null ? user.RoleId : userEntity.RoleId;
                user.RoleName = userEntity.RoleName == null ? user.RoleName : userEntity.RoleName;
                user.Mobile = userEntity.Mobile == null ? user.Mobile : userEntity.Mobile;
                user.Telephone = userEntity.Telephone == null ? user.Telephone : userEntity.Telephone;
                user.Gender = userEntity.Gender == null ? user.Gender : userEntity.Gender;
                user.Birthday = userEntity.Birthday;
                user.IDENTIFYID = userEntity.IDENTIFYID;
                user.EnterTime = userEntity.EnterTime;
                db.Update(user);
                // db1.Update(userEntity);
                // db2.Update(userEntity);

                db.Commit();
                // db1.Commit();
                //db2.Commit();

                if (syncall)
                    this.SyncTraing(null);
                else
                    this.SyncTraing(userEntity.UserId);

                return userEntity.UserId;
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 修改用户登录密码
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="Password">新密码（MD5 小写）</param>
        public void RevisePassword(string keyValue, string Password)
        {
            UserEntity userEntity = new UserEntity();
            userEntity.UserId = keyValue;
            userEntity.Secretkey = Md5Helper.MD5(CommonHelper.CreateNo(), 16).ToLower();
            userEntity.Password = Md5Helper.MD5(DESEncrypt.Encrypt(Password, userEntity.Secretkey).ToLower(), 32).ToLower();
            this.BaseRepository().Update(userEntity);
        }
        /// <summary>
        /// 修改用户状态
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="State">状态：1-启动；0-禁用</param>
        public void UpdateState(string keyValue, int State)
        {
            UserEntity userEntity = new UserEntity();
            userEntity.Modify(keyValue);
            userEntity.EnabledMark = State;
            this.BaseRepository().Update(userEntity);
        }
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="userEntity">实体对象</param>
        public void UpdateEntity(UserEntity userEntity)
        {
            this.BaseRepository().Update(userEntity);
        }

        /// <summary>
        /// 根据用户ID和类别获取用户拥有的资源名称，多个用英文逗号分隔
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="category">数据类别，1:部门名称,2:角色名称,3:岗位名称,4:职位名称,5:工作组</param>
        /// <returns></returns>
        public string GetObjectName(string userId, int category)
        {
            //StringBuilder sb = new StringBuilder();
            //DbParameter[] param ={
            //                        DbParameters.CreateDbParameter("@userId",userId),
            //                        DbParameters.CreateDbParameter("@category",category)
            //                    };
            //DataTable dt = this.BaseRepository().FindTable("select fullname from base_role where roleid in( select objectid from BASE_USERRELATION  where userid=@userId and category=@category)", param.ToArray());
            //foreach (DataRow dr in dt.Rows)
            //{
            //    sb.Append(dr[0].ToString() + ",");
            //}
            //dt.Dispose();
            //return sb.ToString().TrimEnd(',');

            var db = DbFactory.Base();

            var query = from q1 in db.IQueryable<RoleEntity>()
                        join q2 in db.IQueryable<UserRelationEntity>() on q1.RoleId equals q2.ObjectId
                        where q2.UserId == userId && q2.Category == category
                        select q1;

            var data = query.ToList();

            return string.Join(",", data.Select(x => x.FullName));
        }

        public IEnumerable<UserEntity> GetDeptUser(string deptid)
        {
            var db = new RepositoryFactory().BaseRepository();

            var userquery = from q1 in db.IQueryable<UserEntity>()
                            join q2 in db.IQueryable<PeopleEntity>() on q1.UserId equals q2.ID into into1
                            from d1 in into1.DefaultIfEmpty()
                            where q1.DepartmentId == deptid
                            orderby new { d1.Planer, q1.RealName }
                            //select new { q1, Sort1 = d1 == null ? "99" : d1.Planer, Sort2 = q1.CreateDate };
                            select q1;

            //var users = userquery.OrderBy(x => new { x.Sort1, x.Sort2 }).Select(x => x.q1).ToList();
            //var users = userquery.OrderBy(x => x.q1.RealName).Select(x => x.q1).ToList();
            return userquery.ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<UserEntity> GetDeptUserBook(string deptid, string userId)
        {
            var db = new RepositoryFactory().BaseRepository();

            try
            {
                if (!string.IsNullOrEmpty(deptid))
                {
                    var userquery = from q1 in db.IQueryable<UserEntity>()
                                    join q2 in db.IQueryable<PeopleEntity>() on q1.UserId equals q2.ID
                                    where q1.DepartmentId == deptid && q2.FingerMark == "yes"
                                    select q1;

                    return userquery.ToList();
                }
                else
                {
                    var user = db.FindEntity<UserEntity>(userId);

                    var currentdept = (from q in db.IQueryable<DepartmentEntity>()
                                       where q.DepartmentId == user.DepartmentId
                                       select q).FirstOrDefault();

                    while (currentdept.Nature != "厂级")
                    {
                        currentdept = (from q1 in db.IQueryable<DepartmentEntity>()
                                       where q1.DepartmentId == currentdept.ParentId
                                       select q1).FirstOrDefault();
                    }

                    var factorydept = currentdept;

                    var userquery = from q1 in db.IQueryable<UserEntity>()
                                    join q2 in db.IQueryable<PeopleEntity>() on q1.UserId equals q2.ID
                                    where q1.DepartmentId == factorydept.DepartmentId && q2.FingerMark == "yes"
                                    select q1;

                    var data = userquery.ToList();
                    return data;
                }
            }
            catch (Exception ex)
            {
                return new List<UserEntity>();
            }
        }

        //        public List<UserInfoEntity> GetUserData(string code, string account, string name, string tel, int page, int pagesize, out int total)
        //        {
        //            var parameters = new List<DbParameter>();
        //            parameters.Add(DbParameters.CreateDbParameter("@skip", pagesize * (page - 1)));
        //            parameters.Add(DbParameters.CreateDbParameter("@take", pagesize * page));
        //            parameters.Add(DbParameters.CreateDbParameter("@code", code + "%"));
        //            parameters.Add(DbParameters.CreateDbParameter("@account", "%" + account + "%"));
        //            parameters.Add(DbParameters.CreateDbParameter("@name", "%" + name + "%"));
        //            parameters.Add(DbParameters.CreateDbParameter("@tel", "%" + tel + "%"));

        //            var querysql = @"
        //SELECT 
        //    a.userid,
        //    a.ACCOUNT as Account,
        //    a.REALNAME,
        //    a.GENDER,
        //    a.MOBILE,
        //    b.FULLNAME AS OrganizeName,
        //    c.FULLNAME AS DEPTNAME,
        //    d.roles as RoleName,
        //    a.ENABLEDMARK
        //FROM
        //    base_user a
        //        INNER JOIN
        //    (SELECT 
        //        a.USERID, GROUP_CONCAT(b.FULLNAME) AS roles
        //    FROM
        //        base_user a
        //    LEFT JOIN base_role b ON (FIND_IN_SET(b.ROLEID, a.ROLEID))
        //    GROUP BY a.userid) d ON d.userid = a.userid
        //        LEFT JOIN
        //    base_organize b ON b.ORGANIZEID = a.ORGANIZEID
        //        LEFT JOIN
        //    base_department c ON c.departmentid = a.DEPARTMENTID
        //WHERE
        //    a.ACCOUNT <> 'System'{0}
        //ORDER BY a.CREATEDATE
        //LIMIT @skip , @take
        //";
        //            var condition = new StringBuilder();
        //            if (!string.IsNullOrEmpty(code))
        //                condition.Append(" and c.encode like @code");
        //            if (!string.IsNullOrEmpty(account))
        //                condition.Append(" and a.ACCOUNT like @account");
        //            if (!string.IsNullOrEmpty(name))
        //                condition.Append(" and a.REALNAME like @name");
        //            if (!string.IsNullOrEmpty(tel))
        //                condition.Append(" and a.MOBILE like @tel");


        //            var countsql = @"
        //SELECT 
        //    count(1)
        //FROM
        //    base_user a
        //        INNER JOIN
        //    (SELECT 
        //        a.USERID, GROUP_CONCAT(b.FULLNAME) AS roles
        //    FROM
        //        base_user a
        //    LEFT JOIN base_role b ON (FIND_IN_SET(b.ROLEID, a.ROLEID))
        //    GROUP BY a.userid) d ON d.userid = a.userid
        //        LEFT JOIN
        //    base_organize b ON b.ORGANIZEID = a.ORGANIZEID
        //        LEFT JOIN
        //    base_department c ON c.departmentid = a.DEPARTMENTID
        //WHERE
        //    a.ACCOUNT <> 'System'{0}
        //";
        //            var db = new RepositoryFactory().BaseRepository();
        //            var data = db.FindList<UserInfoEntity>(string.Format(querysql, condition.ToString(), OperatorProvider.Provider.Current().DeptId), parameters.ToArray()).ToList();

        //            total = int.Parse(db.FindObject(string.Format(countsql, condition.ToString(), OperatorProvider.Provider.Current().DeptId), parameters.ToArray()).ToString());
        //            return data;
        //        }

        public List<UserEntity> GetUsersByIds(IEnumerable<string> enumerable)
        {
            return this.BaseRepository().IQueryable(x => enumerable.Contains(x.UserId)).ToList();
        }

        public List<UserEntity> GetUsersByDuty(string aduit)
        {
            return this.BaseRepository().IQueryable(x => x.RoleName.Contains(aduit)).ToList();
        }

        public List<UserEntity> GetList(string deptid, int pageSize, int pageIndex, out int total)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<UserEntity>()
                        join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.DepartmentId
                        join q3 in db.IQueryable<PeopleEntity>() on q1.UserId equals q3.ID into t3
                        from q3 in t3.DefaultIfEmpty()
                        where q1.DepartmentId == deptid
                        orderby q3.Planer, q1.RealName
                        select new { q1.UserId, q1.RealName, q1.DepartmentId, q2.FullName, q3.Photo };
            total = query.Count();
            return query.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList().Select(x => new UserEntity() { UserId = x.UserId, RealName = x.RealName, DepartmentId = x.DepartmentId, DepartmentName = x.FullName, Photo = x.Photo }).ToList();
        }

        public List<UserEntity> GetList(string[] depts, int pageSize, int pageIndex, out int total)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<UserEntity>()
                        join q2 in db.IQueryable<PeopleEntity>() on q1.UserId equals q2.ID into into2
                        from q2 in into2.DefaultIfEmpty()
                        where depts.Contains(q1.DepartmentId) && q2.FingerMark == "yes"
                        orderby q2.Planer, q1.RealName
                        select new { q1, q2 };

            total = query.Count();
            var data = query.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            return data.Select(x => x.q1).ToList();
        }

        public void Edit(UserCertificateEntity model)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var entity = db.FindEntity<UserCertificateEntity>(model.Id);
                if (entity == null)
                {
                    db.Insert(model);
                    db.Insert(model.Files);
                }
                else
                {
                    entity.CertificateName = model.CertificateName;
                    entity.firsttime = model.firsttime;
                    entity.effectivestarttime = model.effectivestarttime;
                    entity.effectiveendtime = model.effectiveendtime;
                    entity.rechecktime = model.rechecktime;
                    entity.numbercode = model.numbercode;
                    entity.issue = model.issue;
                    entity.modifytime = model.modifytime;
                    entity.modifyuser = model.modifyuser;
                    entity.modifyuserid = model.modifyuserid;
                    entity.approvaltime = model.approvaltime;
                    entity.neweffectivetime = model.neweffectivetime;
                    entity.effectivetime = model.effectivetime;
                    entity.getthetime = model.getthetime;

                    db.Update(entity);

                    db.Delete<FileInfoEntity>(x => x.RecId == entity.Id);
                    db.Insert(model.Files);
                }

                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

        }

        public List<UserInfoEntity> GetList(string[] depts, int pageSize, int pageIndex, out int total, string key, string value)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<UserEntity>()
                        join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.DepartmentId
                        join q3 in db.IQueryable<DepartmentEntity>() on q2.ParentId equals q3.DepartmentId into into3
                        from q3 in into3.DefaultIfEmpty()
                            //join q4 in db.IQueryable<OrganizeEntity>() on q1.OrganizeId equals q4.OrganizeId
                        where depts.Contains(q1.DepartmentId)
                        select new { q1, q2, q3 };

            if (!string.IsNullOrEmpty(key))
            {
                switch (key)
                {
                    case "Account":
                        query = query.Where(x => x.q1.Account.Contains(value));
                        break;
                    case "RealName":
                        query = query.Where(x => x.q1.RealName.Contains(value));
                        break;
                    case "Mobile":
                        query = query.Where(x => x.q1.Mobile.Contains(value));
                        break;
                    default:
                        break;
                }
            }

            total = query.Count();
            var data = query.OrderBy(x => x.q1.CreateDate).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            return data.Select(x => new UserInfoEntity()
            {
                UserId = x.q1.UserId,
                Account = x.q1.Account,
                RealName = x.q1.RealName,
                Gender = x.q1.Gender,
                Mobile = x.q1.Mobile,
                DeptName = x.q2.Nature == "班组" ? x.q3.FullName + "/" + x.q2.FullName : x.q2.FullName,
                DutyName = x.q1.DutyName,
                PostName = x.q1.PostName,
                RoleName = x.q1.RoleName,
                Manager = x.q1.Manager,
                DepartmentId = x.q1.DepartmentId
            }).ToList();
        }

        public void Register(UserFaceEntity data)
        {
            var db = DbFactory.Base() as Database;
            var entity = db.dbcontext.Set<UserFaceEntity>().Find(data.UserId);
            if (entity == null) db.dbcontext.Set<UserFaceEntity>().Add(data);
            else
            {
                db.dbcontext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                entity.FaceStream = data.FaceStream;
            }

            var files = db.dbcontext.Set<FileInfoEntity>().Where(x => x.RecId == data.UserId && x.Description == data.FaceFile.Description).ToList();
            files.ForEach(x => db.dbcontext.Entry(x).State = System.Data.Entity.EntityState.Deleted);

            db.dbcontext.Set<FileInfoEntity>().Add(data.FaceFile);
            db.dbcontext.SaveChanges();
        }

        public List<UserFaceEntity> GetUserFaces()
        {
            var db = DbFactory.Base() as Database;

            var query = from q in db.IQueryable<UserFaceEntity>()
                        select q;

            return query.ToList();
        }
        #endregion

        public void Save(List<UserEntity> employees)
        {
            var userSet = _context.Set<UserEntity>();
            var peopleSet = _context.Set<PeopleEntity>();
            var departmentSet = _context.Set<DepartmentEntity>();

            foreach (var item in employees)
            {
                var entity = userSet.Find(item.UserId);

                if (entity == null)
                {
                    entity = item;
                    userSet.Add(item);
                }
                else
                {
                    _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;

                    entity.EnabledMark = item.EnabledMark;
                    entity.ModifyDate = item.ModifyDate;
                    entity.ModifyUserId = item.ModifyUserId;
                    entity.ModifyUserName = item.ModifyUserName;

                    entity.RoleId = item.RoleId;
                    entity.RoleName = item.RoleName;
                    entity.Gender = item.Gender;
                    entity.Account = item.Account;
                    entity.RealName = item.RealName;
                    entity.DepartmentId = item.DepartmentId;
                    entity.DepartmentName = item.DepartmentName;
                    entity.DepartmentCode = item.DepartmentCode;
                    entity.DutyId = item.DutyId;
                    entity.DutyName = item.DutyName;
                    entity.UserType = item.UserType;
                    entity.IDENTIFYID = item.IDENTIFYID;
                    entity.IsSpecial = item.IsSpecial;
                    entity.FourPersonType = item.FourPersonType;
                    entity.Birthday = item.Birthday;
                    entity.Nation = item.Nation;
                    entity.Craft = item.Craft;
                    entity.CraftAge = item.CraftAge;
                    entity.Degrees = item.Degrees;
                    entity.EnCode = item.EnCode;
                    entity.Telephone = item.Telephone;
                    //entity.IsEpiboly = item.IsEpiboly;
                    //entity.IsFourPerson = item.IsFourPerson;
                    //entity.SpecialtyType = item.SpecialtyType;
                    entity.PostId = item.PostId;
                    entity.PostCode = item.PostCode;
                    entity.PostName = item.PostName;
                    entity.Political = item.Political;
                    entity.Mobile = item.Mobile;
                    entity.IsSpecialEqu = item.IsSpecialEqu;
                    entity.Age = item.Age;
                    entity.Native = item.Native;
                    entity.TechnicalGrade = item.TechnicalGrade;
                    entity.JobTitle = item.JobTitle;
                    entity.LateDegrees = item.LateDegrees;
                    entity.HealthStatus = item.HealthStatus;
                    entity.EnterTime = item.EnterTime;
                    entity.HeadIcon = item.HeadIcon;
                    entity.SignImg = item.SignImg;

                    if (!string.IsNullOrEmpty(item.Password) && !string.IsNullOrEmpty(item.Secretkey))
                    {
                        entity.Password = item.Password;
                        entity.Secretkey = item.Secretkey;
                    }
                }

                var people = peopleSet.Find(item.UserId);
                if (people == null)
                {
                    people = new PeopleEntity();
                    people.ID = item.UserId;
                    peopleSet.Add(people);
                }
                else
                {
                    _context.Entry(people).State = System.Data.Entity.EntityState.Modified;
                }
                people.Name = item.RealName;
                people.Sex = item.Gender == 1 ? "男" : "女";
                people.BZID = item.DepartmentId;
                people.BZName = item.DepartmentName;
                people.BZCode = item.DepartmentCode;
                people.UserType = item.UserType;
                people.IsSpecial = item.IsSpecial;
                people.QuarterId = item.PostId;
                people.Quarters = item.PostName;
                people.Age = item.Age;
                people.Visage = item.Political;
                people.LinkWay = item.Mobile;
                people.TecLevel = item.TechnicalGrade;
                people.IdentityNo = item.IDENTIFYID;
                people.Folk = item.Nation;
                people.Degree = item.Degrees;
                people.LabourNo = item.EnCode;
                people.EntryDate = item.EnterTime;
                people.HealthStatus = item.HealthStatus;
                people.FingerMark = "yes";
                people.UserAccount = item.Account;
                people.Planer = item.PostCode;
                people.Birthday = item.Birthday;
                people.Native = item.Native;
                people.OldDegree = item.Degrees;
                people.NewDegree = item.LateDegrees;
                people.CurrentWorkAge = item.CraftAge;
                people.WorkKind = item.Craft;
                people.JobName = item.JobTitle;
                people.Telephone = item.Telephone;
                people.SpecialtyType = item.SpecialtyType;
                people.Photo = item.HeadIcon;
                people.Signature = item.SignImg;
                people.RoleDutyId = item.DutyId;
                people.RoleDutyName = item.DutyName;
                people.IsEpiboly = item.IsEpiboly;
                people.IsFourPerson = item.IsFourPerson;
                people.SpecialtyType = item.SpecialtyType;

                var dept = departmentSet.Find(people.BZID);
                if (dept != null)
                {
                    var parentdept = departmentSet.Find(dept.ParentId);
                    if (parentdept != null)
                    {
                        people.DeptId = parentdept.DepartmentId;
                        people.DeptCode = parentdept.EnCode;
                        people.DeptName = parentdept.FullName;
                    }
                }
            }

            _context.SaveChanges();
        }

        public UserFaceEntity GetUserFace(string userid)
        {
            var userfaceSet = _context.Set<UserFaceEntity>();
            var userface = userfaceSet.Find(userid);
            return userface;
        }

        public List<UserEntity> GetList(string name, string account, string[] depts, int pagesize, int pageindex, out int total)
        {
            var query = from q1 in _context.Set<UserEntity>()
                        join q2 in _context.Set<DepartmentEntity>() on q1.DepartmentId equals q2.DepartmentId
                        where depts.Contains(q2.DepartmentId)
                        select new { q1.UserId, q1.Account, q1.RealName, q1.Gender, q1.Mobile, q2.FullName, q1.RoleName, q1.EnabledMark, q1.CreateDate };

            if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.RealName.Contains(name));
            if (!string.IsNullOrEmpty(account)) query = query.Where(x => x.Account.Contains(account));

            total = query.Count();
            var data = query.OrderBy(x => x.CreateDate).Skip(pagesize * (pageindex - 1)).Take(pagesize).ToList();
            return data.Select(x => new UserEntity { UserId = x.UserId, Account = x.Account, RealName = x.RealName, Gender = x.Gender, Mobile = x.Mobile, DepartmentName = x.FullName, RoleName = x.RoleName, EnabledMark = x.EnabledMark }).ToList();
        }

        public UserEntity Get(string dutyDepartmentName, string dutyUser)
        {
            var query = from q1 in _context.Set<UserEntity>()
                        join q2 in _context.Set<DepartmentEntity>() on q1.DepartmentId equals q2.DepartmentId
                        where q1.RealName == dutyUser && q2.FullName == dutyDepartmentName
                        select q1;

            return query.FirstOrDefault();
        }
    }
}
