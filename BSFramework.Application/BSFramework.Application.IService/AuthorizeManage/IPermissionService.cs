using BSFramework.Application.Entity.AuthorizeManage;
using BSFramework.Application.Entity.BaseManage;
using System.Collections.Generic;

namespace BSFramework.Application.IService.AuthorizeManage
{
    /// <summary>
    /// 描 述：权限配置管理（角色、岗位、职位、用户组、用户）
    /// </summary>
    public interface IPermissionService
    {
        #region 获取数据
        /// <summary>
        /// 获取成员列表
        /// </summary>
        /// <param name="objectId">对象Id</param>
        /// <returns></returns>
        IEnumerable<UserRelationEntity> GetMemberList(string objectId);
         /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        IEnumerable<UserRelationEntity> GetObjectList(string userId);
        /// <summary>
        /// 获取功能列表
        /// </summary>
        /// <param name="objectId">对象Id</param>
        /// <returns></returns>
        IEnumerable<AuthorizeEntity> GetModuleList(string objectId);
        /// <summary>
        /// 获取按钮列表
        /// </summary>
        /// <param name="objectId">对象Id</param>
        /// <returns></returns>
        IEnumerable<AuthorizeEntity> GetModuleButtonList(string objectId);
        /// <summary>
        /// 获取视图列表
        /// </summary>
        /// <param name="objectId">对象Id</param>
        /// <returns></returns>
        IEnumerable<AuthorizeEntity> GetModuleColumnList(string objectId);
        /// <summary>
        /// 获取数据权限列表
        /// </summary>
        /// <param name="objectId">对象Id</param>
        /// <returns></returns>
        IEnumerable<AuthorizeDataEntity> GetAuthorizeDataList(string objectId);
        #endregion

        #region 提交数据
        /// <summary>
        /// 添加成员
        /// </summary>
        /// <param name="authorizeType">权限分类</param>
        /// <param name="objectId">对象Id</param>
        /// <param name="userIds">成员Id</param>
        void SaveMember(int authorizeType, string objectId, string[] userIds);
        /// <summary>
        /// 添加授权
        /// </summary>
        /// <param name="authorizeType">权限分类</param>
        /// <param name="objectId">对象Id</param>
        /// <param name="moduleIds">功能Id</param>
        /// <param name="moduleButtonIds">按钮Id</param>
        /// <param name="moduleColumnIds">视图Id</param>
        /// <param name="authorizeDataList">数据权限</param>
        void SaveAuthorize(int authorizeType, string objectId, string[] moduleIds, string[] moduleButtonIds, string[] moduleColumnIds, IEnumerable<AuthorizeDataEntity> authorizeDataList);
        #endregion
    }
}
