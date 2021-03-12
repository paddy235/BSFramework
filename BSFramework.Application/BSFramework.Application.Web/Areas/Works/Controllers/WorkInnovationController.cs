using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.InnovationManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.InnovationManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    /// <summary>
    /// 班组管理创新成果
    /// </summary>
    public class WorkInnovationController : MvcControllerBase
    {
        private DepartmentBLL deptbll = new DepartmentBLL();
        private WorkInnovationBLL bll = new WorkInnovationBLL();
        /// <summary>
        /// 展示页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var user = OperatorProvider.Provider.Current();

            var tree = deptbll.GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = tree.DepartmentId;
            return View();
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public ActionResult GetPageListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            var queryParam = queryJson.ToJObject();
            queryParam.Add("stateout", "待提交");
            queryJson = queryParam.ToJson();
            var data = bll.getList(queryJson, pagination);
            var JsonData = new
            {
                rows = data,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
                costtime = CommonHelper.TimerEnd(watch)
            };
            return Content(JsonData.ToJson());

        }
        /// <summary>
        /// 查看
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexDetail(string keyvalue)
        {
            var user = OperatorProvider.Provider.Current();
            var data = bll.getWorkInnovationByid(keyvalue)[0];
            var ck = false;
            var person = true;
            var id = "";
            string userid = string.Empty;
            string username = string.Empty;
            foreach (var item in data.audit)
            {
                if (string.IsNullOrEmpty(item.state))
                {
                    if (user.UserId == item.userid)
                    {
                        ck = true;
                        id = item.auditid;
                    }
                    if (item.isspecial)
                    {
                        person = false;
                    }

                    break;
                }

            }
            var get = data.audit.FirstOrDefault(x => x.isspecial && string.IsNullOrEmpty(x.state));
            if (get != null)
            {
                userid = get.userid;
                username = get.username;

            }
            ViewBag.userid = userid;
            ViewBag.username = username;
            ViewBag.ck = ck;
            ViewBag.person = person;
            ViewBag.auditid = id;
            return View(data);
        }
        /// <summary>
        /// 审核
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectShow()
        {
            return View();
        }

        /// <summary>
        /// 获取类别
        /// </summary>
        /// <returns></returns>
        public JsonResult SelectUser()
        {
            var user = OperatorProvider.Provider.Current();

            string dept = BSFramework.Util.Config.GetValue("workinnovation");
            var data = bll.getAuditUser(user.UserId, dept, "web");
            return Json(data.Select(x => new BSFramework.Application.Web.Areas.EvaluateAbout.Models.TreeModel
            {
                id = x.UserId,
                value = x.UserId,
                text = x.RealName,
                isexpand = false,
                hasChildren = false
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult saveAudit(WorkInnovationAuditEntity entity)
        {
            var list = new List<WorkInnovationAuditEntity>();
            var data = bll.getAuditId(entity.auditid)[0];
            data.opinion = entity.opinion;
            data.state = entity.state;
            data.submintdate = DateTime.Now;
            list.Add(data);
            var success = true;
            var message = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(entity.userid))
                {
                    var all = bll.getWorkInnovationByid(data.innovationid)[0];
                    var get = all.audit.FirstOrDefault(x => x.sort == (data.sort + 1));
                    if (get != null)
                    {
                        get.userid = entity.userid;
                        get.username = entity.username;
                        list.Add(get);
                    }
                    else
                    {
                        var audit = new WorkInnovationAuditEntity();
                        audit.userid = entity.userid;
                        audit.innovationid = data.innovationid;
                        audit.username = entity.username;
                        audit.isspecial = true;
                        audit.auditid = Guid.NewGuid().ToString();
                        list.Add(audit);
                    }


                }
                bll.Operation(null, list, false);
                return Json(new { success, message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                success = false;
                return Json(new { success, message }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}