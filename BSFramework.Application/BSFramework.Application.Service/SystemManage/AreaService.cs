using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.IService.SystemManage;
using BSFramework.Data.Repository;
using BSFramework.Util.Extension;
using System.Collections.Generic;
using System.Linq;

namespace BSFramework.Application.Service.SystemManage
{
    /// <summary>
    /// 描 述：区域管理
    /// </summary>
    public class AreaService : RepositoryFactory<AreaEntity>, IAreaService
    {
        #region 获取数据
        /// <summary>
        /// 区域列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AreaEntity> GetList()
        {
            return this.BaseRepository().IQueryable(t => t.Layer != 4).OrderBy(t => t.CreateDate).ToList();
        }
        /// <summary>
        /// 区域列表
        /// </summary>
        /// <param name="parentId">节点Id</param>
        /// <param name="keyword">关键字查询</param>
        /// <returns></returns>
        public IEnumerable<AreaEntity> GetList(string parentId, string keyword)
        {
            var expression = LinqExtensions.True<AreaEntity>();
            if (!string.IsNullOrEmpty(parentId))
            {
                expression = expression.And(t => t.ParentId == parentId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.AreaCode.Contains(keyword));
                expression = expression.Or(t => t.AreaName.Contains(keyword));
            }
            return this.BaseRepository().IQueryable(expression).OrderBy(t => t.CreateDate).ToList();
        }
        /// <summary>
        /// 区域实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public AreaEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除区域
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }
        /// <summary>
        /// 保存区域表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="{">区域实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, AreaEntity areaEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                areaEntity.Modify(keyValue);
                this.BaseRepository().Update(areaEntity);
            }
            else
            {
                areaEntity.Create();
                this.BaseRepository().Insert(areaEntity);
            }
        }
        #endregion
    }
}
