using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.Entity.SystemManage.ViewModel;
using BSFramework.Application.IService.SystemManage;
using BSFramework.Application.Service.SystemManage;
using BSFramework.Cache.Factory;
using BSFramework.Util;
using System;
using System.Collections.Generic;

namespace BSFramework.Application.Busines.SystemManage
{
    /// <summary>
    /// 描 述：数据字典明细
    /// </summary>
    public class DataItemDetailBLL
    {
        private IDataItemDetailService service = new DataItemDetailService();
        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "dataItemCache";

        #region 获取数据
        /// <summary>
        /// 明细列表
        /// </summary>
        /// <param name="itemId">分类Id</param>
        /// <returns></returns>
        public IEnumerable<DataItemDetailEntity> GetList(string itemId)
        {
            return service.GetList(itemId);
        }

        public IEnumerable<DataItemDetailEntity> GetListByName(string itemName) 
        {
            return service.GetListByName(itemName);
        }
        /// <summary>
        /// 明细实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public DataItemDetailEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }

        public List<DataItemDetailEntity> GetConfigs(string[] data)
        {
            return service.GetConfigs(data);
        }

        /// <summary>
        /// 数据字典列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataItemModel> GetDataItemList()
        {
            return service.GetDataItemList();
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 项目值不能重复
        /// </summary>
        /// <param name="itemValue">项目值</param>
        /// <param name="keyValue">主键</param>
        /// <param name="itemId">分类Id</param>
        /// <returns></returns>
        public bool ExistItemValue(string itemValue, string keyValue, string itemId)
        {
            return service.ExistItemValue(itemValue, keyValue, itemId);
        }
        /// <summary>
        /// 项目名不能重复
        /// </summary>
        /// <param name="itemName">项目名</param>
        /// <param name="keyValue">主键</param>
        /// <param name="itemId">分类Id</param>
        /// <returns></returns>
        public bool ExistItemName(string itemName, string keyValue, string itemId)
        {
            return service.ExistItemName(itemName, keyValue, itemId);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除明细
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            try
            {
                service.RemoveForm(keyValue);
                CacheFactory.Cache().RemoveCache(cacheKey);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 保存明细表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="dataItemDetailEntity">明细实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, DataItemDetailEntity dataItemDetailEntity)
        {
            try
            {
                dataItemDetailEntity.SimpleSpelling = Str.PinYin(dataItemDetailEntity.ItemName);
                service.SaveForm(keyValue, dataItemDetailEntity);
                CacheFactory.Cache().RemoveCache(cacheKey);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DataItemDetailEntity> GetDataItems(string category)
        {
            return service.GetDataItems(category);
        }

        public DataItemDetailEntity GetDetail(string category, string itemname)
        {
            return service.GetDetail(category, itemname);
        }
        /// <summary>
        /// 保存人身风险预控规则
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="entity"></param>
        public void SaveOrUpdateRole(string roleId, DataItemDetailEntity entity)
        {
            service.SaveOrUpdateRole(roleId, entity);
        }

        public void EditList(string category, DataItemDetailEntity[] models)
        {
            service.EditList(category, models);
        }
        #endregion
    }
}
