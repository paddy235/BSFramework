using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.WebApp;
using BSFramework.Application.Entity.WebApp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class UserWorkStateController : ApiController
    {

        private UserWorkStateBLL bll = new UserWorkStateBLL();
        /// <summary>
        ///获取
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object SelectWorkState()
        {
            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");

            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
            string userId = dy.userId;
            try
            {
                var entity = bll.selectState(userId);
                if (entity == null)
                {
                    return new
                    {
                        info = "查询成功",
                        code = result,
                        data = new UserWorkStateEntity()
                    };
                }
                return new { info = "查询成功", code = result, data = entity };

            }
            catch (Exception ex)
            {

                return new { info = "查询失败：" + ex.Message, code = 1, data = new UserWorkStateEntity() };

            }
        }

        /// <summary>
        ///修改
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object UpdateUserWorkState()
        {

            var result = 0;
            var message = string.Empty;

            try
            {
                string res = HttpContext.Current.Request.Form["json"];
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                var entity = JsonConvert.DeserializeObject<UserWorkStateEntity>(json["data"].ToString());
                var getState = bll.selectState(entity.userId);
                if (getState == null)
                {
                    getState = new UserWorkStateEntity();
                    getState.userId = entity.userId;
                }
                getState.State = string.IsNullOrEmpty(entity.State) ? "" : entity.State;
                bll.addState(getState);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }

        /// <summary>
        ///修改
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object selectType()
        {

            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                UserBLL ubll = new UserBLL();
                string userId = dy.userId;
                string data = dy.data;
                var itemdetialbll = new BSFramework.Application.Busines.SystemManage.DataItemDetailBLL();
                var itembll = new BSFramework.Application.Busines.SystemManage.DataItemBLL();
                var type = itembll.GetEntityByName(data);
                var content = itemdetialbll.GetList(type.ItemId).ToList();
                var list = content.Select(x => new
                {
                    Text = x.ItemName,
                    Value = x.ItemDetailId
                }).ToList();
                return new { code = 0, info = "获取数据成功", data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
        }
    }
}