using BSFramework.Application.Entity.BaseManage;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.BaseManage
{
    /// <summary>
    /// 描 述：角色管理
    /// </summary>
    public interface IRoleService
    {
        #region 获取数据
        /// <summary>
        /// 角色列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<RoleEntity> GetList();

        List<RoleEntity> GetPageList(string rolename, int pagesize, int pageindex, out int total);


        /// <summary>
        /// 角色列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        IEnumerable<RoleEntity> GetPageList(Pagination pagination, string queryJson);
        /// <summary>
        /// 角色列表all
        /// </summary>
        /// <returns></returns>
        IEnumerable<RoleEntity> GetAllList();
        /// <summary>
        /// 角色实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        RoleEntity GetEntity(string keyValue);
        #endregion

        #region 验证数据
        /// <summary>
        /// 角色编号不能重复
        /// </summary>
        /// <param name="enCode">编号</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        bool ExistEnCode(string enCode, string keyValue);
        /// <summary>
        /// 角色名称不能重复
        /// </summary>
        /// <param name="fullName">名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        bool ExistFullName(string fullName, string keyValue);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        /// <summary>
        /// 保存角色表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="roleEntity">角色实体</param>
        /// <returns></returns>
        void SaveForm(string keyValue, RoleEntity roleEntity);
        void Save(List<RoleEntity> roles);
        RoleEntity Delete(string id);
        #endregion
    }
}
