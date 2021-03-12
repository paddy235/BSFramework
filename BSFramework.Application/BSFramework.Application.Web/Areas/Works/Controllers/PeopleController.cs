using Aspose.Cells;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.LllegalManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Busines.WebApp;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.Web.Areas.Works.Models;
using BSFramework.Application.Web.Areas.Works.Views.EduTrain;
using BSFramework.Util;
using BSFramework.Util.Offices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ThoughtWorks.QRCode.Codec;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    public class PeopleController : MvcControllerBase
    {
        //
        // GET: /Works/People/
        private LllegalBLL lbll = new LllegalBLL();
        //private PostCache postCache = new PostCache();
        private UserBLL userbll = new UserBLL();
        private DepartmentBLL deptbll = new DepartmentBLL();
        private PeopleBLL peoplebll = new PeopleBLL();
        private RoleBLL rbll = new RoleBLL();
        private PostBLL pbll = new PostBLL();
        private AreaBLL areaBLL = new AreaBLL();
        private UserWorkAllocationBLL userwork = new UserWorkAllocationBLL();
        public ActionResult Index()
        {

            return View();
        }
        /// <summary>
        /// 展示转岗信息
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public ActionResult getStateInfo(string keyValue)
        {
            var data = userwork.GetDetailByUser(keyValue);
            return View(data);
        }

        public ActionResult IndexDuty()
        {
            var user = OperatorProvider.Provider.Current();

            var tree = deptbll.GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = tree.DepartmentId;

            return View();
        }
        public List<RoleEntity> getNewData(string userid, string deptid)
        {
            if (deptid == "0") deptid = "";
            var dict = new
            {
                data = deptid,
                userid = userid,
                tokenid = ""
            };
            string res = HttpMethods.HttpPost(Path.Combine(Config.GetValue("ErchtmsApiUrl"), "Post", "GetPostByDeptId"), "json=" + JsonConvert.SerializeObject(dict));
            var ret = JsonConvert.DeserializeObject<RetDataModel>(res);
            return JsonConvert.DeserializeObject<List<RoleEntity>>(ret.data.ToString());
            //return new List<RoleEntity>();
        }

        public static SelectList getQuarters(string isbzz, string sel)
        {
            var curr = OperatorProvider.Provider.Current();
            var dict = new
            {
                //data = "4ad1d7d0-50af-4966-8c48-48570075f009",
                //userid = "System",
                //tokenid = ""
                data = curr.DeptId,
                userid = curr.UserId,
                tokenid = ""
            };
            string res = HttpMethods.HttpPost(Path.Combine(Config.GetValue("ErchtmsApiUrl"), "Post", "GetJobByDeptId"), "json=" + JsonConvert.SerializeObject(dict));
            var ret = JsonConvert.DeserializeObject<RetDataModel>(res);
            List<RoleEntity> data = JsonConvert.DeserializeObject<List<RoleEntity>>(ret.data.ToString());
            data = data.OrderBy(x => x.EnCode).ToList();
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (RoleEntity d in data)
            {
                items.Add(new SelectListItem() { Value = d.EnCode, Text = d.FullName });
            }
            SelectList quarterlist = new SelectList(items, "Value", "Text");
            return quarterlist;

            // return new SelectList(new List<SelectListItem>());

        }
        public JsonResult GetDutyData()
        {
            var user = OperatorProvider.Provider.Current();
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var deptid = this.Request.QueryString.Get("deptid");
            var name = this.Request.QueryString.Get("name");
            DepartmentBLL deptBll = new DepartmentBLL();
            PostBLL pbll = new PostBLL();

            if (string.IsNullOrEmpty(deptid)) { deptid = user.DeptId; }
            var data = getNewData(user.UserId, deptid);
            //var data = new List<RoleEntity>();
            if (!string.IsNullOrEmpty(name))
            {
                data = data.Where(x => x.FullName.Contains(name)).ToList();
            }
            foreach (RoleEntity role in data)
            {
                var duty = peoplebll.GetDutyEntityByRole(role.RoleId);
                if (duty != null)
                {
                    role.DutyContent = duty.DutyContent;
                    role.ReviseDate = duty.ReviseDate;
                    role.ReviseUserName = duty.ReviseUserName;
                }

            }
            var total = data.Count();

            data = data.Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / pagesize) }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDutyDangerData()
        {
            var user = OperatorProvider.Provider.Current();
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var deptid = this.Request.QueryString.Get("deptid");
            var name = this.Request.QueryString.Get("name");
            DepartmentBLL deptBll = new DepartmentBLL();
            PostBLL pbll = new PostBLL();
            if (string.IsNullOrEmpty(deptid)) { deptid = user.DeptId; }
            var data = getNewData(user.UserId, deptid);
            if (!string.IsNullOrEmpty(name))
            {
                data = data.Where(x => x.FullName.Contains(name)).ToList();
            }
            foreach (RoleEntity role in data)
            {
                var dutydanger = peoplebll.GetDutyDangerEntityByRole(role.RoleId);

                if (dutydanger != null)
                {
                    role.DutyContent1 = dutydanger.DutyContent;
                    role.Danger = dutydanger.Danger;
                    role.Measure = dutydanger.Measure;
                    role.ReviseDate1 = dutydanger.ReviseDate;
                    role.ReviseDate2 = dutydanger.DangerReviseDate;
                }
            }
            var total = data.Count();

            data = data.Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / pagesize) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FormDuty(string id, string name, string dept)
        {
            var user = OperatorProvider.Provider.Current();
            var model = new RoleEntity();
            model.RoleId = id;
            model.FullName = name;
            if (!string.IsNullOrEmpty(dept) && dept != "null")
                model.DepartmentName = dept;
            var duty = peoplebll.GetDutyEntityByRole(id);
            if (duty != null) model.DutyContent = duty.DutyContent;
            model.ReviseDate = DateTime.Now;
            model.ReviseUserName = user.UserName;

            return View(model);
        }
        public ActionResult FormDutyDanger(string id, string name, string dept)
        {
            var user = OperatorProvider.Provider.Current();
            var model = new RoleEntity();
            model.RoleId = id;
            model.FullName = name;
            if (!string.IsNullOrEmpty(dept) && dept != "null")
                model.DepartmentName = dept;

            var dutydanger = peoplebll.GetDutyDangerEntityByRole(id);
            if (dutydanger != null)
            {
                model.Danger = dutydanger.Danger;
                model.Measure = dutydanger.Measure;
                model.DutyContent1 = dutydanger.DutyContent;
            }
            model.ReviseUserName1 = user.UserName;
            model.ReviseUserName2 = user.UserName;
            model.ReviseDate1 = DateTime.Now;
            model.ReviseDate2 = DateTime.Now;
            return View(model);
        }
        public ActionResult DetailDuty(string id, string name, string dept)
        {
            var user = OperatorProvider.Provider.Current();
            var model = new RoleEntity();
            model.RoleId = id;
            model.FullName = name;
            if (!string.IsNullOrEmpty(dept) && dept != "null")
                model.DepartmentName = dept;
            var duty = peoplebll.GetDutyEntityByRole(id);
            if (duty != null)
            {
                model.DutyContent = duty.DutyContent;
                model.ReviseDate = duty.ReviseDate;
                model.ReviseUserName = duty.ReviseUserName;
            }

            return View(model);
        }
        public ActionResult DetailDutyDanger(string id, string name, string dept)
        {
            var user = OperatorProvider.Provider.Current();
            var model = new RoleEntity();
            model.RoleId = id;
            model.FullName = name;
            if (!string.IsNullOrEmpty(dept) && dept != "null")
                model.DepartmentName = dept;
            var duty = peoplebll.GetDutyEntityByRole(id);
            if (duty != null)
            {
                model.DutyContent = duty.DutyContent;
                model.ReviseDate = duty.ReviseDate;
                model.ReviseUserName = duty.ReviseUserName;
            }
            var dutydanger = peoplebll.GetDutyDangerEntityByRole(id);
            if (dutydanger != null)
            {
                model.Danger = dutydanger.Danger;
                model.Measure = dutydanger.Measure;
                model.DutyContent1 = dutydanger.DutyContent;
                model.ReviseDate1 = dutydanger.ReviseDate;
                model.ReviseUserName1 = dutydanger.ReviseUserName;
                model.ReviseDate2 = dutydanger.DangerReviseDate;
                model.ReviseUserName2 = dutydanger.DangerReviseUserName;
            }
            return View(model);
        }
        [HttpPost]
        public JsonResult SaveDuty(string id, RoleEntity model)
        {
            var user = OperatorProvider.Provider.Current();
            var duty = peoplebll.GetDutyEntityByRole(model.RoleId);
            if (duty == null)
            {
                duty = new DutyEntity();
                duty.ID = Guid.NewGuid().ToString();
                duty.CreateDate = DateTime.Now;
                duty.CreateUserId = user.UserId;
                duty.CreateUserName = user.UserName;
                duty.DutyContent = model.DutyContent;
                duty.ReviseDate = model.ReviseDate.Value;
                //duty.ReviseUserId = user.UserId;
                duty.ReviseUserName = model.ReviseUserName;
                duty.RoleId = model.RoleId;
                duty.RoleName = model.FullName;

            }
            else
            {
                duty.DutyContent = model.DutyContent;
                duty.ReviseDate = model.ReviseDate.Value;
                duty.ReviseUserName = model.ReviseUserName;
            }
            peoplebll.SaveDuty(duty.ID, duty);
            return Json(new { success = true, message = "操作成功" });
        }
        public JsonResult SaveDutyDanger(string id, RoleEntity model)
        {
            var user = OperatorProvider.Provider.Current();
            var duty = peoplebll.GetDutyDangerEntityByRole(model.RoleId);
            if (duty == null)
            {
                duty = new DutyDangerEntity();
                duty.ID = Guid.NewGuid().ToString();
                duty.CreateDate = DateTime.Now;
                duty.CreateUserId = user.UserId;
                duty.CreateUserName = user.UserName;
                duty.Danger = model.Danger;
                duty.Measure = model.Measure;
                duty.DutyContent = model.DutyContent1;
                if (!string.IsNullOrEmpty(duty.DutyContent))
                    duty.ReviseDate = model.ReviseDate1.Value;
                if (!string.IsNullOrEmpty(duty.Danger) || !string.IsNullOrEmpty(duty.Measure))
                    duty.DangerReviseDate = model.ReviseDate2.Value;
                //duty.ReviseUserId = user.UserId;
                duty.ReviseUserName = model.ReviseUserName1;
                duty.DangerReviseUserName = model.ReviseUserName2;
                duty.RoleId = model.RoleId;
                duty.RoleName = model.FullName;

            }
            else
            {
                if (!string.IsNullOrEmpty(model.DutyContent1) && duty.DutyContent != model.DutyContent1)
                    duty.ReviseDate = model.ReviseDate1.Value;
                if (!string.IsNullOrEmpty(model.Danger) || !string.IsNullOrEmpty(model.Measure))
                    if (duty.Danger != model.Danger || duty.Measure != model.Measure)
                        duty.DangerReviseDate = model.ReviseDate2.Value;
                duty.DutyContent = model.DutyContent1;
                duty.Danger = model.Danger;
                duty.Measure = model.Measure;

                duty.ReviseUserName = model.ReviseUserName1;

                duty.DangerReviseUserName = model.ReviseUserName2;
            }
            peoplebll.SaveDutyDanger(duty.ID, duty);
            return Json(new { success = true, message = "操作成功" });
        }

        public ActionResult Export(string deptid, string name)
        {
            var user = OperatorProvider.Provider.Current();
            PostBLL pbll = new PostBLL();
            if (string.IsNullOrEmpty(deptid)) deptid = user.DeptId;
            var data = getNewData(user.UserId, user.DeptId);
            if (!string.IsNullOrEmpty(name))
            {
                data = data.Where(x => x.FullName.Contains(name)).ToList();
            }
            foreach (RoleEntity role in data)
            {
                var duty = peoplebll.GetDutyEntityByRole(role.RoleId);
                if (duty != null)
                {
                    role.DutyContent = duty.DutyContent;
                    role.ReviseDate = duty.ReviseDate;
                    role.ReviseUserName = duty.ReviseUserName;
                }
            }

            Workbook workbook = new Workbook();
            Worksheet sheet = (Worksheet)workbook.Worksheets[0];
            Cells cells = sheet.Cells;

            Style style1 = workbook.Styles[workbook.Styles.Add()];
            style1.Font.Color = Color.Black;
            style1.Font.Size = 11;
            style1.Font.IsBold = true;

            style1.HorizontalAlignment = TextAlignmentType.Center;
            style1.VerticalAlignment = TextAlignmentType.Center;

            Style style2 = workbook.Styles[workbook.Styles.Add()];
            style2.Font.Color = Color.Black;
            style2.Font.Size = 11;
            style2.Font.IsBold = false;
            style2.HorizontalAlignment = TextAlignmentType.Center;
            style2.VerticalAlignment = TextAlignmentType.Center;
            cells.SetColumnWidth(0, 15); cells.SetColumnWidth(2, 30);
            cells.SetColumnWidth(1, 30);
            cells.SetColumnWidth(3, 30);
            cells.SetColumnWidth(4, 30);
            cells.SetColumnWidth(5, 30);
            cells[0, 0].PutValue("序号"); cells[0, 0].SetStyle(style1);
            cells[0, 1].PutValue("部门名称"); cells[0, 1].SetStyle(style1);
            cells[0, 2].PutValue("岗位名称"); cells[0, 2].SetStyle(style1);
            cells[0, 3].PutValue("岗位职责"); cells[0, 3].SetStyle(style1);
            cells[0, 4].PutValue("修订人员"); cells[0, 4].SetStyle(style1);
            cells[0, 5].PutValue("修订时间"); cells[0, 5].SetStyle(style1);

            var roles = data.ToList();
            for (int i = 0; i < data.Count(); i++)
            {

                var role = roles[i];
                cells[i + 1, 0].PutValue(i + 1); cells[i + 1, 0].SetStyle(style2);
                cells[i + 1, 1].PutValue(role.DepartmentName); cells[i + 1, 1].SetStyle(style2);
                cells[i + 1, 2].PutValue(role.FullName); cells[i + 1, 2].SetStyle(style2);
                cells[i + 1, 3].PutValue(role.DutyContent); cells[i + 1, 3].SetStyle(style2);
                cells[i + 1, 4].PutValue(role.ReviseUserName); cells[i + 1, 4].SetStyle(style2);
                cells[i + 1, 5].PutValue(role.ReviseDate == null ? "" : role.ReviseDate.Value.ToString("yyyy-MM-dd HH:mm"));
                cells[i + 1, 5].SetStyle(style2);
            }
            var path = Server.MapPath("~/Content/export/");
            path = path.Substring(0, path.LastIndexOf("\\") + 1);

            workbook.Save(Path.Combine(path, "岗位职责导出.xlsx"), Aspose.Cells.SaveFormat.Xlsx);
            ExcelHelper.DownLoadFile(Path.Combine(path, "岗位职责导出.xlsx"), "岗位职责导出.xlsx");
            return Success("导出成功。", new { });
        }

        public ActionResult ExportDanger(string deptid, string name)
        {
            var user = OperatorProvider.Provider.Current();
            PostBLL pbll = new PostBLL();
            if (string.IsNullOrEmpty(deptid)) deptid = user.DeptId;
            var data = getNewData(user.UserId, user.DeptId);
            if (!string.IsNullOrEmpty(name))
            {
                data = data.Where(x => x.FullName.Contains(name)).ToList();
            }
            foreach (RoleEntity role in data)
            {
                var dutydanger = peoplebll.GetDutyDangerEntityByRole(role.RoleId);

                if (dutydanger != null)
                {
                    role.DutyContent1 = dutydanger.DutyContent;
                    role.Danger = dutydanger.Danger;
                    role.Measure = dutydanger.Measure;
                    role.ReviseDate1 = dutydanger.ReviseDate;
                    role.ReviseDate2 = dutydanger.DangerReviseDate;
                }
            }

            Workbook workbook = new Workbook();
            Worksheet sheet = (Worksheet)workbook.Worksheets[0];
            Cells cells = sheet.Cells;

            Style style1 = workbook.Styles[workbook.Styles.Add()];
            style1.Font.Color = Color.Black;
            style1.Font.Size = 11;
            style1.Font.IsBold = true;

            style1.HorizontalAlignment = TextAlignmentType.Center;
            style1.VerticalAlignment = TextAlignmentType.Center;

            Style style2 = workbook.Styles[workbook.Styles.Add()];
            style2.Font.Color = Color.Black;
            style2.Font.Size = 11;
            style2.Font.IsBold = false;
            style2.HorizontalAlignment = TextAlignmentType.Center;
            style2.VerticalAlignment = TextAlignmentType.Center;

            cells[0, 0].PutValue("序号"); cells[0, 0].SetStyle(style1);
            cells[0, 1].PutValue("部门名称"); cells[0, 1].SetStyle(style1);
            cells[0, 2].PutValue("岗位名称"); cells[0, 2].SetStyle(style1);
            cells[0, 3].PutValue("岗位安全职责"); cells[0, 3].SetStyle(style1);
            cells[0, 4].PutValue("岗位存在的危险点"); cells[0, 4].SetStyle(style1);
            cells[0, 5].PutValue("危险点防范措施"); cells[0, 5].SetStyle(style1);
            cells[0, 6].PutValue("安全职责修订时间"); cells[0, 6].SetStyle(style1);
            cells[0, 7].PutValue("危险点防范措施修订时间"); cells[0, 7].SetStyle(style1);
            cells.SetColumnWidth(0, 15);
            cells.SetColumnWidth(1, 30);
            cells.SetColumnWidth(2, 30);
            cells.SetColumnWidth(3, 30);
            cells.SetColumnWidth(4, 30);
            cells.SetColumnWidth(5, 30);
            cells.SetColumnWidth(6, 30);
            cells.SetColumnWidth(7, 30);
            var roles = data.ToList();
            for (int i = 0; i < data.Count(); i++)
            {
                var role = roles[i];
                cells[i + 1, 0].PutValue(i + 1); cells[i + 1, 0].SetStyle(style2);
                cells[i + 1, 1].PutValue(role.DepartmentName); cells[i + 1, 1].SetStyle(style2);
                cells[i + 1, 2].PutValue(role.FullName); cells[i + 1, 2].SetStyle(style2);
                cells[i + 1, 3].PutValue(role.DutyContent1); cells[i + 1, 3].SetStyle(style2);
                cells[i + 1, 4].PutValue(role.Danger); cells[i + 1, 4].SetStyle(style2);
                cells[i + 1, 5].PutValue(role.Measure);
                cells[i + 1, 5].SetStyle(style2);
                cells[i + 1, 6].PutValue(role.ReviseDate1 == null ? "" : role.ReviseDate1.Value.ToString("yyyy-MM-dd HH:mm")); cells[i + 1, 6].SetStyle(style2);
                cells[i + 1, 7].PutValue(role.ReviseDate2 == null ? "" : role.ReviseDate2.Value.ToString("yyyy-MM-dd HH:mm")); cells[i + 1, 7].SetStyle(style2);
            }
            var path = Server.MapPath("~/Content/export/");
            path = path.Substring(0, path.LastIndexOf("\\") + 1);

            workbook.Save(Path.Combine(path, "岗位安全职责、危险防范措施导出.xlsx"), Aspose.Cells.SaveFormat.Xlsx);
            ExcelHelper.DownLoadFile(Path.Combine(path, "岗位安全职责、危险防范措施导出.xlsx"), "岗位安全职责、危险防范措施导出.xlsx");
            return Success("导出成功。", new { });
        }

        public ActionResult ImportNew(string type)
        {
            ViewBag.type = type;
            return View();
        }
        public JsonResult DoImportNew(string type)
        {
            FileInfoBLL fileBll = new FileInfoBLL();
            var success = true;
            var message = string.Empty;

            try
            {
                if (this.Request.Files.Count == 0) throw new Exception("请上传文件");
                if (!this.Request.Files[0].FileName.EndsWith(".xlsx")) return Json(new { success = false, message = "请上传excel文件！" });

                var book = new Workbook(this.Request.Files[0].InputStream);
                var sheet = book.Worksheets[0];
                var user = OperatorProvider.Provider.Current();
                if (type == "1")  //导入岗位职责
                {
                    if (sheet.Cells[1, 0].StringValue != "部门名称" || sheet.Cells[1, 1].StringValue != "岗位名称" || sheet.Cells[1, 2].StringValue != "岗位职责")
                    {
                        return Json(new { success = false, message = "请使用正确的模板导入！" });
                    }

                    var date = DateTime.Now;
                    PostBLL pbll = new PostBLL();
                    var data = getNewData(user.UserId, user.DeptId);
                    for (int i = 1; i < sheet.Cells.MaxDataRow; i++)
                    {
                        var deptname = sheet.Cells[i + 1, 0].StringValue;
                        var fullname = sheet.Cells[i + 1, 1].StringValue;

                        var role = data.Where(x => x.DepartmentName == deptname && x.FullName == fullname).FirstOrDefault();
                        if (role == null)
                        {
                            success = false;
                            message = "岗位及部门与系统数据不对应，导入失败！";
                            return Json(new { success, message });
                        }
                    }
                    for (int i = 1; i < sheet.Cells.MaxDataRow; i++)
                    {
                        var deptname = sheet.Cells[i + 1, 0].StringValue;
                        var fullname = sheet.Cells[i + 1, 1].StringValue;
                        var content = sheet.Cells[i + 1, 2].StringValue;
                        if (string.IsNullOrEmpty(content)) continue;
                        var role = data.Where(x => x.DepartmentName == deptname && x.FullName == fullname).FirstOrDefault();

                        var duty = peoplebll.GetDutyEntityByRole(role.RoleId);
                        if (duty == null)
                        {
                            duty = new DutyEntity();
                            duty.ID = Guid.NewGuid().ToString();
                            duty.CreateDate = DateTime.Now;
                            duty.CreateUserId = user.UserId;
                            duty.CreateUserName = user.UserName;
                            duty.DutyContent = content;
                            duty.ReviseDate = DateTime.Now;
                            duty.ReviseUserId = user.UserId;
                            duty.ReviseUserName = user.UserName;
                            duty.RoleId = role.RoleId;
                            peoplebll.SaveDuty(duty.ID, duty);
                        }
                        else
                        {
                            duty.DutyContent = content;
                            duty.ReviseDate = DateTime.Now;
                            duty.ReviseUserId = user.UserId;
                            duty.ReviseUserName = user.UserName;
                            peoplebll.SaveDuty(duty.ID, duty);
                        }
                    }
                }
                else if (type == "2")
                {
                    if (sheet.Cells[1, 0].StringValue != "部门名称" || sheet.Cells[1, 1].StringValue != "岗位名称" || sheet.Cells[1, 2].StringValue != "岗位安全职责" || sheet.Cells[1, 3].StringValue != "岗位存在的危险点" || sheet.Cells[1, 4].StringValue != "危险点防范措施")
                    {
                        return Json(new { success = false, message = "请使用正确的模板导入！" });
                    }

                    var date = DateTime.Now;
                    PostBLL pbll = new PostBLL();
                    var data = getNewData(user.UserId, user.DeptId);

                    for (int i = 1; i < sheet.Cells.MaxDataRow; i++)
                    {
                        var deptname = sheet.Cells[i + 1, 0].StringValue;
                        var fullname = sheet.Cells[i + 1, 1].StringValue;
                        var content = sheet.Cells[i + 1, 2].StringValue;
                        var role = data.Where(x => x.DepartmentName == deptname && x.FullName == fullname).FirstOrDefault();
                        if (role == null)
                        {
                            success = false;
                            message = "岗位及部门与系统数据不对应，导入失败！";
                            return Json(new { success, message });
                        }
                    }
                    for (int i = 1; i < sheet.Cells.MaxDataRow; i++)
                    {
                        var deptname = sheet.Cells[i + 1, 0].StringValue;
                        var fullname = sheet.Cells[i + 1, 1].StringValue;
                        var content = sheet.Cells[i + 1, 2].StringValue;
                        var danger = sheet.Cells[i + 1, 3].StringValue;
                        var measure = sheet.Cells[i + 1, 4].StringValue;

                        if (string.IsNullOrEmpty(content) && string.IsNullOrEmpty(danger) && string.IsNullOrEmpty(measure)) continue;
                        var role = data.Where(x => x.DepartmentName == deptname && x.FullName == fullname).FirstOrDefault();

                        var duty = peoplebll.GetDutyDangerEntityByRole(role.RoleId);
                        if (duty == null)
                        {
                            duty = new DutyDangerEntity();
                            duty.ID = Guid.NewGuid().ToString();
                            duty.CreateDate = DateTime.Now;
                            duty.CreateUserId = user.UserId;
                            duty.CreateUserName = user.UserName;
                            if (!string.IsNullOrEmpty(content))
                            {
                                duty.DutyContent = content;
                                duty.ReviseDate = DateTime.Now;
                                duty.ReviseUserId = user.UserId;
                                duty.ReviseUserName = user.UserName;
                            }
                            if (!string.IsNullOrEmpty(danger))
                            {
                                duty.Danger = danger;
                            }
                            if (!string.IsNullOrEmpty(measure))
                            {
                                duty.Measure = measure;
                            }
                            if (!string.IsNullOrEmpty(danger) || !string.IsNullOrEmpty(measure))
                            {
                                duty.DangerReviseDate = DateTime.Now;

                                duty.DangerReviseUserId = user.UserId;
                                duty.DangerReviseUserName = user.UserName;
                            }
                            duty.RoleId = role.RoleId;

                            peoplebll.SaveDutyDanger(duty.ID, duty);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(content))
                            {
                                duty.DutyContent = content;
                                duty.ReviseDate = DateTime.Now;
                                duty.ReviseUserId = user.UserId;
                                duty.ReviseUserName = user.UserName;
                            }
                            if (!string.IsNullOrEmpty(danger))
                            {
                                duty.Danger = danger;
                            }
                            if (!string.IsNullOrEmpty(measure))
                            {
                                duty.Measure = measure;
                            }
                            if (!string.IsNullOrEmpty(danger) || !string.IsNullOrEmpty(measure))
                            {
                                duty.DangerReviseDate = DateTime.Now;
                                duty.DangerReviseUserId = user.UserId;
                                duty.DangerReviseUserName = user.UserName;
                            }
                            peoplebll.SaveDutyDanger(duty.ID, duty);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }

            return Json(new { success, message });
        }
        public ActionResult IndexDutyDanger()
        {
            var user = OperatorProvider.Provider.Current();

            var tree = deptbll.GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = tree.DepartmentId;
            return View();
        }
        private UserWorkAllocationBLL bll = new UserWorkAllocationBLL();

        /// <summary>
        /// 有参数则表示先删除后查询，否则只查询
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="fc"></param>
        /// <returns></returns>current
        public ActionResult List(string id, int page, int pagesize, FormCollection fc)
        {
            var users = OperatorProvider.Provider.Current();
            //if (string.IsNullOrEmpty(id))
            //{
            //    peoplebll.RemoveForm(id);
            //}
            if (page == 0) page = 1;
            if (pagesize == 0) page = 12;

            var total = 0;
            var data = peoplebll.GetList(users.DeptId, page, pagesize, out total).ToList();
            if (data.Count() > 0)
            {
                var deptid = users.DeptId;

                #region  转岗读取信息
                var old = bll.GetIsSendUser(deptid).ToList();//原部门
                var newuser = bll.GetSendUser(deptid).ToList();//转向部门
                foreach (var item in old)
                {
                    var one = data.FirstOrDefault(x => x.ID == item.userId);
                    if (one == null)
                    {
                        continue;
                    }
                    one.state = "转岗确认中";
                    one.AllocationId = item.Id;
                    var ck = false;
                    if (item.oldquarters != one.Quarters)
                    {
                        item.oldquarters = one.Quarters;
                        ck = true;
                    }
                    if (item.oldRoleDutyName != one.RoleDutyName)
                    {
                        item.oldRoleDutyName = one.RoleDutyName;
                        ck = true;
                    }
                    if (item.username != one.Name)
                    {
                        item.username = one.Name;
                        ck = true;
                    }
                    if (ck)
                    {
                        bll.OperationEntity(item, users.UserId);
                    }

                }
                foreach (var item in newuser)
                {
                    var one = peoplebll.GetEntity(item.userId);
                    if (one == null)
                    {
                        continue;
                    }
                    one.state = "转岗待确认";
                    one.AllocationId = item.Id;
                    var ck = false;
                    if (item.oldquarters != one.Quarters)
                    {
                        item.oldquarters = one.Quarters;
                        ck = true;
                    }
                    if (item.oldRoleDutyName != one.RoleDutyName)
                    {
                        item.oldRoleDutyName = one.RoleDutyName;
                        ck = true;
                    }
                    if (item.username != one.Name)
                    {
                        item.username = one.Name;
                        ck = true;
                    }
                    if (ck)
                    {
                        bll.OperationEntity(item, users.UserId);
                    }
                    if (data.Count() < pagesize)
                    {
                        data.Add(one);
                    }

                }
                #endregion
            }

            ViewBag.pages = Math.Ceiling((decimal)total / pagesize);
            ViewBag.current = page;
            ViewBag.pagesize = pagesize;
            return View(data);
        }
        [HttpPost]
        public ActionResult ValidTel(string tel)
        {
            int n = peoplebll.CheckTel(tel);
            ViewBag.Tel = n;
            if (n != 0)  //手机号已存在
            {
                return Success("0", new { tel = "1" });
            }
            else
            {
                return Success("0", new { tel = "0" });
            }
        }
        public ActionResult LList(int page, int pagesize, FormCollection fc)
        {

            var user = OperatorProvider.Provider.Current();

            var total = 0;
            var data = new LllegalBLL().GetListNew(user.DeptId, page, pagesize, out total);

            ViewBag.pages = Math.Ceiling((decimal)total / pagesize);
            ViewBag.current = page;
            ViewBag.pagesize = pagesize;


            return View(data);
        }

        /// <summary>
        /// 成员详情，违章信息分页
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public ActionResult GetLList(string id, int page, int pagesize)
        {


            if (page == 0) page = 1;
            if (pagesize == 0) page = 12;

            var total = 0;
            var data = lbll.GetListNew(id, page, pagesize, out total);
            ViewBag.pages = Math.Ceiling((decimal)total / pagesize);
            ViewBag.current = page;
            ViewBag.Id = id;
            ViewData["LllEntity"] = data.ToList();
            return View(data);


        }

        public IView reflectedView(ControllerContext c, string viewname, string cname, string areaname, string masterpath = null)
        {
            if (c == null)
            {
                throw new ArgumentException("控制器对象获取失败");
            }
            if (string.IsNullOrEmpty(viewname))
            {
                throw new ArgumentException("无法找到视图");
            }
            //foreach(string path in )
            return null;
        }
        public ActionResult Form()
        {
            return View();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult New(string type, string tempImageSrc, PeopleEntity model)
        {
            ViewData["Roles"] = getRoles("", "");
            ViewData["qddl"] = getQuarters("n", "");
            ViewData["Job"] = getJob("");
            ViewData["minzu"] = getFolk("");
            ViewData["wkind"] = getWorkKind("");
            ViewData["Provins"] = getProvins("");
            ViewBag.Tel = "0";
            ViewBag.labour = "0";
            ViewBag.IdentityNo = "0";
            ViewBag.Account = "2";
            if (type == "tolist")   //保存成功点击完成，重新提交，直接返回列表页
            {
                return RedirectToAction("List", "People", new { page = 1, pagesize = 12, area = "Works" });
            }
            FileInfoBLL fb = new FileInfoBLL();

            if (!string.IsNullOrEmpty(model.IdentityNo))
            {
                int l = peoplebll.CheckIdentity(model.IdentityNo);
                ViewBag.IdentityNo = l;
                if (l != 0)
                {
                    return View(model);
                }
            }
            //验证电话号码是否已存在
            if (!string.IsNullOrEmpty(model.LinkWay))
            {
                int n = peoplebll.CheckTel(model.LinkWay.Trim());
                ViewBag.Tel = n;
                int l = peoplebll.CheckNo(model.LabourNo);
                if (n != 0)  //手机号已存在
                {
                    return View(model);
                }
            }
            if (!string.IsNullOrEmpty(model.LabourNo))
            {
                int l = peoplebll.CheckNo(model.LabourNo);
                ViewBag.labour = l;
                if (l != 0)  //工号已存在
                {
                    return View(model);
                }
            }

            if (string.IsNullOrEmpty(type)) //保存 成功后返回列表
            {
                model.LinkWay = model.LinkWay.Trim();
                model.ID = Guid.NewGuid().ToString();
                model.IdentityNo = model.IdentityNo.Trim();
                try
                {
                    //成员信息
                    model.UserAccount = model.LinkWay;
                    model.PassWord = model.IdentityNo.Substring(model.IdentityNo.Length - 4, 4);
                    var file = this.BuildImage(model.ID, "人员");
                    model.Files.Add(file);
                    if (!string.IsNullOrEmpty(tempImageSrc))
                    {
                        model.Photo = tempImageSrc;
                    }
                    model.FingerMark = "yes";
                    //model.Photo=ViewData["uploadPreview"]

                    //user表信息
                    UserEntity user = new UserEntity();
                    user.Account = model.LinkWay;
                    user.Password = model.IdentityNo.Substring(model.IdentityNo.Length - 4, 4);
                    DepartmentEntity dept = new DepartmentEntity();

                    string parentid = deptbll.GetEntity(OperatorProvider.Provider.Current().DeptId).ParentId;  //当前所属部门的父节点id
                    dept = deptbll.GetEntity(parentid);
                    user.DepartmentId = OperatorProvider.Provider.Current().DeptId;
                    user.DepartmentCode = OperatorProvider.Provider.Current().DeptCode;
                    user.Mobile = model.LinkWay;
                    if (model.Sex == "男")
                    {
                        user.Gender = 1;
                    }
                    else
                    {
                        user.Gender = 0;
                    }
                    user.UserId = model.ID; //用户表关联
                    user.RealName = model.Name;
                    user.OrganizeId = dept.OrganizeId;
                    user.LogOnCount = 0;
                    user.PreviousVisit = DateTime.Now;
                    user.UserOnLine = 1;
                    user.CreateUserId = OperatorProvider.Provider.Current().UserId;
                    user.CreateUserName = OperatorProvider.Provider.Current().UserName;
                    user.RoleId = model.RoleDutyId;
                    user.RoleName = model.RoleDutyName;
                    if (userbll.ExistAccount(user.Account, ""))
                    {
                        ViewBag.Account = "1";
                        var curr = OperatorProvider.Provider.Current();
                        var dutyname = getNewData(curr.UserId, "").Where(x => x.RoleId == model.RoleDutyId).FirstOrDefault();
                        if (dutyname != null) model.RoleDutyName = dutyname.FullName;
                        peoplebll.SaveForm(string.Empty, model);
                        user.DutyName = model.RoleDutyName;
                        user.DutyId = model.RoleDutyId;

                        Erchtms e = new Erchtms();
                        user.IDENTIFYID = model.IdentityNo;
                        user.EnterTime = model.EntryDate;
                        user.Birthday = model.Birthday;




                        user.LabourNo = model.LabourNo;
                        user.Craft = model.WorkKind;
                        user.Folk = model.Folk;
                        user.CurrentWorkAge = model.CurrentWorkAge;
                        user.OldDegree = model.OldDegree;
                        user.NewDegree = model.NewDegree;
                        user.Quarters = model.Quarters;
                        user.Planer = model.Planer;
                        user.Visage = model.Visage;
                        user.Age = model.Age;
                        user.Native = model.Native;
                        user.TechnicalGrade = model.TecLevel;
                        user.JobName = model.JobName;
                        user.HealthStatus = model.HealthStatus;
                        user.IsSpecial = model.IsSpecial;
                        user.IsSpecialEqu = model.IsSpecialEquipment;
                        user.EnterTime = model.EntryDate;
                        user.HeadIcon = ToBase64String(Server.MapPath(model.Photo));
                        user.SignImg = null;
                        e.ErchtmsSynchronoous("SaveUser", user, OperatorProvider.Provider.Current().Account);
                        //同步完成之后，重置roleid
                        var role1 = rbll.GetList().Where(x => x.FullName == "班组成员").FirstOrDefault();
                        var role2 = rbll.GetList().Where(x => x.FullName == "班组长").FirstOrDefault();
                        if (model.RoleDutyName == "班长" || model.RoleDutyName == "班组长")
                        {
                            user.RoleId = role2.RoleId;
                            user.RoleName = role2.FullName;
                        }
                        else
                        {
                            user.RoleId = role1.RoleId;
                            user.RoleName = role1.FullName;
                        }
                        userbll.InsertUser(user);
                        ViewBag.Result = "1";//新增成功
                        if (fb.GetPeoplePhoto(model.ID).Count() == 0)
                        {
                            ViewBag.qr = "";
                        }
                        else
                        {
                            ViewBag.qr = fb.GetPeoplePhoto(model.ID).ToList()[0].FileId;
                        }
                        //return RedirectToAction("List", "People", new { page = 1, pagesize = 12, area = "Works" }); 
                    }
                    else
                    {
                        ViewBag.Account = "0"; //账号已经存在，不保存
                    }
                    return View(model);
                    //return RedirectToAction("List", "People", new { page = 1, pagesize = 12, area = "Works" }); 
                }
                catch
                {
                    ViewBag.Result = "0";//新增失败
                    return View(model);
                    //return RedirectToAction("List", "People", new { page = 1, pagesize = 12, area = "Works" }); 
                }
            }
            else  //新增
            {

                ViewBag.Result = "0";
                DepartmentEntity dept = new DepartmentEntity();
                var user = OperatorProvider.Provider.Current();
                model.BZName = user.DeptName;
                model.BZCode = user.DeptCode;
                model.BZID = user.DeptId;
                if (user.DeptId != "0")
                {
                    string parentid = deptbll.GetEntity(user.DeptId).ParentId;  //当前所属部门的父节点id
                    dept = deptbll.GetEntity(parentid);
                    model.DeptId = dept.DepartmentId;
                    model.DeptCode = dept.EnCode;
                    model.DeptName = deptbll.GetEntity(parentid).FullName;
                }
                model.EntryDate = DateTime.Now.Date;
                model.Birthday = Convert.ToDateTime("1990-01-01");


                var total = 0;
                var data = new LllegalBLL().GetListNew("", 1, 12, out total);

                ViewData["LllEntity"] = data.ToList();
                ViewBag.pages = Math.Ceiling((decimal)total / 12);
                ViewBag.current = 1;
                ViewBag.pagesize = 12;
                ViewBag.mod = 2;
                return View(model);
            }

            //return RedirectToAction("Edit", new { id = model.ID });
        }

        void wc_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            //将同步结果写入日志文件
            string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
            System.IO.File.AppendAllText(HttpContext.Server.MapPath("~/logs/" + fileName), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：" + System.Text.Encoding.UTF8.GetString(e.Result) + "\r\n");
        }

        private FileInfoEntity BuildImage(string userid, string type)
        {
            FileInfoBLL fb = new FileInfoBLL();
            var flist = fb.GetPeoplePhoto(userid);
            foreach (FileInfoEntity f in flist)
            {
                fb.Delete(f.FileId);
            }
            var id = Guid.NewGuid().ToString();
            var encoder = new QRCodeEncoder();
            var image = encoder.Encode(userid + "|" + type, Encoding.UTF8);
            var path = "~/Resource/DocumentFile/";
            if (!Directory.Exists(Server.MapPath(path)))
                Directory.CreateDirectory(Server.MapPath(path));

            image.Save(Path.Combine(Server.MapPath(path), id + ".jpg"));

            var user = OperatorProvider.Provider.Current();



            return new FileInfoEntity() { FileId = id, CreateDate = DateTime.Now, CreateUserId = user.UserId, CreateUserName = user.UserName, Description = type, FileExtensions = ".jpg", FileName = id + ".jpg", FilePath = path + id + ".jpg", FileType = "jpg", ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.UserName, RecId = userid };
        }
        public ActionResult Edit(string id, string type, string tempImageSrc, PeopleEntity model)
        {

            string newKey = "!2#3@1YV";
            string newIV = "A~we!S6d";
            Security sec = new Security(newKey, newIV);
            //var current = OperatorProvider.Provider.Current();
            //var userAccount = (new UserBLL()).GetTrainUser(current.UserId);
            //string userAccount = Config.GetValue("userAccount");  //学员对应的培训平台账号
            var current = (new UserBLL()).GetEntity(id);
            string valCode = sec.Encrypt(current == null ? string.Empty : current.Account, newKey, newIV);
            PeopleEntity obj = peoplebll.GetEntity(id);
            ViewData["valCode"] = valCode;
            ViewData["EduPageUrl"] = Config.GetValue("EduPageUrl");
            ViewData["Roles"] = getRoles("", "");
            ViewData["Provins"] = getProvins(obj.Native);
            ViewBag.qr = "";
            if (obj != null)
            {
                ViewData["Job"] = getJob(model.JobName);
                if (obj.Quarters == "班长")
                {
                    ViewData["qddl"] = getQuarters("y", model.Quarters);

                }
                else
                {
                    ViewData["qddl"] = getQuarters("n", model.Quarters);
                }
            }
            ViewData["wkind"] = getWorkKind(obj.WorkKind);
            ViewData["minzu"] = getFolk(obj.Folk);
            FileInfoBLL fb = new FileInfoBLL();
            if (string.IsNullOrEmpty(type))   //修改保存
            {
                ViewBag.Show = "edit";
                ViewBag.imgsrc = model.Photo;
                ViewBag.Tel = "0";
                ViewBag.labour = "0";
                ViewBag.IdentityNo = "0";
                if (fb.GetPeoplePhoto(model.ID).Count() == 0)
                {
                    ViewBag.qr = "";
                }
                else
                {
                    ViewBag.qr = fb.GetPeoplePhoto(model.ID).ToList()[0].FileId;
                }
                if (string.IsNullOrEmpty(model.LinkWay.Trim()))
                {
                    ViewBag.Tel = "0";
                }
                else
                {
                    int n = peoplebll.CheckTel1(model.LinkWay.Trim(), model.ID);
                    ViewBag.Tel = n;
                    if (n != 0)
                    {
                        return View(model);
                    }
                }
                if (string.IsNullOrEmpty(model.LabourNo))
                {
                    ViewBag.labour = "0";
                }
                else
                {
                    int l = peoplebll.CheckNo(model.LabourNo, model.ID);
                    ViewBag.labour = l;
                    if (l != 0)
                    {
                        return View(model);
                    }
                }
                if (string.IsNullOrEmpty(model.IdentityNo))
                {
                    ViewBag.IdentityNo = "0";
                }
                else
                {
                    int l = peoplebll.CheckIdentity(model.IdentityNo, model.ID);
                    ViewBag.IdentityNo = l;
                    if (l != 0)
                    {
                        return View(model);
                    }
                }

                PeopleEntity p = peoplebll.GetEntity(model.ID);
                if (p != null)
                {
                    if (!string.IsNullOrEmpty(tempImageSrc))
                    {
                        model.Photo = tempImageSrc;
                    }
                    var files = fb.GetPeoplePhoto(id).ToList();
                    //insetbzz();

                    if (files.Count() == 0)  //file表中无二维码信息
                    {
                        var file = this.BuildImage(id, "人员");
                        model.Files.Add(file);
                    }
                    else
                    {
                        var file = fb.GetEntity(files[0].FileId);
                        string path = "";
                        if (file != null)
                        {
                            path = Server.MapPath(file.FilePath);
                            if (!System.IO.File.Exists(path)) //二维码文件不存在
                            {
                                var newfile = this.BuildImage(id, "人员");
                                model.Files.Add(newfile);
                            }
                        }

                    }
                    var dutyname = getNewData(current.UserId, "").Where(x => x.RoleId == model.RoleDutyId).FirstOrDefault();
                    if (dutyname != null) model.RoleDutyName = dutyname.FullName;
                    peoplebll.SaveForm(id, model);

                    if (fb.GetPeoplePhoto(model.ID).Count() == 0)
                    {
                        ViewBag.qr = "";
                    }
                    else
                    {
                        ViewBag.qr = fb.GetPeoplePhoto(model.ID).ToList()[0].FileId;
                    }
                    UserEntity user = userbll.GetEntity(id);
                    if (user != null)
                    {
                        user.RealName = model.Name;
                        if (model.Sex == "男")
                        {
                            user.Gender = 1;
                        }
                        else
                        {
                            user.Gender = 0;
                        }
                        user.Mobile = model.LinkWay;
                        user.DutyName = model.RoleDutyName;
                        user.DutyId = model.RoleDutyId;
                        user.IDENTIFYID = model.IdentityNo;
                        //先保存user信息，同步时重置roleid
                        var role1 = rbll.GetList().Where(x => x.FullName == "班组成员").FirstOrDefault();
                        var role2 = rbll.GetList().Where(x => x.FullName == "班组长").FirstOrDefault();
                        if (model.RoleDutyName == "班长" || model.RoleDutyName == "班组长")
                        {
                            user.RoleId = role2.RoleId;
                            user.RoleName = role2.FullName;
                        }
                        else
                        {
                            user.RoleId = role1.RoleId;
                            user.RoleName = role1.FullName;
                        }
                        user.IDENTIFYID = model.IdentityNo;
                        user.EnterTime = model.EntryDate;
                        user.Birthday = model.Birthday;
                        userbll.SaveForm(model.ID, user);

                        user.RoleId = model.RoleDutyId;
                        user.RoleName = model.RoleDutyName;


                        Erchtms e = new Erchtms();
                        userbll.SaveFormNew(id, user);

                        user.LabourNo = model.LabourNo;
                        user.Craft = model.WorkKind;
                        user.Folk = model.Folk;
                        user.CurrentWorkAge = model.CurrentWorkAge;
                        user.OldDegree = model.OldDegree;
                        user.NewDegree = model.NewDegree;
                        user.Quarters = model.Quarters;
                        user.Planer = model.Planer;
                        user.Visage = model.Visage;
                        user.Age = model.Age;
                        user.Native = model.Native;
                        user.TechnicalGrade = model.TecLevel;
                        user.JobName = model.JobName;
                        user.HealthStatus = model.HealthStatus;
                        user.IsSpecial = model.IsSpecial;
                        user.IsSpecialEqu = model.IsSpecialEquipment;
                        user.EnterTime = model.EntryDate;
                        user.HeadIcon = ToBase64String(Server.MapPath(model.Photo));
                        user.Secretkey = null;
                        user.Password = null;
                        //user.SignImg = ToBase64String("http://10.36.0.235/bzzd/Content/styles/static/images/tools///photo-7.png");
                        user.SignImg = null;
                        //current.Account = "System";
                        e.ErchtmsSynchronoous("SaveUser", user, current.Account);


                        //userbll.SaveFormNew(id, user);

                        //WebClient wc = new WebClient();
                        //wc.Credentials = CredentialCache.DefaultCredentials;
                        //try
                        //{
                        //    System.Collections.Specialized.NameValueCollection nc = new System.Collections.Specialized.NameValueCollection();
                        //    //当前操作用户账号
                        //    nc.Add("account", BSFramework.Application.Code.OperatorProvider.Provider.Current().Account);
                        //    //用户信息
                        //    nc.Add("json", Newtonsoft.Json.JsonConvert.SerializeObject(user));
                        //    wc.UploadValuesCompleted += wc_UploadValuesCompleted;
                        //    wc.UploadValuesAsync(new Uri(Config.GetValue("SyncUrl") + "SaveUser?keyValue=" + user.UserId), nc);

                        //}
                        //catch (Exception ex)
                        //{
                        //    //将同步结果写入日志文件
                        //    string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
                        //    System.IO.File.AppendAllText(HttpContext.Server.MapPath("~/logs/" + fileName), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：同步数据失败，同步信息" + Newtonsoft.Json.JsonConvert.SerializeObject(user) + ",异常信息：" + ex.Message + "\r\n");
                        //}
                    }
                    return RedirectToAction("List", "People", new { page = 1, pagesize = 12, area = "Works" });  //跳转列表
                }
                else
                {

                    return View(model);
                }

            }
            else //显示数据
            {
                ViewBag.Result = "0";
                ViewBag.Show = "init";
                if (type == "initobj")
                {
                    ViewBag.Show = "init";
                }
                if (type == "editobj")
                {
                    ViewBag.Show = "edit";
                }
                ViewBag.Tel = "0";
                ViewBag.labour = "0";
                model = peoplebll.GetEntity(id);
                model.EntryDate = model.EntryDate.Date;
                //var dutyname = getNewData("bd5a1c47-7778-4a15-a61c-0ceee5b3b812", "").Where(x => x.RoleId == model.RoleDutyId).FirstOrDefault();
                //if (dutyname != null) model.RoleDutyName = dutyname.FullName;
                ViewBag.Id = model.ID;
                if (fb.GetPeoplePhoto(model.ID).Count() == 0)
                {
                    ViewBag.qr = "";
                }
                else
                {
                    ViewBag.qr = fb.GetPeoplePhoto(model.ID).ToList()[0].FileId;
                }
                ViewBag.imgsrc = model.Photo;
                //if (model.Quarters == "班长")
                //{
                //    ViewData["qddl"] = getQuarters("y", model.Quarters);
                //}
                //else
                //{
                //    ViewData["qddl"] = getQuarters("n", model.Quarters);
                //}
                if (!string.IsNullOrEmpty(model.IdentityNo))
                {
                    ViewBag.Pwd = model.IdentityNo.Substring(model.IdentityNo.Length - 4, 4);
                }
                else
                {
                    ViewBag.Pwd = "";
                }
                var total = 0;
                var data = new LllegalBLL().GetListNew(model.ID, 1, 12, out total);
                //model.DeptName = model.DeptName + " / " + model.BZName;
                ViewData["LllEntity"] = data.ToList();
                ViewBag.pages = Math.Ceiling((decimal)total / 12);
                ViewBag.current = 1;
                ViewBag.pagesize = 12;
                ViewBag.mod = 2;
                return View(model);
            }

        }



        public ActionResult NewEdit(string id, string type, string tempImageSrc, PeopleEntity model)
        {
            ViewData["Roles"] = getRoles("", "");
            PeopleEntity obj = peoplebll.GetEntity(id);
            if (obj != null)
            {
                ViewData["Job"] = getJob(model.JobName);
                if (obj.Quarters == "班长")
                {
                    ViewData["qddl"] = getQuarters("y", model.Quarters);
                }
                else
                {
                    ViewData["qddl"] = getQuarters("n", model.Quarters);
                }
            }
            ViewData["minzu"] = getFolk(obj.Folk);
            ViewData["wkind"] = getWorkKind(obj.WorkKind);
            ViewData["Provins"] = getProvins(obj.Native);
            FileInfoBLL fb = new FileInfoBLL();
            if (string.IsNullOrEmpty(type))   //修改保存
            {
                ViewBag.Show = "edit";
                ViewBag.imgsrc = model.Photo;
                ViewBag.Tel = "0";
                ViewBag.labour = "0";
                ViewBag.IdentityNo = "0";
                if (fb.GetPeoplePhoto(model.ID).Count() == 0)
                {
                    ViewBag.qr = "";
                }
                else
                {
                    ViewBag.qr = fb.GetPeoplePhoto(model.ID).ToList()[0].FileId;
                }
                if (string.IsNullOrEmpty(model.LinkWay.Trim()))
                {
                    ViewBag.Tel = "0";
                }
                else
                {
                    int n = peoplebll.CheckTel1(model.LinkWay.Trim(), model.ID);
                    ViewBag.Tel = n;
                    if (n != 0)
                    {
                        return View(model);
                    }
                }
                if (string.IsNullOrEmpty(model.LabourNo))
                {
                    ViewBag.labour = "0";
                }
                else
                {
                    int l = peoplebll.CheckNo(model.LabourNo, model.ID);
                    ViewBag.labour = l;
                    if (l != 0)
                    {
                        return View(model);
                    }
                }
                if (string.IsNullOrEmpty(model.IdentityNo))
                {
                    ViewBag.IdentityNo = "0";
                }
                else
                {
                    int l = peoplebll.CheckIdentity(model.IdentityNo, model.ID);
                    ViewBag.IdentityNo = l;
                    if (l != 0)
                    {
                        return View(model);
                    }
                }

                PeopleEntity p = peoplebll.GetEntity(model.ID);
                if (p != null)
                {
                    if (!string.IsNullOrEmpty(tempImageSrc))
                    {
                        model.Photo = tempImageSrc;
                    }
                    var files = fb.GetPeoplePhoto(id).ToList();
                    //insetbzz();

                    if (files.Count() == 0)  //file表中无二维码信息
                    {
                        var file = this.BuildImage(id, "人员");
                        model.Files.Add(file);
                    }
                    else
                    {
                        var file = fb.GetEntity(files[0].FileId);
                        string path = "";
                        if (file != null)
                        {
                            path = Server.MapPath(file.FilePath);
                            if (!System.IO.File.Exists(path))
                            {
                                var newfile = this.BuildImage(id, "人员");
                                model.Files.Add(newfile);
                            }
                        }

                    }
                    var dutyname = getNewData(p.ID, "").Where(x => x.RoleId == model.RoleDutyId).FirstOrDefault();
                    if (dutyname != null) model.RoleDutyName = dutyname.FullName;
                    peoplebll.SaveForm(id, model);
                    ViewBag.Result = "1";
                    if (fb.GetPeoplePhoto(model.ID).Count() == 0)
                    {
                        ViewBag.qr = "";
                    }
                    else
                    {
                        ViewBag.qr = fb.GetPeoplePhoto(model.ID).ToList()[0].FileId;
                    }
                    UserEntity user = userbll.GetEntity(id);
                    if (user != null)
                    {
                        user.RealName = model.Name;
                        if (model.Sex == "男")
                        {
                            user.Gender = 1;
                        }
                        else
                        {
                            user.Gender = 0;
                        }
                        //user.Account = model.LinkWay;
                        user.Mobile = model.LinkWay;
                        user.IDENTIFYID = model.IdentityNo;
                        user.DutyName = model.RoleDutyName;
                        user.DutyId = model.RoleDutyId;
                        var role1 = rbll.GetList().Where(x => x.FullName == "班组成员").FirstOrDefault();
                        var role2 = rbll.GetList().Where(x => x.FullName == "班组长").FirstOrDefault();
                        if (model.RoleDutyName == "班长" || model.RoleDutyName == "班组长")
                        {
                            user.RoleId = role2.RoleId;
                            user.RoleName = role2.FullName;
                        }
                        else
                        {
                            user.RoleId = role1.RoleId;
                            user.RoleName = role1.FullName;
                        }
                        userbll.SaveForm(id, user);
                        user.RoleId = model.RoleDutyId;
                        user.RoleName = model.RoleDutyName;
                        Erchtms e = new Erchtms();
                        user.Birthday = model.Birthday;
                        user.LabourNo = model.LabourNo;
                        user.Craft = model.WorkKind;
                        user.Folk = model.Folk;
                        user.CurrentWorkAge = model.CurrentWorkAge;
                        user.OldDegree = model.OldDegree;
                        user.NewDegree = model.NewDegree;
                        user.Quarters = model.Quarters;
                        user.Planer = model.Planer;
                        user.Visage = model.Visage;
                        user.Age = model.Age;
                        user.Native = model.Native;
                        user.TechnicalGrade = model.TecLevel;
                        user.JobName = model.JobName;
                        user.HealthStatus = model.HealthStatus;
                        user.IsSpecial = model.IsSpecial;
                        user.IsSpecialEqu = model.IsSpecialEquipment;
                        user.EnterTime = model.EntryDate;
                        user.HeadIcon = ToBase64String(Server.MapPath(model.Photo));
                        user.Secretkey = null;
                        user.Password = null;
                        user.SignImg = null;
                        e.ErchtmsSynchronoous("SaveUser", user, user.Account);


                    }
                    return View(model);
                }
                else
                {

                    return View(model);
                }

            }
            else //显示数据
            {
                ViewBag.Result = "0";
                ViewBag.Show = "init";
                if (type == "initobj")
                {
                    ViewBag.Show = "init";
                }
                if (type == "editobj")
                {
                    ViewBag.Show = "edit";
                }
                ViewBag.Tel = "0";
                ViewBag.labour = "0";
                model = peoplebll.GetEntity(id);
                model.EntryDate = model.EntryDate == null ? DateTime.Now : model.EntryDate;
                ViewBag.Id = model.ID;
                if (fb.GetPeoplePhoto(model.ID).Count() == 0)
                {
                    ViewBag.qr = "";
                }
                else
                {
                    ViewBag.qr = fb.GetPeoplePhoto(model.ID).ToList()[0].FileId;
                }
                ViewBag.imgsrc = model.Photo;
                //if (model.Quarters == "班长")
                //{
                //    ViewData["qddl"] = getQuarters("y", model.Quarters);
                //}
                //else
                //{
                //    ViewData["qddl"] = getQuarters("n", model.Quarters);
                //}
                if (!string.IsNullOrEmpty(model.IdentityNo))
                {
                    ViewBag.Pwd = model.IdentityNo.Substring(model.IdentityNo.Length - 4, 4);
                }
                else
                {
                    ViewBag.Pwd = "";
                }
                var total = 0;
                var data = new LllegalBLL().GetListNew(model.ID, 1, 12, out total);

                ViewData["LllEntity"] = data.ToList();
                ViewBag.pages = Math.Ceiling((decimal)total / 12);
                ViewBag.current = 1;
                ViewBag.pagesize = 12;
                ViewBag.mod = 2;
                return View(model);
            }

        }
        /// <summary>
        /// 表单页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangeForm(string id, string pwd, string npwd)
        {
            PeopleEntity people = peoplebll.GetEntity(id);
            if (people != null)
            {    //直接读的user表数据，此数据在people表中暂无
                people.PassWord = pwd;
                peoplebll.SaveForm(id, people);
            }
            //修改user表

            userbll.RevisePassword(id, pwd);
            Erchtms e = new Erchtms();
            string[] strList = new string[] { id, npwd };
            e.ErchtmsSynchronoous("UpdatePwd", strList, "");
            return Success("修改成功");
        }
        public ActionResult ChangeFormNew(string id, string pwd, string opwd, string useraccount, string rpwd)
        {
            UserInfoEntity uent = new UserInfoEntity();
            PeopleEntity people = peoplebll.GetEntity(id);
            if (people != null)
            {
                //直接读的user表数据，此数据在people表中暂无
                people.PassWord = pwd;
                peoplebll.SaveForm(id, people);
            }
            //修改user表
            uent = userbll.CheckLoginNew(useraccount, opwd);
            if (uent != null)
            {
                userbll.RevisePassword(id, pwd);
                Erchtms e = new Erchtms();
                string[] strList = new string[] { id, rpwd };
                e.ErchtmsSynchronoous("UpdatePwd", strList, "");
                return Success("修改成功", new { type = "0" });
            }
            else
            {
                return Success("修改失败", new { type = "1" });
            }

        }


        public ActionResult Reset(string id, string pwd, string npwd)
        {
            PeopleEntity model = peoplebll.GetEntity(id);
            model.PassWord = pwd;
            peoplebll.SaveForm(id, model);

            userbll.RevisePassword(id, pwd);
            Erchtms e = new Erchtms();
            string[] strList = new string[] { id, npwd };
            e.ErchtmsSynchronoous("UpdatePwd", strList, model.UserAccount);

            return Success("重置成功");
        }
        /// <summary>
        /// 上传头像
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadFile()
        {
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //没有文件上传，直接返回
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }
            string FileEextension = Path.GetExtension(files[0].FileName);
            string UserId = OperatorProvider.Provider.Current().UserId;
            UserId = Guid.NewGuid().ToString();
            string virtualPath = string.Format("/Resource/PhotoFile/{0}{1}", UserId, FileEextension);
            string fullFileName = Server.MapPath("~" + virtualPath);
            //创建文件夹，保存文件
            string path = Path.GetDirectoryName(fullFileName);
            Directory.CreateDirectory(path);
            files[0].SaveAs(fullFileName);

            //PeopleEntity userEntity = new PeopleEntity();
            //userEntity.ID = OperatorProvider.Provider.Current().UserId;
            //userEntity.Photo = virtualPath;
            //peoplebll.SaveForm(userEntity.ID, userEntity);
            return Success("上传成功。", virtualPath);
        }

        public static SelectList getJob(string job)
        {
            DataItemBLL dataItemBLL = new DataItemBLL();
            DataItemDetailBLL dataItemDetailBLL = new DataItemDetailBLL();
            JobBLL jbll = new JobBLL();
            List<SelectListItem> items = new List<SelectListItem>();

            var user = OperatorProvider.Provider.Current();
            var data = jbll.GetJobList(user.OrganizeId);
            foreach (RoleEntity d in data)
            {
                items.Add(new SelectListItem() { Value = d.FullName, Text = d.FullName });
            }
            items.Insert(0, new SelectListItem() { Value = "请选择", Text = "请选择" });
            SelectList joblist = new SelectList(items, "Value", "Text", job);

            return joblist;
        }
        /// <summary>
        /// 工种编码
        /// </summary>
        /// <param name="workkind"></param>
        /// <returns></returns>
        public static SelectList getWorkKind(string workkind)
        {
            DataItemBLL ditem = new DataItemBLL();
            DataItemDetailBLL detail = new DataItemDetailBLL();
            List<DataItemDetailEntity> flist = new List<DataItemDetailEntity>();
            var folks = ditem.GetEntityByCode("PersonWrokType");
            if (folks != null)
            {
                flist = detail.GetList(folks.ItemId).ToList();
            }
            List<SelectListItem> items = new List<SelectListItem>();

            foreach (DataItemDetailEntity d in flist)
            {
                items.Add(new SelectListItem() { Value = d.ItemName, Text = d.ItemName });
            }
            SelectList joblist = new SelectList(items, "Value", "Text", workkind);
            return joblist;
        }
        /// <summary>
        /// 民族编码
        /// </summary>
        /// <param name="folk"></param>
        /// <returns></returns>
        public static SelectList getFolk(string folk)
        {
            DataItemBLL ditem = new DataItemBLL();
            DataItemDetailBLL detail = new DataItemDetailBLL();
            List<DataItemDetailEntity> flist = new List<DataItemDetailEntity>();
            var folks = ditem.GetEntityByCode("Nation");
            if (folks != null)
            {
                flist = detail.GetList(folks.ItemId).ToList();
            }
            List<SelectListItem> items = new List<SelectListItem>();

            foreach (DataItemDetailEntity d in flist)
            {
                items.Add(new SelectListItem() { Value = d.ItemName, Text = d.ItemName });
            }
            SelectList joblist = new SelectList(items, "Value", "Text", folk);
            return joblist;
        }
        public SelectList getRoles(string userid, string deptid)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            var user = OperatorProvider.Provider.Current();
            var data = getNewData(user.UserId, user.DeptId);
            foreach (RoleEntity d in data)
            {
                items.Add(new SelectListItem() { Value = d.RoleId, Text = d.FullName });
            }
            SelectList quarterlist = new SelectList(items, "Value", "Text");
            return quarterlist;
        }


        public SelectList getProvins(string provins)
        {

            List<SelectListItem> items = new List<SelectListItem>();

            var list = areaBLL.GetAreaList("0");
            foreach (AreaEntity a in list)
            {
                items.Add(new SelectListItem() { Value = a.AreaName, Text = a.AreaName });
            }

            SelectList quarterlist = new SelectList(items, "Value", "Text", provins);
            return quarterlist;


        }
        public static string getplaner(string quarter)
        {
            string val = "";
            PostBLL pbll = new PostBLL();
            JobBLL jbll = new JobBLL();


            var user = OperatorProvider.Provider.Current();
            var data = pbll.GetQuartersList(user.OrganizeId);
            var role = data.Where(x => x.FullName == quarter).FirstOrDefault();
            if (role != null) val = role.EnCode;
            return val;
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public ActionResult GetFormJson(string keyValue)
        {
            var data = peoplebll.GetEntity(keyValue);
            return ToJsonResult(data);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>

        [HandlerMonitor(6, "删除班组成员")]
        public ActionResult RemoveForm(string keyValue)
        {
            Erchtms e = new Erchtms();
            UserEntity user = userbll.GetEntity(keyValue);
            e.ErchtmsSynchronoous("DeleteUser", user, user.Account);


            peoplebll.RemoveForm(keyValue);
            userbll.RemoveForm(keyValue);

            return Success("删除成功。");
        }
        /// <summary>
        /// 保存（新增、修改）
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, PeopleEntity entity)
        {
            peoplebll.SaveForm(keyValue, entity);
            //  Erchtms e = new Erchtms();
            // e.ErchtmsSynchronoous("SaveUser", entity);
            return Success("操作成功。");
        }

        /// <summary>
        /// 更新用户指纹
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="finger"></param>
        /// <returns></returns>
        [AjaxOnly]
        [HttpPost]
        public ActionResult UpdateUserFinger(string userId, string finger)
        {
            //更新用户指纹
            UserEntity userEntity = userbll.GetEntity(userId);
            userEntity.FINGER = finger;
            userbll.UpdateUserInfo(userId, userEntity);


            return Success("操作成功。");
        }

        public string ToBase64String(string path)
        {
            try
            {
                string imgStr = "";
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 20480, true);
                byte[] imgb = new byte[fs.Length];
                fs.Read(imgb, 0, (int)fs.Length);
                imgStr = Convert.ToBase64String(imgb);
                fs.Close();
                return imgStr;
            }
            catch
            {
                return null;
            }
        }

    }
}
