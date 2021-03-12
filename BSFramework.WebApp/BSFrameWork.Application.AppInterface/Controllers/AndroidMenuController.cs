using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.CertificateManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.AuthorizeManage;
using BSFramework.Application.Entity.PublicInfoManage;
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

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class AndroidMenuController : ApiController
    {
        private AndroidmenuBLL bll = new AndroidmenuBLL();
        [HttpPost]
        public object getAndroidMenu()
        {
            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
          string userId = dy.userId;
          
            try
            {
                UserEntity user = new UserBLL().GetEntity(userId);
                if (user==null)
                {
                    return new { info = "查询失败：用户错误" ,code = 1, data = new AndroidmenuEntity() };
                }
                var url = BSFramework.Util.Config.GetValue("AppUrl");
                BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL fileBll = new BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL();
                List<AndroidmenuEntity> returnList = new List<AndroidmenuEntity>();
                var entity = bll.GetList();
                var parent= entity.Where(x=>x.IsMenu&&x.IsEffective).OrderBy(x => x.Sort).ToList();
                foreach (var item in parent)
                {
                    var file = fileBll.GetFilesByRecIdNew(item.MenuId);
                    if (file.Count==1)
                    {
                        item.Icon = file[0].FilePath.Replace("~", url);
                    }
                    var child = entity.Where(x=>x.ParentId==item.MenuId&&x.IsEffective).OrderBy(x => x.Sort).ToList();
                    item.child = new List<AndroidmenuEntity>();
                    foreach (var items in child)
                    {
                        file = fileBll.GetFilesByRecIdNew(items.MenuId);
                        if (file.Count == 1)
                        {
                            items.Icon = file[0].FilePath.Replace("~", url);
                        }
                    }
                    item.child.AddRange(child);
                    returnList.Add(item);
                }
                return new { info = "查询成功", code = result, data = returnList };

            }
            catch (Exception ex)
            {

                return new { info = "查询失败：" + ex.Message, code = 1, data = new AndroidmenuEntity() };

            }
        }

    }
}
