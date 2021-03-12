using BSFramework.Application.Entity;
using BSFramework.Application.Entity.WeChatManage;
using BSFramework.Application.IService;
using BSFramework.Application.IService.WeChatManage;
using BSFramework.Data.Repository;
using System.Collections.Generic;
using System.Linq;

namespace BSFramework.Application.Service.WeChatManage
{
    /// <summary>
    /// 描 述：企业号应用
    /// </summary>
    public class WeChatAppService : RepositoryFactory<WeChatAppEntity>, IWeChatAppService
    {
        #region 获取数据
        /// <summary>
        /// 应用列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WeChatAppEntity> GetList()
        {
            return this.BaseRepository().IQueryable().OrderBy(t => t.CreateDate).ToList();
        }
        /// <summary>
        /// 应用实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public WeChatAppEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除应用
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }
        /// <summary>
        /// 应用（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="weChatAppEntity">应用实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, WeChatAppEntity weChatAppEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                weChatAppEntity.Modify(keyValue);
                this.BaseRepository().Update(weChatAppEntity);
            }
            else
            {
                weChatAppEntity.Create();
                this.BaseRepository().Insert(weChatAppEntity);
            }
        }
        #endregion
    }
}
