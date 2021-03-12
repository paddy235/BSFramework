using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Busines.WorkPlanManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.WorkPlan;
using BSFramework.Util;
using BSFramework.Util.Log;
using BSFrameWork.Application.AppInterface.Models;
using BSFrameWork.Application.AppInterface.Models.QueryModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class WorkPlanController : ApiController
    {
        WorkPlanBLL bll = new WorkPlanBLL();
        DepartmentBLL deptbll = new DepartmentBLL();
        UserBLL userbll = new UserBLL();
        private WorkOrderBLL _workOrderBll;
        private WorkSettingBLL _workSettingBll;
        public WorkPlanController() {
            _workOrderBll = new WorkOrderBLL();
            _workSettingBll = new WorkSettingBLL();
        }
        /// <summary>
        /// 获取工作内容列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetPlanContentList(WorkPlanContentListModel model)
        {
            try
            {
                string userId = model.userId;
                UserEntity user = userbll.GetEntity(userId);
                DepartmentEntity dept = deptbll.GetEntity(user.DepartmentId);
                string plantype = model.data.PlanType;
                var now = DateTime.Now.Date;
                var from = DateTime.Now.AddYears(-100);
                var to = DateTime.Now.Date.AddDays(1);
                if (plantype == "周工作计划")
                {
                    int week = (int)DateTime.Now.DayOfWeek;
                    if (week == 0) week = 7;
                    from = now.AddDays(0 - week + 1);
                    to = from.AddDays(7);
                }
                else if (plantype == "月工作计划")
                {
                    from = new DateTime(now.Year, now.Month, 1);
                    to = new DateTime(now.Year, now.Month, 1).AddMonths(1);
                }
                var list = bll.GetPlanList().Where(x => x.UseDeptId.Contains(user.DepartmentId) && ((x.StartDate >= from && x.StartDate < to) || (x.EndDate >= from && x.EndDate < to)));
                if (!string.IsNullOrEmpty(plantype))
                {
                    list = list.Where(x => x.PlanType == plantype);
                }
                var ids = list.Select(x => x.ID);
                var clist = bll.GetContentList("").Where(x => ids.Contains(x.PlanId)).OrderByDescending(x => x.CreateDate).ToList();
                var plan = new WorkPlanEntity();
                foreach (WorkPlanContentEntity w in clist)
                {
                    w.WorkContent = w.WorkContent == null ? "" : w.WorkContent;
                    w.Remark = w.Remark == null ? "" : w.Remark;
                    w.ChildrenContent = bll.GetContentList("").Where(x => x.ParentId == w.ID && x.BZID == user.DepartmentId).ToList();
                    plan = bll.GetWorkPlanEntity(w.PlanId);
                    if (plan != null) { w.StartDate = plan.StartDate; w.EndDate = plan.EndDate; }
                }
                return new { code = 0, info = "成功", count = clist.Count(), data = clist };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 获取工作内容列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetPlanContentListTotal(WorkPlanContentListModel model)
        {
            try
            {
                string userId = model.userId;
                UserEntity user = userbll.GetEntity(userId);
                DepartmentEntity dept = deptbll.GetEntity(user.DepartmentId);
                string plantype = model.data.PlanType;
                var now = DateTime.Now.Date;
                var from = DateTime.Now.AddYears(-100);
                var to = DateTime.Now.Date.AddDays(1);
                if (plantype == "周工作计划")
                {
                    int week = (int)DateTime.Now.DayOfWeek;
                    if (week == 0) week = 7;
                    from = now.AddDays(0 - week + 1);
                    to = from.AddDays(7);
                }
                else if (plantype == "月工作计划")
                {
                    from = new DateTime(now.Year, now.Month, 1);
                    to = new DateTime(now.Year, now.Month, 1).AddMonths(1);
                }
                var list = bll.GetPlanList().Where(x => x.UseDeptId.Contains(user.DepartmentId) && ((x.StartDate >= from && x.StartDate < to) || (x.EndDate >= from && x.EndDate < to)));
                if (!string.IsNullOrEmpty(plantype))
                {
                    list = list.Where(x => x.PlanType == plantype);
                }
                if (list.Count()==0)
                {
                    return new { code = 0, info = "成功", data = new { finish =0, unfinish = 0, score = 0 } };

                }
                var finish = list.Where(x => x.IsFinished == "已完成").ToList();
                var unfinish = list.Where(x => x.IsFinished == "未完成").ToList();
                decimal score = (decimal)(finish.Count() / list.Count());
                return new { code = 0, info = "成功",  data = new { finish = finish.Count , unfinish = unfinish.Count, score = score } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }


        /// <summary>
        /// 获取工作计划列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetPlanList(WorkPlanListModel model)
        {
            try
            {
                string userId = model.userId;
                UserEntity user = userbll.GetEntity(userId);
                DepartmentEntity dept = deptbll.GetEntity(user.DepartmentId);
                string plantype = model.data.PlanType;
                var now = DateTime.Now.Date;

                var list = bll.GetPlanList().Where(x => x.UseDeptId.Contains(user.DepartmentId));
                if (!string.IsNullOrEmpty(model.data.StartDate))
                {
                    var from = Convert.ToDateTime(model.data.StartDate);
                    list = list.Where(x => x.StartDate >= from || x.EndDate >= from);
                }
                if (!string.IsNullOrEmpty(model.data.EndDate))
                {
                    var to = Convert.ToDateTime(model.data.EndDate);
                    list = list.Where(x => x.StartDate < to || x.EndDate < to);
                }
                if (!string.IsNullOrEmpty(plantype))
                {
                    list = list.Where(x => x.PlanType == plantype);
                }
                var ids = list.Select(x => x.ID);
                var clist = bll.GetContentList("").Where(x => ids.Contains(x.PlanId)).OrderByDescending(x => x.CreateDate).ToList();
                var plan = new WorkPlanEntity();
                foreach (WorkPlanContentEntity w in clist)
                {
                    w.WorkContent = w.WorkContent == null ? "" : w.WorkContent;
                    w.Remark = w.Remark == null ? "" : w.Remark;
                    w.ChildrenContent = bll.GetContentList("").Where(x => x.ParentId == w.ID && x.BZID == user.DepartmentId).ToList();
                    plan = bll.GetWorkPlanEntity(w.PlanId);
                    w.PlanType = plan.PlanType;
                    if (plan != null) { w.StartDate = plan.StartDate; w.EndDate = plan.EndDate; }
                }
                return new { code = 0, info = "成功", count = list.Count(), data = clist };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 保存工作任务（完成任务或新增子任务）
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object SavePlanContent(PlanContentModel model)
        {
            try
            {
                string userId = model.userId;
                UserEntity user = userbll.GetEntity(userId);
                DepartmentEntity dept = deptbll.GetEntity(user.DepartmentId);

                foreach (WorkPlanContentEntity w in model.data.ChildrenContent)
                {
                    if (string.IsNullOrEmpty(w.ID))
                    {
                        w.ID = Guid.NewGuid().ToString();
                        w.CreateUser = user.RealName;
                        w.CreateDate = DateTime.Now;
                        w.CreateUserId = userId;
                    }
                    w.StartDate = Convert.ToDateTime(w.Start);
                    w.EndDate = Convert.ToDateTime(w.End);
                    w.BZID = user.DepartmentId;
                    bll.SaveWorkPlanContent(w.ID, w);
                }

                WorkPlanContentEntity entity = model.data;
                bll.SaveWorkPlanContent(entity.ID, entity);
                //更新工作计划状态
                var plans = bll.GetContentList(entity.PlanId).Where(x => x.IsFinished == "未完成");
                var plan = bll.GetWorkPlanEntity(entity.PlanId);
                if (plans.Count() == 0)
                {
                    plan.IsFinished = "已完成";
                    bll.SaveWorkPlan(plan.ID, plan);
                }
                return new { code = 0, info = "成功" };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        [HttpPost]
        public object Delete(DelModelNew model)
        {
            try
            {
                bll.RemoveWorkPlanContent(model.Id);
                var list = bll.GetContentList("").Where(x => x.ParentId == model.Id);
                foreach (WorkPlanContentEntity w in list) 
                {
                    bll.RemoveWorkPlanContent(w.ID);
                }
                return new { code = 0, info = "成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        #region 排班设置
        /// <summary>
        /// 获取班组某月的排班数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public ModelBucket<object> GetPaiBanList(ParamBucket<MeetingQuery> query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query.Data.KeyWord)) throw new ArgumentNullException("KeyWord", "班组Id不能为空");
                if (string.IsNullOrWhiteSpace(query.Data.StartTime)) throw new ArgumentNullException("StartTime", "时间不能空");
                DateTime startDate;
                if(!DateTime.TryParse(query.Data.StartTime,out startDate)) throw new Exception("StartTime无法转换成有效的时间类型");
                var data = _workOrderBll.GetEntity(startDate, query.Data.KeyWord);
                List<object> resultList = new List<object>();
                if (data != null)
                {
                    var settingList = data.timedata.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < settingList.Length; i++)
                    {
                        resultList.Add(new
                        {
                            Date = new DateTime(startDate.Year, startDate.Month, i + 1),
                            Value = settingList[i]
                        }); 
                    }
                }
                var result = new { OrderId = data.worktimesortid, DataList = resultList };

                return new ModelBucket<object>(0, "查询成功", result) { Success = true };
            }
            catch (Exception ex)
            {
                WriteLog.AddLog("GetPaiBanList\r接收参数：" + JsonConvert.SerializeObject(query) + "\r\n错误信息：" + JsonConvert.SerializeObject(ex), "Api_WorkPlan_Error");
                return new ModelBucket<object>(-1, "查询失败", null) { Message = ex.Message };
            }
        }

        /// <summary>
        /// 根据OrderId查询某班组排班设置
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public ModelBucket<object> GetWorkOrderSettings(ParamBucket<MeetingQuery> query)
        {
            try
            {
                var user = new UserBLL().GetEntity(query.UserId);
                var order = _workOrderBll.GetWorkOrderday(query.Data.KeyWord);
                var data = _workSettingBll.GetList(user.DepartmentId).Where(row => row.WorkSetupId == order).OrderBy(row => row.StartTime).ToList();
                var result = data.Select(x => new { Id = x.WorkSettingId, Value = x.Name }).ToList();
                return new ModelBucket<object>(0, "查询成功", result);
            }
            catch (Exception ex)
            {
                WriteLog.AddLog("GetWorkOrderSettings\r接收参数：" + JsonConvert.SerializeObject(query) + "\r\n错误信息：" + JsonConvert.SerializeObject(ex), "Api_WorkPlan_Error");
                return new ModelBucket<object>(-1, "查询失败", null) { Message = ex.Message };
            }
        }

        /// <summary>
        /// 设置班组某天的排班设置
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket SaveWorkSetting(ParamBucket<MeetingQuery> query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query.Data.KeyWord)) throw new ArgumentNullException("KeyWord", "SettingId不能为空");
                if (string.IsNullOrWhiteSpace(query.Data.Id)) throw new ArgumentNullException("Id", "OrderId不能为空");
                if (string.IsNullOrWhiteSpace(query.Data.StartTime)) throw new ArgumentNullException("StartTime", "时间不能空");
                DateTime startDate;
                if (!DateTime.TryParse(query.Data.StartTime, out startDate)) throw new Exception("StartTime无法转换成有效的时间类型");
                _workOrderBll.WorkSetSaveOneDay(startDate, query.Data.KeyWord, query.Data.Id);
                return new ResultBucket(0, "操作成功");
            }
            catch (Exception ex)
            {
                WriteLog.AddLog("SaveWorkSetting\r接收参数：" + JsonConvert.SerializeObject(query) + "\r\n错误信息：" + JsonConvert.SerializeObject(ex), "Api_WorkPlan_Error");
                return new ResultBucket(-1, "查询失败") { Message = ex.Message };
            }
        }
        /// <summary>
        /// 定时计划  部门排班数据生成
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object StartProduce(string ProduceType)
        {

            var result = 0;
            var message = string.Empty;
        
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                if (ProduceType != "定期排班生成")
                {
                    return new { code = result, info = message };
                }
                logger.Info("————————————部门排班数据生成————————————");
                var nowDate = DateTime.Now;
                var nextDate = new DateTime(nowDate.Year, 1, 1).AddYears(1);
                _workOrderBll.nextyear(nextDate);
                logger.Info("————————————部门排班数据生产完成————————————");
            }
            catch (Exception ex)
            {
                logger.Info("————————————部门排班数据生成错误————————————");
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


        #endregion

    }
}