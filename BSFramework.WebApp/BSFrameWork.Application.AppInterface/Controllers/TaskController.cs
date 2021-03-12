using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Busines.WorkPlanManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.WorkPlan;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util;
using BSFrameWork.Application.AppInterface.Models;
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
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class TaskController : ApiController
    {
        /// <summary>
        /// 任务管理
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<MeetingJobEntity> GetListByDeptId(ParamBucket<string> arg)
        {
            var bll = new TaskBLL();
            var total = 0;
            var data = bll.GetListByDeptId(arg.Data, arg.PageSize, arg.PageIndex, out total);
            foreach (var item in data)
            {
                if (string.IsNullOrEmpty(item.Relation.StartMeetingId))
                {
                    if (item.EndTime < DateTime.Now) item.Status = "已超期";
                    else item.Status = "未开始";
                }
                else
                {
                    if (item.Relation.IsFinished == "finish") item.Status = "已完成";
                    else
                    {
                        if (item.Relation.EndMeeting == null)
                            item.Status = "进行中";
                        else
                        {
                            if (item.Relation.EndMeeting.IsOver) item.Status = "未完成";
                            else item.Status = "进行中";
                        }
                    }
                }
            }
            return new ListBucket<MeetingJobEntity>() { Data = data, Total = total };
        }

        /// <summary>
        /// 任务管理/删除
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket Delete(ParamBucket<string> arg)
        {
            var result = 0;
            var msg = string.Empty;
            var bll = new TaskBLL();
            try
            {
                bll.Delete(arg.Data);
            }
            catch (Exception e)
            {
                result = 1;
                msg = e.Message;
            }
            return new ResultBucket() { code = result, info = msg };
        }
    }
}