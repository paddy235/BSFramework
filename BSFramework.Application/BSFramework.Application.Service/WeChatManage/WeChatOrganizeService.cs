using BSFramework.Application.Entity.WeChatManage;
using BSFramework.Application.IService.WeChatManage;
using BSFramework.Data.Repository;
using System.Collections.Generic;
using System.Linq;

namespace BSFramework.Application.Service.WeChatManage
{
    /// <summary>
    /// 描 述：企业号部门
    /// </summary>
    public class WeChatOrganizeService : RepositoryFactory<WeChatDeptRelationEntity>, IWeChatOrganizeService
    {
        #region 获取数据
        /// <summary>
        /// 部门列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WeChatDeptRelationEntity> GetList()
        {
            return this.BaseRepository().IQueryable().ToList();
        }
        /// <summary>
        /// 部门实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public WeChatDeptRelationEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }
        /// <summary>
        /// 部门（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="weChatDeptRelationEntity">部门实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, WeChatDeptRelationEntity weChatDeptRelationEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                weChatDeptRelationEntity.Modify(keyValue);
                this.BaseRepository().Update(weChatDeptRelationEntity);
            }
            else
            {
                weChatDeptRelationEntity.Create();
                this.BaseRepository().Insert(weChatDeptRelationEntity);
            }
        }
        #endregion
    }
}
