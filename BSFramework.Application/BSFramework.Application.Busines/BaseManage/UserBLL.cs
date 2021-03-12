using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.MessageManage;
using BSFramework.Application.IService.BaseManage;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Cache.Factory;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.Offices;
using BSFramework.Util.SignalR;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using BSFramework.Application.Entity.CertificateManage;

namespace BSFramework.Application.Busines.BaseManage
{
    /// <summary>
    /// 描 述：用户管理
    /// </summary>
    public class UserBLL
    {
        private IUserService service = new UserService();
        private IUserInfoService service1 = new UserInfoService();
        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = BSFramework.Util.Config.GetValue("SoftName") + "_userCache";

        public List<UserEntity> GetList(string deptid, int pageSize, int pageIndex, out int total)
        {
            return service.GetList(deptid, pageSize, pageIndex, out total);
        }

        #region 获取数据
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <returns></returns>
        public List<UserEntity> GetTable()
        {
            return service.GetTable();
        }

        public UserEntity GetUserByAccount(string account)
        {
            return service.GetUserByAccount(account);
        }
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserEntity> GetList()
        {
            return service.GetList();
        }

        public List<UserEntity> GetList(string[] depts, int pageSize, int pageIndex, out int total)
        {
            return service.GetList(depts, pageSize, pageIndex, out total);
        }

        public IQueryable<UserEntity> GetUserList()
        {
            return service.GetUserList();
        }
        ///// <summary>
        ///// 用户列表
        ///// </summary>
        ///// <param name="pagination">分页</param>
        ///// <param name="queryJson">查询参数</param>
        ///// <returns></returns>
        //public DataTable GetPageList(Pagination pagination, string queryJson)
        //{
        //    return service.GetPageList(pagination, queryJson);
        //}



        public IEnumerable<UserEntity> GetDeptUsers(string deptId)
        {
            return service.GetDeptUser(deptId);
        }

        public IEnumerable<UserEntity> GetDeptUserBook(string deptId, string userId)
        {
            return service.GetDeptUserBook(deptId, userId);
        }

        /// <summary>
        /// 用户列表（ALL）
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllTable()
        {
            return service.GetAllTable();
        }
        /// <summary>
        /// 用户实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public UserEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }

        public string InsertUser(UserEntity userEntity)
        {
            return service.InsertUser(userEntity);
        }
        /// <summary
        /// 用户基本信息
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public UserInfoEntity GetUserInfoEntity(string keyValue)
        {
            return service1.GetUserInfoEntity(keyValue);
        }
        public UserInfoEntity GetUserInfoByAccount(string account)
        {
            return service1.GetUserInfoByAccount(account);
        }
        /// <summary>
        /// 根据用户ID和类别获取用户拥有的资源名称，多个用英文逗号分隔
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="category">数据类别，1:部门名称,2:角色名称,3:岗位名称,4:职位名称,5:工作组</param>
        /// <returns></returns>
        public string GetObjectName(string userId, int category)
        {
            return service.GetObjectName(userId, category);
        }

        public List<UserEntity> GetUsersByDuty(string aduit)
        {
            return service.GetUsersByDuty(aduit);
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 账户不能重复
        /// </summary>
        /// <param name="account">账户值</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistAccount(string account, string keyValue = "")
        {
            return service.ExistAccount(account, keyValue);
        }

        public void Edit(UserCertificateEntity model)
        {
            service.Edit(model);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            try
            {
                service.RemoveForm(keyValue);
                CacheFactory.Cache().RemoveCache(cacheKey);
                UpdateIMUserList(keyValue, false, null);
            }
            catch (Exception)
            {
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
            keyValue = service.SaveForm(keyValue, userEntity);
            CacheFactory.Cache().RemoveCache(cacheKey);
            UpdateIMUserList(keyValue, true, userEntity);
            return keyValue;
        }

        public string SaveFormNew(string keyValue, UserEntity userEntity)
        {
            try
            {
                keyValue = service.SaveFormNew(keyValue, userEntity);
                CacheFactory.Cache().RemoveCache(cacheKey);
                UpdateIMUserList(keyValue, true, userEntity);
                return keyValue;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 修改用户基本信息
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="userEntity">用户实体</param>
        /// <returns></returns>
        public string UpdateUserInfo(string keyValue, UserEntity userEntity)
        {
            try
            {
                service.UpdateEntity(userEntity);
                CacheFactory.Cache().RemoveCache(cacheKey);
                return keyValue;
            }
            catch (Exception)
            {
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
            try
            {
                service.RevisePassword(keyValue, Password);
                CacheFactory.Cache().RemoveCache(cacheKey);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 修改用户状态
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="State">状态：1-启动；0-禁用</param>
        public void UpdateState(string keyValue, int State)
        {
            try
            {
                service.UpdateState(keyValue, State);
                CacheFactory.Cache().RemoveCache(cacheKey);
                if (State == 0)
                {
                    UpdateIMUserList(keyValue, false, null);
                }
                else
                {
                    UserEntity entity = service.GetEntity(keyValue);
                    UpdateIMUserList(keyValue, true, entity);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public UserEntity CheckLogin(string username, string password)
        {
            UserEntity userEntity = service.CheckLogin(username);
            if (userEntity != null)
            {
                if (userEntity.EnabledMark == 1)
                {
                    string dbPassword = Md5Helper.MD5(DESEncrypt.Encrypt(password.ToLower(), userEntity.Secretkey).ToLower(), 32).ToLower();
                    if (dbPassword == userEntity.Password)
                    {
                        DateTime LastVisit = DateTime.Now;
                        int LogOnCount = (userEntity.LogOnCount).ToInt() + 1;
                        if (userEntity.LastVisit != null)
                        {
                            userEntity.PreviousVisit = userEntity.LastVisit.ToDate();
                        }
                        userEntity.LastVisit = LastVisit;
                        userEntity.LogOnCount = LogOnCount;
                        userEntity.UserOnLine = 1;
                        service.UpdateEntity(userEntity);
                        return userEntity;
                        //return service1.GetUserInfoEntity(userEntity.UserId);
                    }
                    else
                    {
                        //throw new Exception("密码和账户名不匹配");
                        return null;
                    }
                }
                else
                {
                    //throw new Exception("账户名被系统锁定,请联系管理员");
                    return null;
                }
            }
            else
            {
                //throw new Exception("账户不存在，请重新输入");
                return null;
            }
        }

        public UserInfoEntity CheckLoginNew(string username, string password)
        {
            UserEntity userEntity = service.CheckLogin(username);
            if (userEntity != null)
            {
                if (userEntity.EnabledMark == 1)
                {
                    string dbPassword = Md5Helper.MD5(DESEncrypt.Encrypt(password.ToLower(), userEntity.Secretkey).ToLower(), 32).ToLower();
                    if (dbPassword == userEntity.Password)
                    {
                        DateTime LastVisit = DateTime.Now;
                        int LogOnCount = (userEntity.LogOnCount).ToInt() + 1;
                        if (userEntity.LastVisit != null)
                        {
                            userEntity.PreviousVisit = userEntity.LastVisit.ToDate();
                        }
                        userEntity.LastVisit = LastVisit;
                        userEntity.LogOnCount = LogOnCount;
                        userEntity.UserOnLine = 1;
                        service.UpdateEntity(userEntity);
                        return service1.GetUserInfoEntity(userEntity.UserId);
                    }
                    else
                    {
                        return null;
                        //throw new Exception("密码和账户名不匹配");
                    }
                }
                else
                {
                    return null;
                    //throw new Exception("账户名被系统锁定,请联系管理员");
                }
            }
            else
            {
                return null;
                //throw new Exception("账户不存在，请重新输入");
            }
        }

        public UserEntity Get(string dutyDepartmentName, string dutyUser)
        {
            return service.Get(dutyDepartmentName, dutyUser);
        }

        public List<UserEntity> GetList(string name, string account, string[] depts, int pagesize, int pageindex, out int total)
        {
            return service.GetList(name, account, depts, pagesize, pageindex, out total);
        }

        public List<UserEntity> GetUsersByIds(IEnumerable<string> enumerable)
        {
            return service.GetUsersByIds(enumerable);
        }

        /// <summary>
        /// 更新实时通信用户列表
        /// </summary>
        private void UpdateIMUserList(string keyValue, bool isAdd, UserEntity userEntity)
        {
            try
            {
                IMUserModel entity = new IMUserModel();
                OrganizeBLL bll = new OrganizeBLL();
                DepartmentBLL dbll = new DepartmentBLL();
                entity.UserId = keyValue;
                if (userEntity != null)
                {
                    entity.RealName = userEntity.RealName;
                    entity.DepartmentId = dbll.GetEntity(userEntity.DepartmentId).FullName;
                    entity.Gender = (int)userEntity.Gender;
                    entity.HeadIcon = userEntity.HeadIcon;
                    //entity.OrganizeId = bll.GetEntity(userEntity.OrganizeId).FullName; 
                }
                SendHubs.callMethod("upDateUserList", entity, isAdd);
            }
            catch
            {

            }
        }

        //public List<UserInfoEntity> GetUserData(string code, string account, string name, string tel, int page, int pagesize, out int total)
        //{
        //    return service.GetUserData(code, account, name, tel, page, pagesize, out total);
        //}
        #endregion

        #region 处理数据
        /// <summary>
        /// 导出用户列表
        /// </summary>
        /// <returns></returns>
        public void GetExportList(string condition, string keyword, string code, string isOrg)
        {
            //取出数据源
            DataTable exportTable = service.GetExportList(condition, keyword, code, isOrg);
            //设置导出格式
            ExcelConfig excelconfig = new ExcelConfig();
            //excelconfig.Title = "测试用户导出";
            //excelconfig.TitleFont = "微软雅黑";
            //excelconfig.TitlePoint = 25;
            excelconfig.HeadHeight = 50;
            excelconfig.HeadPoint = 12;
            excelconfig.HeadFont = "宋体";
            excelconfig.FileName = "用户导出.xls";
            excelconfig.IsAllSizeColumn = true;

            //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
            List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
            excelconfig.ColumnEntity = listColumnEntity;
            ColumnEntity columnentity = new ColumnEntity();
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "account", ExcelColumn = "账户" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "realname", ExcelColumn = "姓名" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "gender", ExcelColumn = "性别" });
            //excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "birthday", ExcelColumn = "生日" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "mobile", ExcelColumn = "手机" });
            //excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "telephone", ExcelColumn = "电话", Background = Color.Red });
            //excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "wechat", ExcelColumn = "微信" });
            //excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "manager", ExcelColumn = "主管" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "organize", ExcelColumn = "公司" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "department", ExcelColumn = "部门" });
            //excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "description", ExcelColumn = "说明" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "createdate", ExcelColumn = "创建日期" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "createusername", ExcelColumn = "创建人" });
            //调用导出方法
            ExcelHelper.ExcelDownload(exportTable, excelconfig);
            //从泛型Lis导出
            //TExcelHelper<DepartmentEntity>.ExcelDownload(department.GetList().ToList(), excelconfig);
        }
        #endregion

        public string GetTrainUser(string userid)
        {
            var user = this.GetEntity(userid);
            var dept = (new DepartmentBLL()).GetEntity(user.DepartmentId);
            return dept.TrainUser ?? string.Empty;
        }

        public List<UserInfoEntity> GetList(string[] depts, int pageSize, int pageIndex, out int total, string key, string value)
        {
            return service.GetList(depts, pageSize, pageIndex, out total, key, value);
        }

        public void Register(UserFaceEntity data)
        {
            service.Register(data);
        }

        public List<UserFaceEntity> GetUserFaces()
        {
            return service.GetUserFaces();
        }

        public void Save(List<UserEntity> employees) => service.Save(employees);

        public UserEntity Delete(string id)
        {
            throw new NotImplementedException();
        }

        public UserFaceEntity GetUserFace(string id)
        {
            return service.GetUserFace(id);
        }
    }
}
