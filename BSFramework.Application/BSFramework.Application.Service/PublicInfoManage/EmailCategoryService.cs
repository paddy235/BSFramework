using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.PublicInfoManage;
using BSFramework.Data.Repository;
using BSFramework.Util.Extension;
using System.Collections.Generic;
using System.Linq;

namespace BSFramework.Application.Service.PublicInfoManage
{
    /// <summary>
    /// 描 述：邮件分类
    /// </summary>
    public class EmailCategoryService : RepositoryFactory<EmailCategoryEntity>, IEmailCategoryService
    {
        #region 获取数据
        /// <summary>
        /// 分类列表
        /// </summary>
        /// <param name="UserId">用户Id</param>
        /// <returns></returns>
        public IEnumerable<EmailCategoryEntity> GetList(string UserId)
        {
            var expression = LinqExtensions.True<EmailCategoryEntity>();
            expression = expression.And(t => t.CreateUserId == UserId);
            return this.BaseRepository().IQueryable(expression).ToList();
        }
        /// <summary>
        /// 分类实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public EmailCategoryEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }
        /// <summary>
        /// 保存分类表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="emailCategoryEntity">分类实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, EmailCategoryEntity emailCategoryEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                emailCategoryEntity.Modify(keyValue);
                this.BaseRepository().Update(emailCategoryEntity);
            }
            else
            {
                emailCategoryEntity.Create();
                this.BaseRepository().Insert(emailCategoryEntity);
            }
        }
        #endregion
    }
}
