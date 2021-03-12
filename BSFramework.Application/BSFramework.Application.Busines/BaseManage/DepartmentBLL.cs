using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.IService.BaseManage;
using BSFramework.Application.Service.BaseManage;
using System;
using System.Linq;
using System.Collections.Generic;
using BSFramework.Cache.Factory;

namespace BSFramework.Application.Busines.BaseManage
{
    /// <summary>
    /// 描 述：部门管理
    /// </summary>
    public class DepartmentBLL
    {
        private IDepartmentService service = new DepartmentService();
        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = BSFramework.Util.Config.GetValue("SoftName") + "_DepartmentCache";

        #region 获取数据
        /// <summary>
        /// 部门列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DepartmentEntity> GetList()
        {
            return service.GetList();
        }

        public DepartmentEntity GetCompany(string deptId)
        {
            return service.GetCompany(deptId);
        }

        /// <summary>
        /// 部门列表通讯录
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public IEnumerable<DepartmentEntity> GetDepartmentList(string keyValue)
        {
            return service.GetDepartmentList(keyValue);
        }
        public int GetPeopleNumber(string keyValue)
        {
            return service.GetPeopleNumber(keyValue);
        }

        public object GetSubTeams(string departmentId)
        {
            return service.GetSubTeams(departmentId);
        }

        /// <summary>
        /// 获取某单位下所有的班组
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public List<DepartmentEntity> GetSubGroups(string departmentId)
        {
            return service.GetSubGroups(departmentId);
        }

        /// <summary>
        /// 部门实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public DepartmentEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
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
            return service.ExistEnCode(enCode, keyValue);
        }
        /// <summary>
        /// 部门名称不能重复
        /// </summary>
        /// <param name="fullName">名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistFullName(string fullName, string keyValue)
        {
            return service.ExistFullName(fullName, keyValue);
        }

        /// <summary>
        /// 获取某用户所在部门下所有的班组
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<DepartmentEntity> GetGroupsByUser(string userId)
        {
            return service.GetGroupsByUser(userId);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="keyValue">主键</param>
        public bool RemoveForm(string keyValue)
        {
            try
            {
                service.RemoveForm(keyValue);
                CacheFactory.Cache().RemoveCache(cacheKey);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 保存部门表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="departmentEntity">部门实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, DepartmentEntity departmentEntity)
        {
            try
            {
                service.SaveForm(keyValue, departmentEntity);
                CacheFactory.Cache().RemoveCache(cacheKey);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DepartmentEntity> GetSubDepartments(string id, string category)
        {
            return service.GetSubDepartments(id, category);
        }

        public List<DepartmentEntity> GetSubDepartments(string[] depts)
        {
            return service.GetSubDepartments(depts);
        }

        /// <summary>
        /// 获取父节点
        /// </summary>
        /// <param name="id"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public DepartmentEntity GetParent(string id, string category)
        {


            var dept = service.GetEntity(id);
            if (dept == null)
            {
                return new DepartmentEntity() { DepartmentId = "" };
            }
            if (dept.Nature == category)
            {
                return dept;
            }
            return GetParent(dept.ParentId, category);

        }
        public DepartmentEntity GetRootDepartment()
        {
            return service.GetRootDepartment();
        }

        public IList<DepartmentEntity> GetAllGroups()
        {
            return service.GetAllGroups();
        }

        public string GetFactory(string userId)
        {
            return service.GetFactory(userId);
        }

        public List<DepartmentEntity> GetDepartments(string deptid)
        {
            return service.GetDepartments(deptid);
        }

        public DepartmentEntity GetAuthorizationDepartment(string deptid)
        {
            return service.GetAuthorizationDepartment(deptid);
        }
        public DepartmentEntity GetAuthorizationDepartmentApp(string deptid)
        {
            return service.GetAuthorizationDepartmentApp(deptid);
        }
        public List<DepartmentEntity> GetChildDepartments(string deptid)
        {
            return service.GetChildDepartments(deptid);
        }

        public int GetSafeDays(string deptid)
        {
            return service.GetSafeDays(deptid);
        }

        public List<DepartmentEntity> GetDepartments(string[] depts)
        {
            return service.GetDepartments(depts);
        }

        public void Save(List<DepartmentEntity> depts)
        {
            service.Save(depts);
        }

        public DepartmentEntity Delete(string id)
        {
            return service.Delete(id);
        }
        /// <summary>
        /// 获取所有的部门信息
        /// </summary>
        /// <returns></returns>
        public List<DepartmentEntity> GetAll()
        {
            return service.GetAll();
        }
        #endregion
    }
}
