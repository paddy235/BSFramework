using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ThoughtWorks.QRCode.Codec;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class CultureController : MvcControllerBase
    {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var user = OperatorProvider.Provider.Current();

            var bll = new CultureBLL();
            var data = bll.GetCulture(user.DeptId);

            var persontotal = bll.GetPersonTotal(user.DeptId);
            var avgage = bll.GetAvgage(user.DeptId);
            data.Contents[0].CultureContent = string.Format(data.Contents[0].CultureContent, persontotal, Math.Round(avgage, 1));

            var last = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);

            var current1 = bll.GetTotal1(user.DeptId, DateTime.Now);
            var current2 = bll.GetTotal2(user.DeptId, DateTime.Now);
            var current3 = bll.GetTotal3(user.DeptId, DateTime.Now);
            var current4 = bll.GetTotal4(user.DeptId, DateTime.Now);

            ViewBag.current1 = current1;
            ViewBag.current2 = current2;
            ViewBag.current3 = current3.ToString("p");
            ViewBag.current4 = current4;

            var last1 = bll.GetTotal1(user.DeptId, last);
            var last2 = bll.GetTotal2(user.DeptId, last);
            var last3 = bll.GetTotal3(user.DeptId, last);
            var last4 = bll.GetTotal4(user.DeptId, last);

            ViewBag.last1 = last1;
            ViewBag.last2 = last2;
            ViewBag.last3 = last3.ToString("p");
            ViewBag.last4 = last4;

            var image1 = bll.GetImage1(user.DeptId, DateTime.Now);
            var image2 = bll.GetImage2(user.DeptId, DateTime.Now);
            var image3 = bll.GetImage3(user.DeptId, DateTime.Now);
            var image4 = bll.GetImage4(user.DeptId, DateTime.Now);

            ViewBag.image1 = image1;
            ViewBag.image2 = image2;
            ViewBag.image3 = image3;
            ViewBag.image4 = image4;

            var notices = bll.GetNotices(user.DeptId);
            ViewBag.notices = notices;

            ViewBag.date1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy/MM/dd");
            ViewBag.date2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
            ViewBag.date3 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1).ToString("yyyy/MM/dd");
            ViewBag.date4 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1).ToString("yyyy/MM/dd");

            if (data.CultureTemplateSubject == "安全蓝")
                return View("Index3", data);
            else if (data.CultureTemplateSubject == "热情橙")
                return View("Index4", data);

            return View(data);
        }

        public ViewResult Index2()
        {
            return View();
        }

        public JsonResult IndexData()
        {
            return Json(null);
        }
        private DepartmentBLL dept = new DepartmentBLL();

        public ViewResult Edit(string id)
        {
            var user = OperatorProvider.Provider.Current();
            string deptid = string.Empty;
            if (user.DeptId != "0")
            {
                var userDpet = dept.GetEntity(user.DeptId);
                deptid = user.DeptId;
                if (userDpet != null)
                {
                    var parent = dept.GetEntity(userDpet.ParentId);
                    deptid = parent.DepartmentId;
                }
            }
            ViewBag.deptid = deptid;

            var bll = new CultureBLL();

            var data = bll.GetTemplate(id);
            if (data == null)
            {
                data = new CultureTemplateEntity()
                {
                    Contents = new List<CultureTemplateItemEntity>()
                {
                    new CultureTemplateItemEntity() { CultureTemplateItemId = Guid.NewGuid().ToString(), ContentSubject = "班组简介"  },
                    new CultureTemplateItemEntity() { CultureTemplateItemId = Guid.NewGuid().ToString(), ContentSubject = "班组成员"  },
                    new CultureTemplateItemEntity() { CultureTemplateItemId = Guid.NewGuid().ToString(), ContentSubject = "安全理念"  },
                    new CultureTemplateItemEntity() { CultureTemplateItemId = Guid.NewGuid().ToString(), ContentSubject = "班务公开"  },
                    new CultureTemplateItemEntity() { CultureTemplateItemId = Guid.NewGuid().ToString(), ContentSubject = "班组荣誉"  },
                    new CultureTemplateItemEntity() { CultureTemplateItemId = Guid.NewGuid().ToString(), ContentSubject = "班组风采"  },
                    new CultureTemplateItemEntity() { CultureTemplateItemId = Guid.NewGuid().ToString(), ContentSubject = "公告栏"  },
                    new CultureTemplateItemEntity() { CultureTemplateItemId = Guid.NewGuid().ToString(), ContentSubject = "7S管理"  },
                    new CultureTemplateItemEntity() { CultureTemplateItemId = Guid.NewGuid().ToString(), ContentSubject = "发展历程"  },
                    new CultureTemplateItemEntity() { CultureTemplateItemId = Guid.NewGuid().ToString(), ContentSubject = "员工风采"  }
                }
                };
            }

            ViewBag.contents = data.Contents;

            return View(data);
        }

        [HttpPost]
        public JsonResult Edit(string id, CultureTemplateEntity model)
        {
            var bll = new CultureBLL();

            var success = true;
            var message = "保存成功";

            model.CreateTime = DateTime.Now;
            model.CreateUserId = OperatorProvider.Provider.Current().UserId;

            foreach (var item in model.Contents)
            {
                item.CultureContent = HttpUtility.UrlDecode(item.CultureContent);
                if (item.CultureContent == "null") item.CultureContent = null;
            }

            try
            {
                bll.SaveCulture(model);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }

        public ViewResult Edit2(string id)
        {
            var bll = new CultureBLL();

            var data = bll.GetTemplateContent(id);

            if (data == null) data = new CultureTemplateItemEntity();
            ViewBag.id = id;

            return View(data);
        }

        public JsonResult GetData(string name, int rows, int page)
        {
            var bll = new CultureBLL();
            var total = 0;
            var data = bll.GetData(name, rows, page, out total);
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / rows) }, JsonRequestBehavior.AllowGet);
        }
    }
}
