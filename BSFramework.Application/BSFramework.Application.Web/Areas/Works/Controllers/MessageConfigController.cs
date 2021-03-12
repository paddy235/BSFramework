using Aspose.Cells;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.BudgetAbout;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.BudgetAbout;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using Bst.Fx.MessageData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    public class MessageConfigController : MvcControllerBase
    {
        //
        // GET: /Works/Budget/

        public ViewResult Index()
        {
            return View();
        }

        public JsonResult GetData(FormCollection fc)
        {
            var title = fc.Get("title");
            var configbll = new MessageConfigBLL();
            var data = configbll.GetMessageConfigs(title);
            return Json(new { rows = data, records = data.Count });
        }

        public ViewResult Detail(string id)
        {
            var categories = new List<SelectListItem>() { new SelectListItem() { Value = "Message", Text = "消息" }, new SelectListItem() { Value = "Todo", Text = "待办" } };
            ViewData["categories"] = categories;

            var configbll = new MessageConfigBLL();
            var data = configbll.GetConfigDetail(Guid.Parse(id));
            return View(data);
        }
        public JsonResult Edit(string id, ConfigEntity config)
        {
            var configbll = new MessageConfigBLL();
            var success = true;
            var message = "保存成功！";
            try
            {
                configbll.ModifyConfig(config);
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
            }
            return Json(new AjaxResult() { type = success ? ResultType.success : ResultType.error, message = message });
        }

    }
}
