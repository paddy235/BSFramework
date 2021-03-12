using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BSFramework.Application.IService.PublicInfoManage
{
    /// <summary>
    /// 描 述：app版本
    /// </summary>
    public interface PackageIService
    {
        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        IEnumerable<PackageEntity> GetList(string queryJson);
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        PackageEntity GetEntity(string keyValue);

        /// <summary>
        /// 根据应用类型获取最新版本号和下载路径
        /// </summary>
        /// <param name="packType"></param>
        /// <returns></returns>
        PackageEntity GetEntity(int packType);
        IEnumerable<PackageEntity> GetPageList(Pagination pagination, string queryJson);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        PackageEntity GetEntity(Expression<Func<PackageEntity, bool>> expression);

        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        void SaveForm(string keyValue, PackageEntity entity);
        #endregion
    }
}
