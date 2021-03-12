using BSFramework.Data.Repository;
using System.Collections.Generic;
using System.Linq;
using System;
using BSFramework.Application.IService.AuthorizeManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.AuthorizeManage;

namespace BSFramework.Application.Service.BaseManage
{
    /// <summary>
    /// 描 述：权限配置管理（角色、岗位、职位、用户组、用户）
    /// </summary>
    public class PermissionService : RepositoryFactory, IPermissionService
    {
        #region 获取数据
        /// <summary>
        /// 获取成员列表
        /// </summary>
        /// <param name="objectId">对象Id</param>
        /// <returns></returns>
        public IEnumerable<UserRelationEntity> GetMemberList(string objectId)
        {
            return this.BaseRepository().IQueryable<UserRelationEntity>(t => t.ObjectId == objectId).OrderByDescending(t => t.CreateDate).ToList();
        }
        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public IEnumerable<UserRelationEntity> GetObjectList(string userId)
        {
            return this.BaseRepository().IQueryable<UserRelationEntity>(t => t.UserId == userId).OrderByDescending(t => t.CreateDate).ToList();
        }
        /// <summary>
        /// 获取功能列表
        /// </summary>
        /// <param name="objectId">对象Id</param>
        /// <returns></returns>
        public IEnumerable<AuthorizeEntity> GetModuleList(string objectId)
        {
            return this.BaseRepository().IQueryable<AuthorizeEntity>(t => t.ObjectId == objectId && t.ItemType == 1).ToList();
        }
        /// <summary>
        /// 获取按钮列表
        /// </summary>
        /// <param name="objectId">对象Id</param>
        /// <returns></returns>
        public IEnumerable<AuthorizeEntity> GetModuleButtonList(string objectId)
        {
            return this.BaseRepository().IQueryable<AuthorizeEntity>(t => t.ObjectId == objectId && t.ItemType == 2).ToList();
        }
        /// <summary>
        /// 获取视图列表
        /// </summary>
        /// <param name="objectId">对象Id</param>
        /// <returns></returns>
        public IEnumerable<AuthorizeEntity> GetModuleColumnList(string objectId)
        {
            return this.BaseRepository().IQueryable<AuthorizeEntity>(t => t.ObjectId == objectId && t.ItemType == 3).ToList();
        }
        /// <summary>
        /// 获取数据权限列表
        /// </summary>
        /// <param name="objectId">对象Id</param>
        /// <returns></returns>
        public IEnumerable<AuthorizeDataEntity> GetAuthorizeDataList(string objectId)
        {
            return this.BaseRepository().IQueryable<AuthorizeDataEntity>(t => t.ObjectId == objectId).OrderBy(t => t.SortCode).ToList();
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 添加成员
        /// </summary>
        /// <param name="authorizeType">权限分类</param>
        /// <param name="objectId">对象Id</param>
        /// <param name="userIds">成员Id</param>
        public void SaveMember(int authorizeType, string objectId, string[] userIds)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Delete<UserRelationEntity>(t => t.ObjectId == objectId);
                int SortCode = 1;
                foreach (string item in userIds)
                {
                    UserRelationEntity userRelationEntity = new UserRelationEntity();
                    userRelationEntity.Create();
                    userRelationEntity.Category = (int)authorizeType;
                    userRelationEntity.ObjectId = objectId;
                    userRelationEntity.UserId = item;
                    userRelationEntity.SortCode = SortCode++;
                    db.Insert(userRelationEntity);
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
        /// 添加授权
        /// </summary>
        /// <param name="authorizeType">权限分类</param>
        /// <param name="objectId">对象Id</param>
        /// <param name="moduleIds">功能Id</param>
        /// <param name="moduleButtonIds">按钮Id</param>
        /// <param name="moduleColumnIds">视图Id</param>
        /// <param name="authorizeDataList">数据权限</param>
        public void SaveAuthorize(int authorizeType, string objectId, string[] moduleIds, string[] moduleButtonIds, string[] moduleColumnIds, IEnumerable<AuthorizeDataEntity> authorizeDataList)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {

                #region 功能
                int SortCode = 1;
                db.Delete<AuthorizeEntity>(t => t.ObjectId == objectId);
                if (moduleIds.Length > 0)
                {
                    foreach (string item in moduleIds)
                    {

                        AuthorizeEntity authorizeEntity = new AuthorizeEntity();
                        authorizeEntity.Create();
                        authorizeEntity.Category = (int)authorizeType;
                        authorizeEntity.ObjectId = objectId;
                        authorizeEntity.ItemType = 1;
                        authorizeEntity.ItemId = item;
                        authorizeEntity.SortCode = SortCode++;
                        db.Insert(authorizeEntity);

                    }

                }
                else
                {
                    db.Delete<AuthorizeDataEntity>(objectId, "ObjectId");
                }
                #endregion

                if (moduleColumnIds.Length > 0)
                {
                    foreach (string mId in moduleColumnIds)
                    {
                        #region 数据权限
                        SortCode = 1;
                        int index = 0;
                        db.ExecuteBySql(string.Format("delete from BASE_AUTHORIZEDATA where ObjectId='{0}' and resourceid='{1}'", objectId, mId));
                        foreach (AuthorizeDataEntity authorizeDataEntity in authorizeDataList)
                        {
                            authorizeDataEntity.Create();
                            authorizeDataEntity.Category = (int)authorizeType;
                            authorizeDataEntity.ObjectId = objectId;
                            authorizeDataEntity.ResourceId = mId;
                            // authorizeDataEntity.Module = "Department";
                            authorizeDataEntity.SortCode = SortCode++;
                            db.Insert(authorizeDataEntity);
                            index++;
                        }
                        #endregion
                    }

                }
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        #endregion
    }
}
