using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.IService.SystemManage;
using BSFramework.Data;
using BSFramework.Data.EF;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSFramework.Application.Service.SystemManage
{
    /// <summary>
    /// 描 述：系统日志
    /// </summary>
    public class LogService : RepositoryFactory<LogEntity>, ILogService
    {
        private System.Data.Entity.DbContext _context;

        public LogService()
        {
            _context = (DbFactory.Base() as Database).dbcontext;
        }


        #region 获取数据
        /// <summary>
        /// 日志列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<LogEntity> GetPageList(Pagination pagination, string queryJson)
        {
            DatabaseType dataType = DbHelper.DbType;
            var queryParam = queryJson.ToJObject();
            //日志分类
            if (!queryParam["Category"].IsEmpty())
            {
                int categoryId = queryParam["CategoryId"].ToInt();
                pagination.conditionJson += string.Format(" and CategoryId ='{0}'", categoryId);
            }
            //操作时间
            if (!queryParam["StartTime"].IsEmpty() && !queryParam["EndTime"].IsEmpty())
            {
                DateTime startTime = queryParam["StartTime"].ToDate();
                DateTime endTime = queryParam["EndTime"].ToDate().AddDays(1);
                if (dataType == DatabaseType.Oracle)
                {
                    pagination.conditionJson += string.Format(" and to_char(OperateTime,'yyyymmdd')>='{0}' and   to_char(OperateTime,'yyyymmdd')<='{1}' ", startTime.ToString("yyyyMMdd"), endTime.ToString("yyyyMMdd"));
                }
                else if (dataType == DatabaseType.SqlServer)
                {
                    pagination.conditionJson += string.Format(" and OperateTime >= '{0}' and  OperateTime <=  '{1}'", startTime, endTime);
                }
            }
            //操作用户Id
            if (!queryParam["OperateUserId"].IsEmpty())
            {
                string OperateUserId = queryParam["OperateUserId"].ToString();
                pagination.conditionJson += string.Format(" and OperateUserId = '{0}'", OperateUserId);
            }
            //操作用户账户
            if (!queryParam["OperateAccount"].IsEmpty())
            {
                string OperateAccount = queryParam["OperateAccount"].ToString();
                pagination.conditionJson += string.Format(" and OperateAccount = '{0}'", OperateAccount);
            }
            //操作类型
            if (!queryParam["OperateType"].IsEmpty())
            {
                string operateType = queryParam["OperateType"].ToString();
                pagination.conditionJson += string.Format(" and OperateType = '{0}'", operateType);
            }
            //功能模块
            if (!queryParam["Module"].IsEmpty())
            {
                string module = queryParam["Module"].ToString();
                pagination.conditionJson += string.Format(" and Module like '%{0}%'", module);
            }

            return this.BaseRepository().FindListByProcPager(pagination, dataType);
        }
        /// <summary>
        /// 日志实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public LogEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 清空日志
        /// </summary>
        /// <param name="categoryId">日志分类Id</param>
        /// <param name="keepTime">保留时间段内</param>
        public void RemoveLog(int categoryId, string keepTime)
        {
            DateTime operateTime = DateTime.Now;
            if (keepTime == "7")//保留近一周
            {
                operateTime = DateTime.Now.AddDays(-7);
            }
            else if (keepTime == "1")//保留近一个月
            {
                operateTime = DateTime.Now.AddMonths(-1);
            }
            else if (keepTime == "3")//保留近三个月
            {
                operateTime = DateTime.Now.AddMonths(-3);
            }
            var expression = LinqExtensions.True<LogEntity>();
            expression = expression.And(t => t.OperateTime <= operateTime);
            expression = expression.And(t => t.CategoryId == categoryId);
            this.BaseRepository().Delete(expression);
        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logEntity">对象</param>
        public void WriteLog(LogEntity logEntity)
        {
            logEntity.LogId = Guid.NewGuid().ToString();
            logEntity.OperateTime = DateTime.Now;
            logEntity.DeleteMark = 0;
            logEntity.EnabledMark = 1;
            logEntity.IPAddress = Net.Ip;
            logEntity.Host = Net.Host;
            logEntity.Browser = Net.Browser;
            this.BaseRepository().Insert(logEntity);
        }

        public List<LogEntity> GetPageList(int? categoryid, DateTime starttime, DateTime endtime, string operateaccount, string ipaddress, string operatetype, string module, int rows, int page, out int total)
        {
            var query = _context.Set<LogEntity>().AsNoTracking().AsQueryable();

            if (categoryid != null) query = query.Where(x => x.CategoryId == categoryid);
            if (!string.IsNullOrEmpty(operateaccount)) query = query.Where(x => x.OperateAccount == operateaccount);
            if (!string.IsNullOrEmpty(ipaddress)) query = query.Where(x => x.IPAddress == ipaddress);
            if (!string.IsNullOrEmpty(operatetype)) query = query.Where(x => x.OperateType == operatetype);
            if (!string.IsNullOrEmpty(module)) query = query.Where(x => x.Module == module);

            total = query.Count();
            return query.OrderBy(x => x.OperateTime).Skip(rows * (page - 1)).Take(rows).ToList();
        }
        #endregion
    }
}
