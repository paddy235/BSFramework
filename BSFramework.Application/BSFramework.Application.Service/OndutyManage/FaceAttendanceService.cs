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
    public class FaceAttendanceService : RepositoryFactory<FaceAttendanceEntity>, IFaceAttendanceService
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
        public List<FaceAttendanceEntity> GetPagesList(Pagination pagination, string queryJson,string userid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var expression = LinqExtensions.True<FaceAttendanceEntity>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["type"].IsEmpty())
            {
                var type = queryParam["type"].ToString();
                if (type=="0")
                {
                    expression = expression.And(x => x.ondutyuserid == userid);

                }
            }
            if (!queryParam["start"].IsEmpty())
            {
                var start = Convert.ToDateTime(queryParam["start"]);
                expression = expression.And(x => x.ondutytime >= start);
            }
            if (!queryParam["end"].IsEmpty())
            {
                var end = Convert.ToDateTime(queryParam["end"]);
                expression = expression.And(x => x.ondutytime <= end);
            }
            var data = db.IQueryable(expression).OrderByDescending(x => x.CreateDate);
            pagination.records = data.Count();
            return data.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList();

        }
        public List<FaceAttendanceEntity> GetList(DateTime start, DateTime end, string userid,string deptid)
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
        public FaceAttendanceEntity GetEntity(string keyValue)
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
        public void SaveForm(string keyValue, FaceAttendanceEntity entity)
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

        public void SaveFormTime(string keyValue, FaceAttendanceTimeEntity entity) {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                if (!string.IsNullOrEmpty(keyValue))
                {

                    db.Update(entity);
                }
                else
                {

                    db.Insert(entity);
                }
                db.Commit();

            }
            catch (Exception)
            {

                db.Rollback();
            }
        }

    }
}
