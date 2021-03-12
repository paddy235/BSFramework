using BSFramework.Application.Entity.BaseManage;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BSFramework.Application.Entity.CertificateManage;

namespace BSFramework.Application.IService.BaseManage
{
    /// <summary>
    /// 描 述：用户管理
    /// </summary>
    public interface IUserService
    {
        #region 获取数据
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <returns></returns>
        List<UserEntity> GetTable();

        /// <summary>
        /// 根据账号获取用户
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        UserEntity GetUserByAccount(string account);
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<UserEntity> GetList();

        IQueryable<UserEntity> GetUserList();
        ///// <summary>
        ///// 用户列表
        ///// </summary>
        ///// <param name="pagination">分页</param>
        ///// <param name="queryJson">查询参数</param>
        ///// <returns></returns>
        //DataTable GetPageList(Pagination pagination, string queryJson);
        List<UserEntity> GetList(string deptid, int pageSize, int pageIndex, out int total);


        /// <summary>
        /// 用户列表（ALL）
        /// </summary>
        /// <returns></returns>
        DataTable GetAllTable();
        /// <summary>
        /// 用户实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        UserEntity GetEntity(string keyValue);

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns></returns>
        UserEntity CheckLogin(string username);
        List<UserEntity> GetList(string[] depts, int pageSize, int pageIndex, out int total);

        /// <summary>
        /// 导出用户列表
        /// </summary>
        /// <returns></returns>
        DataTable GetExportList(string condition, string keyword, string code, string isOrg);

        /// <summary>
        /// 根据用户ID和类别获取用户拥有的资源名称，多个用英文逗号分隔
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="category">数据类别，1:部门名称,2:角色名称,3:岗位名称,4:职位名称,5:工作组</param>
        /// <returns></returns>
        string GetObjectName(string userId, int category);

        IEnumerable<UserEntity> GetDeptUser(string deptid);

        IEnumerable<UserEntity> GetDeptUserBook(string deptid, string userId);

        string InsertUser(UserEntity userEntity);
        #endregion

        #region 验证数据
        /// <summary>
        /// 账户不能重复
        /// </summary>
        /// <param name="account">账户值</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        bool ExistAccount(string account, string keyValue);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        /// <summary>
        /// 保存用户表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="userEntity">用户实体</param>
        /// <returns></returns>
        string SaveForm(string keyValue, UserEntity userEntity);

        string SaveFormNew(string keyValue, UserEntity userEntity);
        /// <summary>
        /// 修改用户登录密码
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="Password">新密码（MD5 小写）</param>
        void RevisePassword(string keyValue, string Password);
        /// <summary>
        /// 修改用户状态
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="State">状态：1-启动；0-禁用</param>
        void UpdateState(string keyValue, int State);
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="userEntity">实体对象</param>
        void UpdateEntity(UserEntity userEntity);
        //List<UserInfoEntity> GetUserData(string code, string account, string name, string tel, int page, int pagesize, out int total);
        List<UserEntity> GetUsersByIds(IEnumerable<string> enumerable);
        List<UserEntity> GetUsersByDuty(string aduit);
        void Edit(UserCertificateEntity model);
        List<UserInfoEntity> GetList(string[] depts, int pageSize, int pageIndex, out int total, string key, string value);
        void Register(UserFaceEntity data);
        List<UserFaceEntity> GetUserFaces();

        void Save(List<UserEntity> employees);
        UserFaceEntity GetUserFace(string id);
        List<UserEntity> GetList(string name, string account, string[] depts, int pagesize, int pageindex, out int total);
        UserEntity Get(string dutyDepartmentName, string dutyUser);
        #endregion
    }
}
