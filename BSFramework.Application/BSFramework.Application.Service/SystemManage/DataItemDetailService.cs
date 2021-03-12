using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.Entity.SystemManage.ViewModel;
using BSFramework.Application.IService.SystemManage;
using BSFramework.Data.Repository;
using BSFramework.Util.Extension;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace BSFramework.Application.Service.SystemManage
{
    /// <summary>
    /// 描 述：数据字典明细
    /// </summary>
    public class DataItemDetailService : RepositoryFactory<DataItemDetailEntity>, IDataItemDetailService
    {
        #region 获取数据
        /// <summary>
        /// 明细列表
        /// </summary>
        /// <param name="itemId">分类Id</param>
        /// <returns></returns>
        public IEnumerable<DataItemDetailEntity> GetList(string itemId)
        {
            return this.BaseRepository().IQueryable(t => t.ItemId == itemId).OrderBy(t => t.SortCode).ToList();
        }

        public IEnumerable<DataItemDetailEntity> GetListByName(string itemName)
        {
            return this.BaseRepository().IQueryable(t => t.ItemName == itemName).OrderBy(t => t.SortCode).ToList();
        }
        /// <summary>
        /// 明细实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public DataItemDetailEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        /// <summary>
        /// 获取数据字典列表（给绑定下拉框提供的）
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataItemModel> GetDataItemList()
        {
            //StringBuilder strSql = new StringBuilder();
            //strSql.Append(@"SELECT  i.ItemId ,
            //                        i.ItemCode AS EnCode ,
            //                        d.ItemDetailId ,
            //                        d.ParentId ,
            //                        d.ItemCode ,
            //                        d.ItemName ,
            //                        d.ItemValue ,
            //                        d.QuickQuery ,
            //                        d.SimpleSpelling ,
            //                        d.IsDefault ,
            //                        d.SortCode ,
            //                        d.EnabledMark
            //                FROM    Base_DataItemDetail d
            //                        LEFT JOIN Base_DataItem i ON i.ItemId = d.ItemId
            //                WHERE   1 = 1
            //                        AND d.EnabledMark = 1
            //                        AND d.DeleteMark = 0
            //                ORDER BY d.SortCode ASC");
            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<DataItemDetailEntity>()
                        join q2 in db.IQueryable<DataItemEntity>() on q1.ItemId equals q2.ItemId into t
                        from t1 in t.DefaultIfEmpty()
                        select new { q1, t1.ItemCode };
            var data = query.Select(d => new DataItemModel
            {
            EnCode = d.ItemCode  ,
                ItemDetailId=       d.q1.ItemDetailId ,
                ParentId =   d.q1.ParentId ,
                ItemName = d.q1.ItemName ,
                ItemValue =  d.q1.ItemValue ,
                SimpleSpelling = d.q1.SimpleSpelling ,
                SortCode = d.q1.SortCode ,
                EnabledMark = d.q1.EnabledMark,
                 ItemId  = d.q1.ItemId
            }).ToList();
            return data;
            //return new RepositoryFactory().BaseRepository().FindList<DataItemModel>(strSql.ToString());
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
            var expression = LinqExtensions.True<DataItemDetailEntity>();
            expression = expression.And(t => t.ItemValue == itemValue).And(t => t.ItemId == itemId);
            if (!string.IsNullOrEmpty(keyValue))
            {
                expression = expression.And(t => t.ItemDetailId != keyValue);
            }
            return this.BaseRepository().IQueryable(expression).Count() == 0 ? true : false;
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
            var expression = LinqExtensions.True<DataItemDetailEntity>();
            expression = expression.And(t => t.ItemName == itemName).And(t => t.ItemId == itemId);
            if (!string.IsNullOrEmpty(keyValue))
            {
                expression = expression.And(t => t.ItemDetailId != keyValue);
            }
            return this.BaseRepository().IQueryable(expression).Count() == 0 ? true : false;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除明细
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }
        /// <summary>
        /// 保存明细表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="dataItemDetailEntity">明细实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, DataItemDetailEntity dataItemDetailEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                dataItemDetailEntity.Modify(keyValue);
                this.BaseRepository().Update(dataItemDetailEntity);
            }
            else
            {
                dataItemDetailEntity.Create();
                this.BaseRepository().Insert(dataItemDetailEntity);
            }
        }

        public List<DataItemDetailEntity> GetDataItems(string category)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<DataItemDetailEntity>()
                        join q2 in db.IQueryable<DataItemEntity>() on q1.ItemId equals q2.ItemId
                        where q2.ItemName == category
                        orderby q1.SortCode, q1.CreateDate
                        select q1;
            return query.ToList();
        }

        public DataItemDetailEntity GetDetail(string category, string itemname)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<DataItemDetailEntity>()
                        join q2 in db.IQueryable<DataItemEntity>() on q1.ItemId equals q2.ItemId
                        where q2.ItemName == category && q1.ItemName == itemname
                        select q1;
            return query.FirstOrDefault();
        }

        public void SaveOrUpdateRole(string roleId, DataItemDetailEntity entity)
        {
            if (string.IsNullOrWhiteSpace(entity.ItemDetailId))
            {
                entity.ItemDetailId = roleId;
                this.BaseRepository().Insert(entity);
            }
            else
            {
                this.BaseRepository().Update(entity);
            }
        }

        public void EditList(string category, DataItemDetailEntity[] models)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            var query = from q1 in db.IQueryable<DataItemDetailEntity>()
                        join q2 in db.IQueryable<DataItemEntity>() on q1.ItemId equals q2.ItemId
                        select q1;

            var entities = query.ToList();
            foreach (var item in models)
            {
                var entity = entities.Find(x => x.ItemName == item.ItemName);
                if (entity != null) entity.ItemValue = item.ItemValue;
            }

            try
            {
                db.Update(entities);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public List<DataItemDetailEntity> GetConfigs(string[] data)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<DataItemDetailEntity>()
                        where data.Contains(q.ItemName)
                        select q;

            return query.ToList();
        }
        #endregion
    }
}
