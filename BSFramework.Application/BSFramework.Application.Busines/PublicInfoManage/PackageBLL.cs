using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.PublicInfoManage;
using BSFramework.Application.Service.PublicInfoManage;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System;

namespace BSFramework.Application.Busines.PublicInfoManage
{
    /// <summary>
    /// 描 述：app版本
    /// </summary>
    public class PackageBLL
    {
        private PackageIService service = new PackageService();

        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public IEnumerable<PackageEntity> GetList(string queryJson)
        {
            return service.GetList(queryJson);
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public PackageEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public PackageEntity GetEntity(System.Linq.Expressions.Expression<Func<PackageEntity,bool>> expression)
        {
            return service.GetEntity(expression);
        }

        /// <summary>
        /// 根据应用类型获取最新版本号和下载路径
        /// </summary>
        /// <param name="packType"></param>
        /// <returns></returns>
        public PackageEntity GetEntity(int packType)
        {
            return service.GetEntity(packType);
        }

        /// <summary>
        /// 获取分页的列表
        /// </summary>
        public IEnumerable<PackageEntity> GetPageList(Pagination pagination, string queryJson)
        {
            return service.GetPageList(pagination,queryJson);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
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
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, PackageEntity entity)
        {
            try
            {
                service.SaveForm(keyValue, entity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
