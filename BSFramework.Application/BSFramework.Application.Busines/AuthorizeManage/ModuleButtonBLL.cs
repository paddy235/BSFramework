using BSFramework.Application.Entity.AuthorizeManage;
using BSFramework.Application.IService.AuthorizeManage;
using BSFramework.Application.Service.BaseManage;
using System;
using System.Collections.Generic;
using System.Data;

namespace BSFramework.Application.Busines.AuthorizeManage
{
    /// <summary>
    /// 描 述：系统按钮
    /// </summary>
    public class ModuleButtonBLL
    {
        private IModuleButtonService service = new ModuleButtonService();

        #region 获取数据
        /// <summary>
        /// 按钮列表
        /// </summary>
        /// <returns></returns>
        public List<ModuleButtonEntity> GetList()
        {
            return service.GetList();
        }
        public DataTable GetButtonList()
        {
            return service.GetButtonList();
        }
        /// <summary>
        /// 按钮列表
        /// </summary>
        /// <param name="moduleId">功能Id</param>
        /// <returns></returns>
        public List<ModuleButtonEntity> GetList(string moduleId)
        {
            return service.GetList(moduleId);
        }

        public DataTable GetDataList(string moduleId, string objectId, string itemId)
        {
            return service.GetDataList(moduleId, objectId, itemId);
        }
        /// <summary>
        /// 按钮实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public ModuleButtonEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 复制按钮
        /// </summary>
        /// <param name="KeyValue">主键</param>
        /// <param name="moduleId">功能主键</param>
        /// <returns></returns>
        public void CopyForm(string keyValue, string moduleId)
        {
            try
            {
                ModuleButtonEntity moduleButtonEntity = this.GetEntity(keyValue);
                moduleButtonEntity.ModuleId = moduleId;
                service.AddEntity(moduleButtonEntity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
