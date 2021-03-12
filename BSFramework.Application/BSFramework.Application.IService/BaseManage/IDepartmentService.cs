using BSFramework.Application.Entity.BaseManage;
using System.Collections.Generic;

namespace BSFramework.Application.IService.BaseManage
{
    /// <summary>
    /// 描 述：部门管理
    /// </summary>
    public interface IDepartmentService
    {
        #region 获取数据
        /// <summary>
        /// 部门列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<DepartmentEntity> GetList();
        /// <summary>
        /// 部门列表通讯录
        /// </summary>
        /// <returns></returns>
        IEnumerable<DepartmentEntity> GetDepartmentList(string keyValue);
        int GetPeopleNumber(string keyValue);
        /// <summary>
        /// 部门实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        DepartmentEntity GetEntity(string keyValue);
        DepartmentEntity GetCompany(string deptId);
        #endregion

        #region 验证数据
        /// <summary>
        /// 部门编号不能重复
        /// </summary>
        /// <param name="enCode">编号</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        bool ExistEnCode(string enCode, string keyValue);
        /// <summary>
        /// 部门名称不能重复
        /// </summary>
        /// <param name="fullName">名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        bool ExistFullName(string fullName, string keyValue);
        object GetSubTeams(string departmentId);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        /// <summary>
        /// 保存部门表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="departmentEntity">机构实体</param>
        /// <returns></returns>
        void SaveForm(string keyValue, DepartmentEntity departmentEntity);
        DepartmentEntity GetRootDepartment();
        List<DepartmentEntity> GetSubDepartments(string id, string category);
        IList<DepartmentEntity> GetAllGroups();
        string GetFactory(string userId);
        List<DepartmentEntity> GetDepartments(string deptid);
        DepartmentEntity GetAuthorizationDepartment(string deptid);
        DepartmentEntity GetAuthorizationDepartmentApp(string deptid);

        List<DepartmentEntity> GetChildDepartments(string deptid);
        int GetSafeDays(string deptid);
        List<DepartmentEntity> GetSubDepartments(string[] depts);
        List<DepartmentEntity> GetDepartments(string[] depts);
        List<DepartmentEntity> GetGroupsByUser(string userId);
        void Save(List<DepartmentEntity> depts);
        DepartmentEntity Delete(string id);
        List<DepartmentEntity> GetAll();
        /// <summary>
        /// 获取某单位下所有的班组
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        List<DepartmentEntity> GetSubGroups(string departmentId);
        #endregion
    }
}
