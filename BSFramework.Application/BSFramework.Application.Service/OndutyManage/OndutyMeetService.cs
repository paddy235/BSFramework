using BSFramework.Application.Entity.OndutyManage;
using BSFramework.Application.IService.OndutyManage;
using BSFramework.Data;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.OndutyManage
{
    /// <summary>
    /// 
    /// </summary>
    public class OndutyMeetService : RepositoryFactory<OndutyMeetEntity>, IOndutyMeetService
    {

        /// <summary>
        /// 获取台账分页数据
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public DataTable GetPageList(Pagination pagination, string queryJson)
        {
            DatabaseType dataType = DbHelper.DbType;

            return this.BaseRepository().FindTableByProcPager(pagination, dataType);
        }
        /// <summary>
        /// 获取台账分页数据
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public List<OndutyMeetEntity> GetPagesList(Pagination pagination, string queryJson, string userid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var expression = LinqExtensions.True<OndutyMeetEntity>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["keyvalue"].IsEmpty())
            {
                var keyvalue = queryParam["keyvalue"].ToString();
                    expression = expression.And(x => x.otherid == keyvalue);
            }
            if (!queryParam["meettype"].IsEmpty())
            {
                var meettype = queryParam["meettype"].ToString();
                expression = expression.And(x => x.ondutyshift.Contains(meettype));
            }
            if (!queryParam["moduletype"].IsEmpty())
            {
                var moduletype = queryParam["moduletype"].ToString();

                expression = expression.And(x => x.ondutyshift.Contains(moduletype));
            }
            var data = db.IQueryable(expression).OrderByDescending(x => x.CreateDate);
            pagination.records = data.Count();
            return data.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList();

        }
        public List<OndutyMeetEntity> GetList(string other) {
            var list = this.BaseRepository().IQueryable(x => x.otherid==other).ToList();
            return list;

        }
        public List<OndutyMeetEntity> GetList(DateTime start, DateTime end, string userid,string deptid)
        {
            var list = this.BaseRepository().IQueryable(x => x.ondutytime >= start && x.ondutytime <= end);
            if (!string.IsNullOrEmpty(userid))
            {
                list = list.Where(x => x.ondutyuserid == userid);
            }
            if (!string.IsNullOrEmpty(deptid))
            {
                list = list.Where(x => x.ondutydeptid == deptid);
            }
            return list.ToList();
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public OndutyMeetEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue"></param>
        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        public void SaveForm(string keyValue, OndutyMeetEntity entity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
               
                this.BaseRepository().Update(entity);
            }
            else
            {
               
                this.BaseRepository().Insert(entity);
            }
        }

    }
}
