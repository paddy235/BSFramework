using BSFramework.Application.Busines.DeptCycleTaskManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BSFrameWork.Application.AppInterface.Models;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class DeptCycleTaskController : BaseApiController
    {

        /// <summary>
        /// 定时计划  部门定时任务推送
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object Start(string switchType)
        {

            var result = 0;
            var message = string.Empty;
            var task = new DeptCycleTaskBLL();
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                if (switchType != "部门定期任务库")
                {
                    return new { code = result, info = message };
                }
                logger.Info("————————————部门定期任务库数据生成————————————");
                task.GetDpetTask(DateTime.Now, "", "");
                logger.Info("————————————部门定期任务库数据完成————————————");
            }
            catch (Exception ex)
            {
                logger.Info("————————————部门定期任务库数据错误————————————");
                var exNext = ex;
                int i = 0;
                while (true)
                {
                    i++;
                    if (i == 5)
                    {
                        break;
                    }
                    if (exNext.InnerException == null)
                    {
                        logger.Info("Message:" + ex.Message);
                        break;
                    }
                    exNext = exNext.InnerException;
                }

                logger.Info("——————————————————————————————————————————");
                return new { code = 1, info = ex.Message };

            }
            return new { code = result, info = message };
        }
        /// <summary>
        ///获取数据
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object GetNowDayTask(BaseDataModel<DeptCycleTaskModel> dy)
        {

            var result = 0;
            var message = string.Empty;
            try
            {
                if (!dy.allowPaging)
                {
                    dy.pageSize = 10000;
                    dy.pageIndex = 1;
                }
                var task = new DeptWorkCycleTaskBLL();
                Pagination pagination = new Pagination();
                pagination.rows = dy.pageSize;
                pagination.page = dy.pageIndex;
                dy.data.app = "1";
                var queryJson = JsonConvert.SerializeObject(dy.data);
                var data = task.GetPageList(pagination, queryJson, dy.userId);
                return new { code = result, info = "操作成功", data = data };
            }
            catch (Exception ex)
            {

                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        ///获取数据
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object GetNowDayTaskDetail(BaseDataModel<string> dy)
        {

            var result = 0;
            var message = string.Empty;
            try
            {
                var task = new DeptWorkCycleTaskBLL();
                var queryJson = JsonConvert.SerializeObject(dy.data);
                var data = task.getEntity(dy.data);
                return new { code = result, info = "操作成功", data = data };
            }
            catch (Exception ex)
            {

                return new { code = 1, info = ex.Message };
            }

        }

    }
}
