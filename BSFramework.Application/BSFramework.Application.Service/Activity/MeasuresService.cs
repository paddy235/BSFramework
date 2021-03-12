using BSFramework.Application.Entity.Activity;
using BSFramework.Application.IService.Activity;
using BSFramework.Data.Repository;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Data;
namespace BSFramework.Application.Service.Activity
{
    /// <summary>
    /// 描 述：安全预知训练
    /// </summary>
    public class MeasuresService : RepositoryFactory<MeasuresEntity>, MeasuresIService
    {
        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public IEnumerable<MeasuresEntity> GetList(string queryJson)
        {
            return this.BaseRepository().IQueryable().ToList();
        }
        /// <summary>
        /// 根据危险预知训练Id获取数据列表
        /// </summary>
        /// <param name="dangerId">关联危险预知训练记录Id</param>
        /// <returns></returns>
        public List<MeasuresEntity> GetMeasureList(string dangerId)
        {
            //DataTable dt = this.BaseRepository().FindTable(string.Format("select id,dangersource,measure,dutyman,userid,isover from wg_measures where dangerid='{0}' ", dangerId));

            var data = this.BaseRepository().IQueryable().Where(x => x.DangerId == dangerId).OrderBy(x => x.CreateDate).ToList();
            return data;
        }

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        /// 未使用 
        public DataTable GetPageList(Pagination pagination, string queryJson)
        {
            return new DataTable();
            //var queryParam = queryJson.ToJObject();

            //if (!queryParam["keyword"].IsEmpty())
            //{
            //    string keyword = queryParam["keyword"].ToString().Trim();
            //    pagination.conditionJson += string.Format(" and (subject like '%{0}%' or fullname like '%{0}%' or activityplace like '%{0}%')", keyword);
            //}
            ////查询条件
            //if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
            //{
            //    string condition = queryParam["condition"].ToString();
            //    string keyord = queryParam["keyword"].ToString();
            //    switch (condition)
            //    {
            //        case "Account":            //账户
            //            pagination.conditionJson += string.Format(" and ACCOUNT  like '%{0}%'", keyord);
            //            break;
            //        case "RealName":          //姓名
            //            pagination.conditionJson += string.Format(" and REALNAME  like '%{0}%'", keyord);
            //            break;
            //        case "Mobile":          //手机
            //            pagination.conditionJson += string.Format(" and MOBILE like '%{0}%'", keyord);
            //            break;
            //        default:
            //            break;
            //    }
            //}
            //DataTable dt = this.BaseRepository().FindTableByProcPager(pagination, DbHelper.DbType);
            //return dt;
        }

        public DataTable GetMeasuresById(Pagination pagination)
        {
            DataTable dt = this.BaseRepository().FindTableByProcPager(pagination, DbHelper.DbType);
            return dt;
        }

        public List<MeasuresEntity> GetMeasuresByIds(Pagination pagination,string dangerid)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from a in db.IQueryable<MeasuresEntity>()
                        where a.DangerId == dangerid
                        select a;
            pagination.records = query.Count();
            var data = query.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList();
          
            return data;
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public MeasuresEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
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
        public void SaveForm(string keyValue, MeasuresEntity entity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                entity.Modify(keyValue);
                this.BaseRepository().Update(entity);
            }
            else
            {
                entity.Create();
                this.BaseRepository().Insert(entity);
            }
        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void Save(MeasuresEntity entity)
        {
            entity.Create();
            this.BaseRepository().Insert(entity);
        }
        #endregion
    }
}
