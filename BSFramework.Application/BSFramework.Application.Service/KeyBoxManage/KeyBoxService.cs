using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.KeyboxManage;
using BSFramework.Application.IService.KeyBoxManage;
using BSFramework.Data.Repository;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Service.WorkMeeting;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.KeyBoxManage
{
    /// <summary>
    /// 钥匙管理
    /// </summary>
    public class KeyBoxService : RepositoryFactory<KeyBoxEntity>, IKeyBoxService
    {
        #region 获取数据

        #region keyBox

        /// <summary>
        /// id获取数据
        /// </summary>
        /// <returns></returns>
        public List<KeyBoxEntity> getKeyBoxData()
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = db.IQueryable<KeyBoxEntity>().ToList();
            return data;
        }

        /// <summary>
        /// id获取数据
        /// </summary>
        /// <returns></returns>
        public KeyBoxEntity getKeyBoxDataById(string Id)
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = db.IQueryable<KeyBoxEntity>(x => x.ID == Id).FirstOrDefault();
            return data;
        }

        /// <summary>
        /// 获取序号
        /// </summary>
        /// <returns></returns>
        public string getKeyBoxSort(string Category)
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = db.IQueryable<KeyBoxEntity>(x => x.Category == Category).OrderByDescending(x => x.Sort).FirstOrDefault();
            if (data == null)
            {
                return "01";
            }
            else
            {
                var num = Convert.ToInt32(data.Sort) + 1;
                if (num < 10)
                {
                    return "0" + num;
                }
                else
                {
                    return num.ToString();
                }
            }


        }


        /// <summary>
        /// 获取分类下的数量和借用数量
        /// </summary>
        /// <returns></returns>
        public object getKeyBoxCategoryData(string DeptId, List<string> category)
        {
            var db = new RepositoryFactory().BaseRepository();
            var DeptStr = string.Empty;
            var dept = db.FindEntity<DepartmentEntity>(DeptId);
            if (dept.TeamType == "01")
            {

                var orderService = new WorkOrderService();
                var workSet = orderService.GetWorkOrderGroup(DeptId);
                if (workSet.Count() > 0)
                {
                    DeptStr = string.Join(",", workSet.Select(x => x.departmentid));
                }
                else
                {
                    DeptStr = DeptId;
                }
            }
            else
            {
                DeptStr = DeptId;
            }

            var data = db.IQueryable<KeyBoxEntity>(x => DeptStr.Contains(x.DeptId)).ToList();

            var query = from a in category
                        join b in (
                        from c in data
                        group c by c.Category into d
                        select new { Category = d.Key, count = d.Count(), usecount = d.Count(x => x.State) }
                        ) on a equals b.Category into e
                        from f in e.DefaultIfEmpty()
                        select new
                        {
                            Category = a,
                            count = f != null ? f.count : 0,
                            usecount = f != null ? f.usecount : 0
                        };

            return query;
        }
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public List<KeyBoxEntity> GetPageKeyBoxList(Pagination pagination, string queryJson)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = db.IQueryable<KeyBoxEntity>();
            var queryParam = queryJson.ToJObject();
            //if (!queryParam["DeptCode"].IsEmpty())
            //{
            //    var deptcode = queryParam["deptCode"].ToString();
            //    query = query.Where(x => x.DeptCode.StartsWith(deptcode));
            //}
            var DeptId = queryParam["DeptId"].ToString();
            var dept = db.FindEntity<DepartmentEntity>(DeptId);
            var DeptStr = DeptId;
            if (dept.TeamType == "01")
            {
                var orderService = new WorkOrderService();
                var workSet = orderService.GetWorkOrderGroup(DeptId);
                if (workSet.Count() > 0)
                {
                    DeptStr = string.Join(",", workSet.Select(x => x.departmentid));
                }
                else
                {
                    DeptStr = DeptId;
                }

            }
            else
            {
                DeptStr = DeptId;
            }
            query = query.Where(x => DeptStr.Contains(x.DeptId));

            if (!queryParam["keyWord"].IsEmpty())
            {
                var keyWord = queryParam["keyWord"].ToString();
                query = query.Where(x => x.KeyCode.Contains(keyWord) || x.KeyPlace.Contains(keyWord));
            }
            if (!queryParam["Category"].IsEmpty())
            {
                var Category = queryParam["Category"].ToString();
                query = query.Where(x => x.Category == Category);
            }
            if (!queryParam["CategoryId"].IsEmpty())
            {
                var CategoryId = queryParam["CategoryId"].ToString();
                query = query.Where(x => x.CategoryId == CategoryId);
            }
            if (!queryParam["CreateDate"].IsEmpty())
            {
                var CreateDate = Convert.ToDateTime(queryParam["CreateDate"].ToString());
                var end = CreateDate.AddDays(1).AddMilliseconds(-1);
                query = query.Where(x => x.CreateDate >= CreateDate && CreateDate <= end);
            }
            if (!queryParam["State"].IsEmpty())
            {
                var State = queryParam["State"].ToString();
                if (State == "1")
                {
                    query = query.Where(x => !x.State);
                }
                if (State == "2")
                {
                    query = query.Where(x => x.State);
                }

            }
            pagination.records = query.Count();
            query = query.OrderByDescending(x => x.KeyCode).ThenByDescending(x=>x.CreateDate).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
            return query.ToList();
        }

        #endregion
        #region keyUse
        /// <summary>
        /// id获取数据
        /// </summary>
        /// <returns></returns>
        public KeyUseEntity getKeyUseDataById(string Id)
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = db.IQueryable<KeyUseEntity>(x => x.ID == Id).FirstOrDefault();
            return data;
        }


        /// <summary>
        /// KeyId获取数据
        /// </summary>
        /// <returns></returns>
        public List<KeyUseEntity> getKeyUseDataByKeyId(string Id)
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = db.IQueryable<KeyUseEntity>(x => x.KeyId == Id).ToList();
            return data;
        }

        /// <summary>
        /// 列表分页 正在借出的纪录
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public List<KeyUseEntity> GetPageKeyUseListByState(Pagination pagination, string queryJson)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = db.IQueryable<KeyBoxEntity>();
            var queryParam = queryJson.ToJObject();
            var DeptId = queryParam["DeptId"].ToString();
            var dept = db.FindEntity<DepartmentEntity>(DeptId);
            var DeptStr = DeptId;
            if (dept.TeamType == "01")
            {


                var orderService = new WorkOrderService();
                var workSet = orderService.GetWorkOrderGroup(DeptId);
                if (workSet.Count() > 0)
                {
                    DeptStr = string.Join(",", workSet.Select(x => x.departmentid));
                }
                else
                {
                    DeptStr = DeptId;
                }
            }
            else
            {
                DeptStr = DeptId;
            }
            query = query.Where(x => DeptStr.Contains(x.DeptId));
            if (!queryParam["Category"].IsEmpty())
            {
                var Category = queryParam["Category"].ToString();
                query = query.Where(x => x.Category == Category);
            }

            query = query.Where(x => x.State);
            pagination.records = query.Count();
            query = query.OrderByDescending(x => x.Category).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
            var data = from a in query
                       join b in (
                      from c in
                           db.IQueryable<KeyUseEntity>()
                      group c by c.KeyId into e
                      select e.OrderByDescending(x => x.LoanDate).FirstOrDefault()
                       )
                       on a.ID equals b.KeyId into tb
                       from tb1 in tb.DefaultIfEmpty()
                       select tb1;
            return data.ToList();
        }


        /// <summary>
        /// 列表分页 历史纪录
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public List<KeyUseEntity> GetPageKeyUseList(Pagination pagination, string queryJson)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = db.IQueryable<KeyUseEntity>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["Category"].IsEmpty())
            {
                var Category = queryParam["Category"].ToString();
                query = query.Where(x => x.Category == Category);
            }
            if (!queryParam["CategoryId"].IsEmpty())
            {
                var CategoryId = queryParam["CategoryId"].ToString();
                query = query.Where(x => x.CategoryId == CategoryId);
            }
            if (!queryParam["CreateDate"].IsEmpty())
            {
                var CreateDate = Convert.ToDateTime(queryParam["CreateDate"].ToString());
                var end = CreateDate.AddDays(1).AddMilliseconds(-1);
                query = query.Where(x => x.LoanDate >= CreateDate && x.LoanDate <= end);

            }

            if (!queryParam["keyWord"].IsEmpty())
            {
                var KeyWord = queryParam["keyWord"].ToString();
                query = query.Where(x => x.KeyPlace.Contains(KeyWord));
            }
            if (!queryParam["KeyId"].IsEmpty())
            {
                var KeyId = queryParam["KeyId"].ToString();
                query = query.Where(x => x.KeyId == KeyId);
            }
            var DeptId = queryParam["DeptId"].ToString();
            var dept = db.FindEntity<DepartmentEntity>(DeptId);
            var DeptStr = DeptId;
            if (dept.TeamType == "01")
            {


                var orderService = new WorkOrderService();
                var workSet = orderService.GetWorkOrderGroup(DeptId);
                if (workSet.Count() > 0)
                {
                    DeptStr = string.Join(",", workSet.Select(x => x.departmentid));
                }
                else
                {
                    DeptStr = DeptId;
                }

            }
            else
            {
                DeptStr = DeptId;
            }
            query = query.Where(x => DeptStr.Contains(x.DeptId));
            pagination.records = query.Count();
            query = query.OrderByDescending(x => x.LoanDate).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
            return query.ToList();
        }

        #endregion



        #endregion
        #region 操作数据

        #region keyBox
        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="entity"></param>
        public void operateKeyBox(List<KeyBoxEntity> entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                foreach (var item in entity)
                {
                    if (!string.IsNullOrEmpty(item.ID))
                    {
                        var dataEntity = db.FindEntity<KeyBoxEntity>(item.ID);
                        if (dataEntity != null)
                        {
                            dataEntity.KeyPlace = item.KeyPlace;
                            dataEntity.DeptCode = item.DeptCode;
                            dataEntity.DeptId = item.DeptId;
                            dataEntity.DeptName = item.DeptName;
                            db.Update(dataEntity);

                        }
                        else
                        {
                            db.Insert(item);
                        }
                    }
                    else
                    {
                        item.ID = Guid.NewGuid().ToString();
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
        public void removeKeyBox(string keyvalue)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var dataEntity = db.FindEntity<KeyBoxEntity>(keyvalue);
                if (dataEntity != null)
                {
                    db.Delete(dataEntity);
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

        #region keyBox and keyuse
        /// <summary>
        /// 归还钥匙
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <param name="userId"></param>
        public void ReturnKey(string keyvalue, string userId)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var keyStr = keyvalue.Split(',');
                foreach (var item in keyStr)
                {
                    var dataEntity = db.FindEntity<KeyUseEntity>(item);
                    if (dataEntity != null)
                    {
                        var user = db.FindEntity<UserEntity>(userId);
                        dataEntity.SendBackDate = DateTime.Now;
                        dataEntity.OperateUser = user.RealName;
                        dataEntity.OperateUserId = user.UserId;
                        db.Update(dataEntity);
                        var keyBox = db.FindEntity<KeyBoxEntity>(dataEntity.KeyId);
                        if (keyBox != null)
                        {
                            keyBox.State = false;
                            db.Update(keyBox);

                        }

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
        /// 借用钥匙
        /// </summary>
        public void BorrowKey(KeyUseEntity dataEntity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {

                if (dataEntity != null)
                {
                    dataEntity.ID = Guid.NewGuid().ToString();
                    db.Insert(dataEntity);
                    var keyBox = db.FindEntity<KeyBoxEntity>(dataEntity.KeyId);
                    if (keyBox != null)
                    {
                        keyBox.State = true;
                        db.Update(keyBox);

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

        #region keyuse
        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="entity"></param>
        public void operateKeyUse(KeyUseEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {

                if (!string.IsNullOrEmpty(entity.ID))
                {
                    var dataEntity = db.FindEntity<KeyUseEntity>(entity.ID);
                    if (dataEntity != null)
                    {

                        db.Update(dataEntity);
                    }
                    else
                    {
                        db.Insert(entity);
                    }
                }
                else
                {
                    entity.ID = Guid.NewGuid().ToString();
                    db.Insert(entity);
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
        public void removeKeyUse(string keyvalue)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var dataEntity = db.FindEntity<KeyUseEntity>(keyvalue);
                if (dataEntity != null)
                {
                    db.Delete(dataEntity);
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

        #endregion
    }
}
