using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.Entity.SystemManage.ViewModel;
using System.Collections.Generic;

namespace BSFramework.Application.IService.SystemManage
{
    /// <summary>
    /// 描 述：数据字典明细
    /// </summary>
    public interface IDataItemDetailService
    {
        #region 获取数据
        /// <summary>
        /// 明细列表
        /// </summary>
        /// <param name="itemId">分类Id</param>
        /// <returns></returns>
        IEnumerable<DataItemDetailEntity> GetList(string itemId);

        IEnumerable<DataItemDetailEntity> GetListByName(string itemName);
        /// <summary>
        /// 明细实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        DataItemDetailEntity GetEntity(string keyValue);
        /// <summary>
        /// 获取数据字典列表（给绑定下拉框提供的）
        /// </summary>
        /// <returns></returns>
        IEnumerable<DataItemModel> GetDataItemList();
        #endregion

        #region 验证数据
        /// <summary>
        /// 项目值不能重复
        /// </summary>
        /// <param name="itemValue">项目值</param>
        /// <param name="keyValue">主键</param>
        /// <param name="itemId">分类Id</param>
        /// <returns></returns>
        bool ExistItemValue(string itemValue, string keyValue, string itemId);
        List<DataItemDetailEntity> GetConfigs(string[] data);

        /// <summary>
        /// 项目名不能重复
        /// </summary>
        /// <param name="itemName">项目名</param>
        /// <param name="keyValue">主键</param>
        /// <param name="itemId">分类Id</param>
        /// <returns></returns>
        bool ExistItemName(string itemName, string keyValue, string itemId);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除明细
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        /// <summary>
        /// 保存明细表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="dataItemDetailEntity">明细实体</param>
        /// <returns></returns>
        void SaveForm(string keyValue, DataItemDetailEntity dataItemDetailEntity);
        List<DataItemDetailEntity> GetDataItems(string category);
        DataItemDetailEntity GetDetail(string category, string itemname);
        /// <summary>
        /// 保存人身风险预控规则
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="entity"></param>
        void SaveOrUpdateRole(string roleId, DataItemDetailEntity entity);

        void EditList(string category, DataItemDetailEntity[] models);
        #endregion
    }
}
