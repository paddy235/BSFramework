using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.IService.SystemManage;
using BSFramework.Application.Service.SystemManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;

namespace BSFramework.Application.Busines.SystemManage
{
    /// <summary>
    /// 描 述：系统日志
    /// </summary>
    public static class LogBLL
    {
        private static ILogService service = new LogService();

        #region 获取数据
        /// <summary>
        /// 日志列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public static IEnumerable<LogEntity> GetPageList(Pagination pagination, string queryJson)
        {
            return service.GetPageList(pagination, queryJson);
        }
        /// <summary>
        /// 日志实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public static LogEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 清空日志
        /// </summary>
        /// <param name="categoryId">日志分类Id</param>
        /// <param name="keepTime">保留时间段内</param>
        public static void RemoveLog(int categoryId, string keepTime)
        {
            try
            {
                service.RemoveLog(categoryId, keepTime);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<LogEntity> GetPageList(int? categoryid, DateTime starttime, DateTime endtime, string operateaccount, string ipaddress, string operatetype, string module, int rows, int page, out int total)
        {
            return service.GetPageList(categoryid, starttime, endtime, operateaccount, ipaddress, operatetype, module, rows, page, out total);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logEntity">对象</param>
        public static void WriteLog(this LogEntity logEntity)
        {
            try
            {
                service.WriteLog(logEntity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
