using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.WebApp;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.WebApp;
using BSFramework.Util.WebControl;
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
    public class UserEduncationFileController : ApiController
    {
        private UserBLL ubll = new UserBLL();
        private UserEduncationFileBLL bll = new UserEduncationFileBLL();
        //peoplecontroller GetQuarters  获取岗位

        /// <summary>
        ///获取转岗详情
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object SelectGetDetail()
        {
            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");

            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
            string userId = dy.userId;
            bool allowPaging = dy.allowPaging;

            string from = dy.data.from;
            string to = dy.data.to;
            string parentType = dy.data.parentType;
            string childrenType = dy.data.childrenType;
            try
            {
                DateTime? datefrom = null;
                DateTime? dateto = null;
                if (!string.IsNullOrEmpty(from))
                {
                    datefrom = Convert.ToDateTime(from);
                }
                if (!string.IsNullOrEmpty(to))
                {
                    dateto = Convert.ToDateTime(to);
                }
                int total = 0;
                UserEntity user = ubll.GetEntity(userId);
                var entity = new Eduncationdata();
                if (allowPaging)
                {
                    var pageIndex = (int)dy.pageIndex;
                    var pageSize = (int)dy.pageSize;
                    entity = bll.getEduncationList(user.Account,user.DepartmentId,user.RealName, userId, datefrom, dateto, parentType, childrenType, "", pageIndex, pageSize, out total);
                }
                else
                {
                    entity = bll.getEduncationList(user.Account,user.DepartmentId, user.RealName, userId, datefrom, dateto, parentType, childrenType, "", 0, 0, out total);
                }


                return new { info = "查询成功", code = result, data = entity };

            }
            catch (Exception ex)
            {

                return new { info = "查询失败：" + ex.Message, code = 1, data = new Eduncationdata() };

            }

        }
    }
}