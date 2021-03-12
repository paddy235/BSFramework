using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.PublicInfoManage;
using BSFramework.Application.Service.PublicInfoManage;
using System;
using System.Collections.Generic;

namespace BSFramework.Application.Busines.PublicInfoManage
{
    /// <summary>
    /// 描 述：邮件分类
    /// </summary>
    public class EmailCategoryBLL
    {
        private IEmailCategoryService service = new EmailCategoryService();

        #region 获取数据
        /// <summary>
        /// 分类列表
        /// </summary>
        /// <param name="UserId">用户Id</param>
        /// <returns></returns>
        public IEnumerable<EmailCategoryEntity> GetList(string UserId)
        {
            return service.GetList(UserId);
        }
        /// <summary>
        /// 分类实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public EmailCategoryEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            try
            {
                service.RemoveForm(keyValue);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 保存分类表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="emailCategoryEntity">分类实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, EmailCategoryEntity emailCategoryEntity)
        {
            try
            {
                service.SaveForm(keyValue, emailCategoryEntity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
