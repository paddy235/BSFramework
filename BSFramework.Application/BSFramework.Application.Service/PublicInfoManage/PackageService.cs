using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.PublicInfoManage;
using BSFramework.Data.Repository;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Linq;
using BSFramework.Data;
using BSFramework.Util;
using BSFramework.Util.Extension;
using System.Data;
using System.Linq.Expressions;
using System;



namespace BSFramework.Application.Service.PublicInfoManage
{
    /// <summary>
    /// 描 述：app版本
    /// </summary>
    public class PackageService : RepositoryFactory<PackageEntity>, PackageIService
    {
        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public IEnumerable<PackageEntity> GetList(string queryJson)
        {
            return this.BaseRepository().IQueryable().ToList();
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public PackageEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        /// <summary>
        /// 根据应用类型获取最新版本号和下载路径
        /// </summary>
        /// <param name="packtype">0-安卓APP，1-安卓终端</param>
        /// <returns></returns>
        public PackageEntity GetEntity(int packtype)
        {
            return this.BaseRepository().IQueryable(t => t.PackType == packtype).OrderByDescending(t => t.CreateDate).FirstOrDefault();
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<PackageEntity> GetPageList(Pagination pagination, string queryJson)
        {

            DatabaseType dataType = DbHelper.DbType;

            var db = new RepositoryFactory().BaseRepository();

            var query = from a in db.IQueryable<PackageEntity>()
                        select a;
            var queryParam = queryJson.ToJObject();
            //搜索关键字
            if (!queryParam["keyword"].IsEmpty())
            {
                string keyword = queryParam["keyword"].ToString();
                //pagination.conditionJson += string.Format(" and t.AppName like '%{0}%'", keyword);
                query = query.Where(x => x.AppName.Contains(keyword));
            }
            query = query.OrderByDescending(x => x.CreateDate);
            return query;
            //IEnumerable<PackageEntity> list = this.BaseRepository().FindListByProcPager(pagination, dataType);

            //return list;

        }


        public PackageEntity GetEntity(Expression<Func<PackageEntity, bool>> expression)
        {
            return BaseRepository().IQueryable(expression).OrderByDescending(x => x.CreateDate).FirstOrDefault();
        }

        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, PackageEntity entity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                PackageEntity se = this.BaseRepository().FindEntity(keyValue);
                if (se == null)
                {
                    entity.ID = keyValue;
                    entity.Create();
                    this.BaseRepository().Insert(entity);


                }
                else
                {
                    entity.Modify(keyValue);
                    this.BaseRepository().Update(entity);
                }
            }
            else
            {
                entity.Create();
                this.BaseRepository().Insert(entity);
            }
        }

        #endregion
    }
}
