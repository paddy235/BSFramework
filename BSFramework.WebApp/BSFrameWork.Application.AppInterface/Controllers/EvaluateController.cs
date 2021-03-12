using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.InnovationManage;
using BSFramework.Application.Busines.WebApp;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.InnovationManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SevenSManage;
//using BSFramework.Application.Web.Areas.Works.Models;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using BSFrameWork.Application.AppInterface.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class EvaluateController : ApiController
    {
        private ActivityBLL bll = new ActivityBLL();
        private DepartmentBLL dtbll = new DepartmentBLL();
        private UserBLL ubll = new UserBLL();
        /// <summary>
        ///提交评价
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object PostEvaluate(BaseDataModel<ActivityEvaluateEntity> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                if (dy.data == null)
                {
                    return new { code = 1, info = "数据错误" };
                }
                if (string.IsNullOrEmpty(dy.data.Activityid))
                {
                    return new { code = 1, info = "请传入活动id" };
                }
                if (string.IsNullOrEmpty(dy.data.ActivityEvaluateId))
                {
                    dy.data.ActivityEvaluateId = Guid.NewGuid().ToString();
                    UserEntity user = ubll.GetEntity(dy.userId);
                    dy.data.Nature = dtbll.GetEntity(user.DepartmentId).Nature;
                }
                bll.SaveEvaluate(dy.data.Activityid, null, dy.data);
                return new { code = result, info = "操作成功" };
            }
            catch (Exception ex)
            {

                return new { code = 1, info = ex.Message };
            }
        }
        /// <summary>
        ///获取评价
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object getEvaluate(BaseDataModel<string> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                int total = 0;
                if (dy.allowPaging)
                {
                    var data = bll.GetActivityEvaluateEntity(dy.data, dy.pageSize, dy.pageIndex, out total);
                    return new { code = result, info = "操作成功", data = data };
                }
                else
                {
                    var data = bll.GetActivityEvaluateEntity(dy.data, 100, 1, out total);
                    return new { code = result, info = "操作成功", data = data };

                }
            }
            catch (Exception ex)
            {

                return new { code = 1, info = ex.Message };
            }
        }
    }
}