using BSFramework.Application.Entity.AuthorizeManage;
using BSFramework.Application.IService.AuthorizeManage;
using BSFramework.Data.Repository;
using BSFramework.Util.Extension;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BSFramework.Application.Service.BaseManage
{
    /// <summary>
    /// 描 述：系统按钮
    /// </summary>
    public class ModuleButtonService : RepositoryFactory<ModuleButtonEntity>, IModuleButtonService
    {
        #region 获取数据
        /// <summary>
        /// 按钮列表
        /// </summary>
        /// <returns></returns>
        public List<ModuleButtonEntity> GetList()
        {
            return this.BaseRepository().IQueryable().OrderBy(t => t.SortCode).ToList();
        }
        public DataTable GetButtonList()
        {
            return this.BaseRepository().FindTable("select encode from Base_ModuleButton");
        }
        /// <summary>
        /// 按钮列表
        /// </summary>
        /// <param name="moduleId">功能Id</param>
        /// <returns></returns>
        public List<ModuleButtonEntity> GetList(string moduleId)
        {
            var expression = LinqExtensions.True<ModuleButtonEntity>();
            expression = expression.And(t => t.ModuleId.Equals(moduleId));
            return this.BaseRepository().IQueryable(expression).OrderBy(t => t.SortCode).ToList();
        }
        public DataTable GetDataList(string moduleId, string objectId, string itemId)
        {
            return this.BaseRepository().FindTable(string.Format("select authorizetype from base_authorizedata where resourceid='{0}' and objectId='{1}' and itemId='{2}'", moduleId, objectId, itemId));
        }
        /// <summary>
        /// 按钮实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public ModuleButtonEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="moduleButtonEntity">按钮实体</param>
        public void AddEntity(ModuleButtonEntity moduleButtonEntity)
        {
            moduleButtonEntity.Create();
            this.BaseRepository().Insert(moduleButtonEntity);
        }
        #endregion
    }
}
