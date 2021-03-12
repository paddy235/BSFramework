using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.WebApp;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.WebApp;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using ThoughtWorks.QRCode.Codec;

namespace BSFrameWork.Application.AppInterface
{
    public class UserExperiencController : ApiController
    {

        private UserExperiencBLL cbll = new UserExperiencBLL();
        /// <summary>
        ///获取人员工作经历列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object SelectByUserId()
        {
            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");

            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
            string userId = dy.userId;
            try
            {
                var entity = cbll.SelectByUserId(userId).OrderByDescending(x => x.createtime);
                return new { info = "查询成功", code = result, data = entity };

            }
            catch (Exception ex)
            {

                return new { info = "查询失败：" + ex.Message, code = 1, data = new List<UserExperiencEntity>() };

            }
        }

        /// <summary>
        ///获取人员工作经历详情
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object SelectDetail()
        {
            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");

            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
            string userId = dy.userId;
            string Id = dy.data;
            try
            {
                var entity = cbll.SelectDetail(Id);
                return new { info = "查询成功", code = result, data = entity };

            }
            catch (Exception ex)
            {

                return new { info = "查询失败：" + ex.Message, code = 1, data = new UserExperiencEntity() };

            }
        }

        /// <summary>
        ///修改人员工作经历
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object DeleteUserExperienc()
        {

            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");

            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
            string userId = dy.userId;
            string Id = dy.data;
            try
            {
                cbll.delete(Id);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }

        /// <summary>
        ///修改人员工作经历
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object UpdateUserExperienc()
        {

            var result = 0;
            var message = string.Empty;

            try
            {
                string res = HttpContext.Current.Request.Form["json"];
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                var entity = JsonConvert.DeserializeObject<UserExperiencEntity>(json["data"].ToString());
                UserEntity user = new UserBLL().GetEntity(userId);
                var get = cbll.SelectDetail(entity.ExperiencId);
                entity.createuserid = get.createuserid;
                entity.createtime = get.createtime;
                entity.createuser = get.createuser;
                entity.modifyuserid = userId;
                entity.modifytime = DateTime.Now;
                entity.modifyuser = user.RealName;
                cbll.update(entity);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }

        /// <summary>
        ///新增人员工作经历
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object addUserExperienc()
        {

            var result = 0;
            var message = string.Empty;

            try
            {
                string res = HttpContext.Current.Request.Form["json"];
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                var entity = JsonConvert.DeserializeObject<UserExperiencEntity>(json["data"].ToString());
                UserEntity user = new UserBLL().GetEntity(userId);
                if (user == null)
                {
                    entity.createuserid = userId;
                    entity.createtime = DateTime.Now;
                }
                else
                {
                    entity.createuserid = userId;
                    entity.createtime = DateTime.Now;
                    entity.createuser = user.RealName;
                }

                cbll.add(entity);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }


    }
}