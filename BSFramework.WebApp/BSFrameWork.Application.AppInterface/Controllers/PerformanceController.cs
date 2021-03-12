using BSFramework.Application.Busines.PerformanceManage;
using Newtonsoft.Json;
using System.Dynamic;
using System.Web;
using System.Web.Http;
using System;
using System.Net.Http;
using BSFrameWork.Application.AppInterface.Models;
using BSFramework.Application.Entity.PerformanceManage;
using System.Collections.Generic;
using BSFramework.Application.Busines.WebApp;
using System.Linq;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class PerformanceController : ApiController
    {
        /// <summary>
        /// 绩效管理配置
        /// </summary>
        private PerformancesetupBLL bll = new PerformancesetupBLL();
        private PerformanceBLL Socrebll = new PerformanceBLL();
        private PerformancetitleBLL Titlebll = new PerformancetitleBLL();
        private PerformanceupBLL upbll = new PerformanceupBLL();
        private UserWorkAllocationBLL getbll = new UserWorkAllocationBLL();

        /// <summary>
        /// 定时计划  推送未提交的班组
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object BaseDataInfo(string Performance)
        {

            var result = 0;
            var message = string.Empty;
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
            
                if (Performance != "绩效管理")
                {
                    return new { code = result, info = message };
                }
                var entity = getbll.GetSubDepartments("", "班组");
               
                logger.Info("————————————绩效管理管理数据生成————————————");
                bll.BaseDataOperation(entity);
                logger.Info("————————————绩效管理管理数据完成————————————");
            }
            catch (Exception ex)
            {
                logger.Info("————————————绩效管理管理数据错误————————————");
                var exNext = ex;
                int i = 0;
                while (true)
                {
                    i++;
                    if (i==5)
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
        ///绩效管理配置 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object PerformanceSetUp()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            try
            {
                var dy = JsonConvert.DeserializeObject<BaseDataModel<PerformanceSetUpModel>>(json);
                bll.operation(dy.data.time, dy.data.departmentid, dy.data.add, dy.data.del, dy.data.Listupdate);
                return new { info = "操作成功", code = result };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }


        }


        /// <summary>
        ///绩效管理实时数据
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object PerformanceGetData()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            try
            {
                var dy = JsonConvert.DeserializeObject<BaseDataModel<PerformanceSetUpModel>>(json);
                DateTime? time = new DateTime();
                if (!string.IsNullOrEmpty(dy.data.time))
                    time = Convert.ToDateTime(dy.data.time);
                else
                    time = null;

                bll.PerformanceGetData(dy.data.departmentid, time);
                return new { info = "操作成功", code = result };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }

        }
        /// <summary>
        /// 获取报表数据
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object PerformanceSelect()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            try
            {
                var dy = JsonConvert.DeserializeObject<BaseDataModel<PerformanceModel>>(json);
                var Scoredata = Socrebll.getScore(dy.data.time, dy.data.departmentid);
                Scoredata = Scoredata.OrderBy(x => x.planer).ThenBy(x => x.username).ToList();
                var Titledata = Titlebll.getTitle(dy.data.time, dy.data.departmentid);
                if (Titledata == null)
                {
                    object data = null;
                    return new
                    {
                        info = "操作成功",
                        code = result,
                        data = data
                    };
                }
                var Updata = upbll.getList(Titledata.titleid);

                return new
                {
                    info = "操作成功",
                    code = result,
                    data = new
                    {
                        title = Titledata,
                        score = Scoredata,
                        up = Updata.isup
                    }
                };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }

        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object AllTitle()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            try
            {
                var dy = JsonConvert.DeserializeObject<BaseDataModel<PerformanceModel>>(json);
                var data = bll.AllTitle(dy.data.departmentid);
                return new { info = "操作成功", code = result, data = data };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1, data = new List<PerformancesetupEntity>() };
            }


        }
        /// <summary>
        /// 修改数报表数据
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object PerformanceOperation()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            try
            {
                var dy = JsonConvert.DeserializeObject<BaseDataModel<List<PerformanceEntity>>>(json);
                Socrebll.operation(dy.data);
                return new { info = "操作成功", code = result };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }

        }


        /// <summary>
        /// 个人统计
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object PerformanceByuser()
        {

            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            try
            {
                var dy = JsonConvert.DeserializeObject<BaseDataModel<PerformanceModel>>(json);
                var data = bll.PerformanceByuser(dy.userId, dy.data.time, dy.data.departmentid);
                return data;

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }

        }


        /// <summary>
        /// 修改上传
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object PerformanceupOperation()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            try
            {
                var dy = JsonConvert.DeserializeObject<BaseDataModel<string>>(json);
                upbll.operation(dy.data);
                return new { info = "操作成功", code = result };
            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }
        }
        /// <summary>
        /// 月度统计
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object PerformanceMonth()
        {

            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            try
            {
                var dy = JsonConvert.DeserializeObject<BaseDataModel<PerformanceModel>>(json);
                var data = Socrebll.getScoreList(dy.data.time, dy.data.departmentid);
                return data;
            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }
        }
        /// <summary>
        /// 个人绩效
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object PerformanceUser()
        {


            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            try
            {
                var dy = JsonConvert.DeserializeObject<BaseDataModel<PerformanceModel>>(json);
                var data = Socrebll.getScore(dy.userId, dy.data.time, dy.data.departmentid);
                return data;
            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }
        }
    }
}
