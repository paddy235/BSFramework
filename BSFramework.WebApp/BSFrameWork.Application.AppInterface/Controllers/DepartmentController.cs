using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Util;
using BSFrameWork.Application.AppInterface.Models;
using Bst.Fx.Uploading;
using Bst.ServiceContract.MessageQueue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ThoughtWorks.QRCode.Codec;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class DepartmentController : ApiController
    {
        [HttpPost]
        public ModelBucket<object> GetSubTeams(ParamBucket<string> args)
        {
            var departmentbll = new DepartmentBLL();
            var dept = departmentbll.GetAuthorizationDepartment(args.Data);
            var data = departmentbll.GetSubTeams(dept.DepartmentId);
            return new ModelBucket<object>() { Data = data, Success = true };
        }

        /// <summary>
        /// 查询所有的班组
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetAllGroups()
        {
            try
            {
                var departmentbll = new DepartmentBLL();
                var data = departmentbll.GetAllGroups();
                return new { Code = 0, Info = "查询成功", Data = data };
            }
            catch (Exception ex)
            {
                return new { Code = -1, Info = "查询失败", Data = ex.Message };
            }
           
        }

        [HttpPost]
        public object GetGroupsByUser()
        {
            try
            {
                string res = HttpContext.Current.Request["json"];
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string userId = dy.userId;
                if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException("userId不能为空");
                List<DepartmentEntity> deptList = new DepartmentBLL().GetGroupsByUser(userId);
                return new { Code = 0, Info = "查询成功", data = deptList };
            }
            catch (Exception ex)
            {
                return new { Code = -1, Info = "查询失败", data = ex.Message };
            }
        }
    }
}