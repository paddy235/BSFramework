using BSFramework.Application.Entity.WorkPlan;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bst.Bzzd.DataSource;
using Bst.Bzzd.DataSource.Entities;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.IService.YearWorkPlanManage;
using BSFramework.Application.Entity.YearWorkPlan;
using System.Data;
using BSFramework.Util.WebControl;
using BSFramework.Util;
using BSFramework.Data;
using BSFramework.Util.Extension;

namespace BSFramework.Application.Service.YearWorkPlanManage
{
    /// <summary>
    /// 
    /// </summary>
    public class YearWorkPlanService : RepositoryFactory<YearWorkPlanEntity>, IYearWorkPlanService
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public List<YearWorkPlanEntity> GetPlanList()
        {
            var db = new RepositoryFactory<YearWorkPlanEntity>().BaseRepository();
            var query = db.IQueryable();
            return query.ToList();
        }

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public DataTable GetPageList(Pagination pagination, string queryJson)
        {
            //Pagination pagination = new Pagination();
            //pagination.p_kid = "id";
            //pagination.p_fields = @"plan,remark,planstart,planend,planfinish,lastprogress,progress,bookmark,deptname,deptid,deptcode,CREATEUSERID,CREATEUSERNAME,MODIFYDATE,MODIFYUSERID,MODIFYUSERNAME,CREATEDATE";
            //pagination.p_tablename = @"(select id,plan,remark,planstart,planend,planfinish,lastprogress,progress,bookmark,deptname,deptid,deptcode,CREATEUSERID,CREATEUSERNAME,MODIFYDATE,MODIFYUSERID,MODIFYUSERNAME,CREATEDATE ";
            //pagination.p_tablename += "  from  (select id,plan,remark,planstart,planend,planfinish,lastprogress,progress,bookmark,deptname,deptid,deptcode,CREATEUSERID,CREATEUSERNAME,MODIFYDATE,MODIFYUSERID,MODIFYUSERNAME,CREATEDATE  from wg_yearworkplan  ";
            //pagination.sidx = "CREATEDATE";
            //pagination.sord = "desc";
            //pagination.conditionJson = "1=1";
            //var user = new UserBLL().GetEntity(dy.userId);
            //pagination.p_tablename += string.Format(" where deptcode like '{0}%'", user.DepartmentCode);
            //pagination.p_tablename += " ORDER BY CREATEDATE DESC) t GROUP BY bookmark ) as t ";
            IRepository db = new RepositoryFactory().BaseRepository();
            var queryParam = queryJson.ToJObject();
            var deptcode = queryParam["deptCode"].ToString();
            var query =
                from a in db.IQueryable<YearWorkPlanEntity>()
                where a.deptcode.StartsWith(deptcode)
                orderby a.CREATEDATE descending
                group a by a.bookmark into c
                select c.FirstOrDefault();

            query = query.OrderByDescending(x => x.CREATEDATE).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
            var dt = DataHelper.ConvertToTable(query);
            //DataTable dt = db.FindTableByProcPager(pagination, DbHelper.DbType);
            return dt;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public void SaveFormList(List<YearWorkPlanEntity> entity)
        {
            var db = new RepositoryFactory<YearWorkPlanEntity>().BaseRepository().BeginTrans();
            try
            {
                foreach (var item in entity)
                {
                    item.historyEntity = null;
                    if (string.IsNullOrEmpty(item.id))
                    {
                        item.id = Guid.NewGuid().ToString();
                        db.Insert(item);
                    }
                    else
                    {

                        db.Update(item);
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
        /// 数据操作
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        public void SaveForm(string keyValue, YearWorkPlanEntity entity)
        {
            var db = new RepositoryFactory<YearWorkPlanEntity>().BaseRepository();

            try
            {
                if (string.IsNullOrEmpty(keyValue))
                {
                    entity.id = Guid.NewGuid().ToString();
                    entity.bookmark = Guid.NewGuid().ToString();
                    db.Insert(entity);
                }
                else
                {
                    db.Update(entity);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public YearWorkPlanEntity GetEntity(string keyValue)
        {

            var data = this.BaseRepository().FindEntity(keyValue);
            data.historyEntity = new List<YearWorkPlanEntity>();
            var datalist = this.BaseRepository().IQueryable(x => x.bookmark == data.bookmark);
            foreach (var item in datalist)
            {
                if (item.id != data.id)
                {
                    data.historyEntity.Add(item);
                }
            }
            if (data.historyEntity.Count > 0)
            {
                data.historyEntity = data.historyEntity.OrderByDescending(x => x.CREATEDATE).ToList();

            }
            return data;
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue"></param>
        public void RemoveForm(string keyValue)
        {
            var data = this.BaseRepository().FindEntity(keyValue);
            var datalist = this.BaseRepository().IQueryable(x => x.bookmark == data.bookmark).ToList();
            this.BaseRepository().Delete(datalist);

        }

    }
}
