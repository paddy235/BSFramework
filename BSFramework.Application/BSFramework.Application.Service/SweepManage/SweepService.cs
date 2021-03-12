using BSFramework.Application.Entity.SweepManage;
using BSFramework.Application.IService.SweepManage;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.SweepManage
{
    /// <summary>
    /// 保洁管理
    /// </summary>
    public class SweepService : RepositoryFactory<SweepEntity>, ISweepService
    {
        #region 获取数据

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public List<SweepEntity> getSweepData()
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = db.IQueryable<SweepEntity>().ToList();
            return data;
        }

        /// <summary>
        /// id获取数据
        /// </summary>
        /// <returns></returns>
        public SweepEntity getSweepDataById(string Id)
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = db.FindEntity<SweepEntity>(Id);
            return data;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public List<SweepEntity> getSweepAndItemData()
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = db.IQueryable<SweepEntity>().ToList();
            foreach (var item in data)
            {
                item.Items = db.IQueryable<SweepItemEntity>(x => x.SweepId == item.ID).OrderBy(x => x.Sort).ToList();
            }
            return data;
        }

        /// <summary>
        /// id获取数据
        /// </summary>
        /// <returns></returns>
        public SweepEntity getSweepAndItemDataById(string Id)
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = db.FindEntity<SweepEntity>(Id);
            data.Items = db.IQueryable<SweepItemEntity>(x => x.SweepId == data.ID).OrderBy(x => x.Sort).ToList();
            return data;
        }
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public List<SweepEntity> GetPageSweepList(Pagination pagination, string queryJson)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = db.IQueryable<SweepEntity>();
            var queryParam = queryJson.ToJObject();


            if (!queryParam["keyWord"].IsEmpty())
            {
                var keyWord = queryParam["keyWord"].ToString();
                query = query.Where(x => x.CreateUserName.Contains(keyWord) || x.ModifyUserName.Contains(keyWord) || x.State.Contains(keyWord) || x.Situation.Contains(keyWord) || x.DutyUser.Contains(keyWord) || x.District.Contains(keyWord));
            }


            if (!queryParam["StartData"].IsEmpty())
            {
                var Start = Convert.ToDateTime(queryParam["StartData"].ToString());

                query = query.Where(x => x.CreateDate >= Start);
            }
            if (!queryParam["EndData"].IsEmpty())
            {
                var End = Convert.ToDateTime(queryParam["EndData"].ToString()).AddDays(1).AddMilliseconds(-1);
                query = query.Where(x => x.CreateDate <= End);
            }
            //区域id
            if (!queryParam["DistrictId"].IsEmpty())
            {
                var DistrictId = queryParam["DistrictId"].ToString();
                query = query.Where(x => x.DistrictId == DistrictId);
            }
            if (!queryParam["DistrictCode"].IsEmpty())
            {
                var DistrictCode = queryParam["DistrictCode"].ToString();
                query = query.Where(x => x.DistrictCode.StartsWith(DistrictCode));
            }
            if (!queryParam["State"].IsEmpty())
            {
                var State = queryParam["State"].ToString();
                query = query.Where(x => x.State == State);
            }
            pagination.records = query.Count();
            query = query.OrderByDescending(x => x.CreateDate).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
            return query.ToList();
        }

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public List<SweepEntity> GetPageSweepAndItemList(Pagination pagination, string queryJson)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = db.IQueryable<SweepEntity>();
            var queryParam = queryJson.ToJObject();


            if (!queryParam["keyWord"].IsEmpty())
            {
                var keyWord = queryParam["keyWord"].ToString();
                query = query.Where(x => x.CreateUserName.Contains(keyWord) || x.ModifyUserName.Contains(keyWord));
            }

            if (!queryParam["StartData"].IsEmpty())
            {
                var Start = Convert.ToDateTime(queryParam["StartData"].ToString());

                query = query.Where(x => x.CreateDate >= Start);
            }
            if (!queryParam["EndData"].IsEmpty())
            {
                var End = Convert.ToDateTime(queryParam["EndData"].ToString()).AddDays(1).AddMilliseconds(-1);
                query = query.Where(x => x.CreateDate <= End);
            }
            //区域id
            if (!queryParam["DistrictId"].IsEmpty())
            {
                var DistrictId = queryParam["DistrictId"].ToString();
                query = query.Where(x => x.DistrictId == DistrictId);
            }
            if (!queryParam["DistrictCode"].IsEmpty())
            {
                var DistrictCode = queryParam["DistrictCode"].ToString();
                query = query.Where(x => x.DistrictCode.StartsWith(DistrictCode));
            }
            if (!queryParam["State"].IsEmpty())
            {
                var State = queryParam["State"].ToString();
                query = query.Where(x => x.State == State);
            }

            if (!queryParam["UserId"].IsEmpty())
            {
                var UserId = queryParam["UserId"].ToString();
                query = query.Where(x => x.CreateUserId == UserId);
            }
            pagination.records = query.Count();
            query = query.OrderByDescending(x => x.CreateDate).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
            var data = query.ToList();
            foreach (var item in data)
            {
                item.Items = db.IQueryable<SweepItemEntity>(x => x.SweepId == item.ID).OrderBy(x => x.Sort).ToList();
            }
            return data;
        }

        #endregion
        #region 数据操作
        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="entity"></param>
        public void operateSweepAndItem(List<SweepEntity> entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                foreach (var item in entity)
                {
                    if (!string.IsNullOrEmpty(item.ID))
                    {
                        var dataEntity = db.FindEntity<SweepEntity>(item.ID);
                        if (dataEntity != null)
                        {
                            var items = dataEntity.Items;
                            dataEntity.ModifyDate = DateTime.Now;
                            dataEntity.State = item.State;
                            dataEntity.ModifyUserId = item.ModifyUserId;
                            dataEntity.ModifyUserName = item.ModifyUserName;
                            dataEntity.QualityDeptCode = item.QualityDeptCode;
                            dataEntity.QualityDeptId = item.QualityDeptId;
                            dataEntity.QualityDeptName = item.QualityDeptName;
                            dataEntity.Situation = item.Situation;
                            dataEntity.Items = null;
                            db.Update(dataEntity);

                        }
                        else
                        {
                            var items = dataEntity.Items;
                            dataEntity.CreateDate = DateTime.Now;
                            foreach (var itemsData in items)
                            {
                                itemsData.ID = Guid.NewGuid().ToString();
                                itemsData.SweepId = item.ID;
                            }
                            db.Insert(items);
                            dataEntity.Items = null;
                            db.Insert(item);

                        }
                    }
                    else
                    {
                        item.ID = Guid.NewGuid().ToString();
                        item.CreateDate = DateTime.Now;
                        foreach (var items in item.Items)
                        {
                            items.ID = Guid.NewGuid().ToString();
                            items.SweepId = item.ID;
                        }
                        db.Insert(item.Items);
                        item.Items = null;
                        db.Insert(item);

                    }

                }
                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyvalue"></param>
        public void removeSweepAndItem(string keyvalue)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var dataEntity = db.FindEntity<SweepEntity>(keyvalue);
                if (dataEntity != null)
                {
                    db.Delete(dataEntity);
                    var items = db.IQueryable<SweepItemEntity>(x => x.SweepId == keyvalue);
                    if (items.Count() > 0)
                    {
                        db.Delete(items);
                    }
                }
                db.Commit();
            }
            catch (Exception e)
            {
                db.Rollback();
                throw e;
            }
        }
        #endregion
    }
}
