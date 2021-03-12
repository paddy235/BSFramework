using Aspose.Cells;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.BudgetAbout;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.BudgetAbout;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Web.Areas.Works.Models;
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
    public class CostController : MvcControllerBase
    {
        //
        // GET: /Works/Budget/

        public ViewResult Index()
        {
            return View();
        }

        public JsonResult GetData(FormCollection fc)
        {
            var year = fc.Get("year");
            var month = fc.Get("month");
            var category = fc.Get("category");
            var deptname = fc.Get("dept");
            var page = int.Parse(fc.Get("page") ?? "1");
            var rows = int.Parse(fc.Get("rows") ?? "20");
            var costbll = new CostBLL();
            var user = OperatorProvider.Provider.Current();
            var total = 0;
            var costdept = Config.GetValue("CostDept");

            var deptbll = new DepartmentBLL();
            var dept = new DepartmentBLL().GetEntity(user.DeptId);
            var depts = default(List<DepartmentEntity>);
            if (dept == null || dept.FullName == costdept)
            {
                dept = deptbll.GetRootDepartment();
                depts = deptbll.GetSubDepartments(dept.DepartmentId, null);
            }
            else
            {
                depts = deptbll.GetSubDepartments(dept.DepartmentId, null);
                depts.Add(dept);
            }
            var data = costbll.GetCostRecords(page, rows, out total, depts == null ? null : depts.Select(x => x.DepartmentId).ToArray(), year, month, category, deptname);
            var filebll = new FileInfoBLL();
            foreach (var item in data)
            {
                var files = filebll.GetFileList(item.RecordId.ToString());
                if (files.Count > 0)
                {
                    item.FilePath = Url.Action("DownloadFile", new { area = "PublicInfoManage", controller = "ResourceFile", keyValue = files[0].FileId, filename = files[0].FileName, recId = files[0].RecId });
                    item.FileName = files[0].FileName;
                }
            }
            return Json(new { rows = data, records = total, total = Math.Ceiling((double)total / rows), page });
        }

        [HttpPost]
        public JsonResult DoImport(CostRecordEntity record)
        {
            var success = true;
            var message = "保存成功！";

            var user = OperatorProvider.Provider.Current();

            if (this.Request.Files.Count > 0)
            {
                var book = default(Workbook);
                try
                {
                    book = new Workbook(this.Request.Files[0].InputStream);
                }
                catch (Exception e)
                {
                    return Json(new AjaxResult() { type = ResultType.error, message = "无法识别的文件！" });
                }

                try
                {
                    var sheet = book.Worksheets[0];
                    var titleIndex = this.GetTitleRow(sheet, record.Category);
                    var dataindex = this.GetDataIndex(sheet, titleIndex);

                    var costbll = new CostBLL();
                    var costitems = this.GetCostData(sheet, titleIndex, dataindex);
                    record.CostItems = costitems;
                    record.RecordId = Guid.NewGuid();
                    costbll.AddCost(record);
                    var filename = this.Request.Files[0].FileName;
                    var path = Path.Combine(Server.MapPath("~/Resource"), "Budget");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    var id = Guid.NewGuid().ToString();
                    this.Request.Files[0].SaveAs(Path.Combine(path, id + Path.GetExtension(filename)));
                    var fileinfo = new FileInfoEntity()
                    {
                        FileId = id,
                        FilePath = "~/Resource/Budget/" + id + Path.GetExtension(filename),
                        FileName = filename,
                        CreateDate = DateTime.Now,
                        CreateUserId = user.UserId,
                        CreateUserName = user.UserName,
                        DeleteMark = 0,
                        EnabledMark = 1,
                        FileExtensions = Path.GetExtension(filename),
                        FileSize = this.Request.Files[0].ContentLength.ToString(),
                        ModifyDate = DateTime.Now,
                        ModifyUserId = user.UserId,
                        ModifyUserName = user.UserName,
                        RecId = record.RecordId.ToString()
                    };
                    var filebll = new FileInfoBLL();
                    filebll.SaveForm(null, fileinfo);
                }
                catch (Exception e)
                {
                    success = false;
                    message = e.Message;
                }

            }

            return Json(new AjaxResult() { type = success ? ResultType.success : ResultType.error, message = message });
        }

        private List<CostItemEntity> GetCostData(Worksheet sheet, int titleIndex, Dictionary<string, int> dataindex)
        {
            var result = new List<CostItemEntity>();

            var user = OperatorProvider.Provider.Current();
            var costdept = Config.GetValue("CostDept");

            var deptbll = new DepartmentBLL();
            var dept = new DepartmentBLL().GetEntity(user.DeptId);
            var depts = default(List<DepartmentEntity>);
            if (dept.FullName == costdept)
            {
                dept = deptbll.GetRootDepartment();
            }
            depts = deptbll.GetSubDepartments(dept.DepartmentId, "班组");

            for (int i = titleIndex + 1; i <= sheet.Cells.MaxDataRow; i++)
            {
                var costitem = new CostItemEntity()
                {
                    CostItemId = Guid.NewGuid(),
                    DeptName = sheet.Cells[i, dataindex["dept"]].StringValue,
                    Amount = (decimal)sheet.Cells[i, dataindex["amount"]].DoubleValue
                };

                var currentdept = depts.Find(x => x.FullName == costitem.DeptName);
                if (currentdept == null) throw new Exception(string.Format("行 {0} 单位错误！", (i + 1)));
                else costitem.DeptId = currentdept.DepartmentId;

                result.Add(costitem);
            }

            return result;
        }

        private Dictionary<string, int> GetDataIndex(Worksheet sheet, int titleIndex)
        {
            var result = new Dictionary<string, int>();
            for (int j = 1; j <= sheet.Cells.MaxDataColumn; j++)
            {
                if (sheet.Cells[titleIndex, j].StringValue == "班组")
                {
                    result.Add("dept", j);
                    continue;
                }
                else if (sheet.Cells[titleIndex, j].StringValue == "金额")
                    result.Add("amount", j);
            }

            if (result.Count != 2)
                throw new Exception("无法识别文件！");

            return result;
        }

        private int GetTitleRow(Worksheet sheet, string category)
        {
            for (int i = 0; i <= sheet.Cells.MaxDataRow; i++)
            {
                if (category == "材料费" && sheet.Cells[i, 0].StringValue == "任务名称")
                    return i;
                else if (category == "修理费" && sheet.Cells[i, 0].StringValue == "项目名称")
                    return i;
            }

            throw new Exception("无法识别文件！");
        }

        public ViewResult Detail(string id)
        {
            var categories = new List<SelectListItem>() { new SelectListItem() { Value = "材料费", Text = "材料费" }, new SelectListItem() { Value = "修理费", Text = "修理费" } };
            ViewData["categories"] = categories;

            var user = OperatorProvider.Provider.Current();
            var deptbll = new DepartmentBLL();
            var dept = deptbll.GetEntity(user.DeptId);
            if (dept == null) dept = deptbll.GetRootDepartment();
            var subdepts = deptbll.GetChildDepartments(user.DeptId);
            var depts = subdepts.OrderBy(x => x.EnCode).Select(x => new SelectListItem() { Value = x.DepartmentId, Text = x.FullName }).ToList();
            ViewData["depts"] = depts;

            ViewBag.id = id;
            var itemdetialbll = new DataItemDetailBLL();
            var itembll = new DataItemBLL();
            var main = itembll.GetEntityByName("费用专业类型");
            var content = itemdetialbll.GetList(main.ItemId).ToList();
            var professionals = content.Select(x => new SelectListItem() { Value = x.ItemValue, Text = x.ItemName }).ToList();
            professionals.Insert(0, new SelectListItem() { Value = "", Text = "==请选择==" });
            ViewData["professionals"] = professionals;

            var now = DateTime.Now;
            var record = new CostRecordEntity()
            {
                RecordMonth = new DateTime(now.Year, now.Month, 1),
                RecordUserId = user.UserId,
                RecordUser = user.UserName,
                RecordDeptId = dept.DepartmentId,
                RecordDept = dept.FullName,
                RecordTime = DateTime.Now
            };

            return View(record);
        }

        public JsonResult Remove(string id)
        {
            var success = true;
            var message = "保存成功！";
            try
            {
                var costbll = new CostBLL();
                costbll.Remove(id);

                var filebll = new FileInfoBLL();
                var files = filebll.DeleteByRecId(id);
                foreach (var item in files)
                {
                    System.IO.File.Delete(Server.MapPath(item.FilePath));
                }
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
            }
            return Json(new AjaxResult() { type = success ? ResultType.success : ResultType.error, message = message });
        }

        public ViewResult Summary()
        {
            var user = OperatorProvider.Provider.Current();
            var costdept = Config.GetValue("CostDept");

            var deptbll = new DepartmentBLL();
            var dept = new DepartmentBLL().GetEntity(user.DeptId);
            var depts = default(List<DepartmentEntity>);
            if (dept == null)
            {
                dept = deptbll.GetRootDepartment();
                depts = deptbll.GetSubDepartments(dept.DepartmentId, "班组");
            }
            else
            {
                if (dept.FullName == costdept)
                {
                    dept = deptbll.GetRootDepartment();
                    depts = deptbll.GetSubDepartments(dept.DepartmentId, "班组");
                }
                else
                {
                    depts = deptbll.GetSubDepartments(dept.DepartmentId, "班组");
                    depts.Add(dept);
                }
            }
            ViewBag.deptid = dept.DepartmentId;
            var list = depts.OrderBy(x => x.EnCode).Select(x => new SelectListItem() { Value = x.DepartmentId, Text = x.FullName }).ToList();
            list.Insert(0, new SelectListItem() { Text = "请选择", Value = string.Empty });
            ViewData["dept"] = list;

            return View();
        }

        public JsonResult GetSummaryData(FormCollection fc)
        {
            var year = int.Parse(fc.Get("year"));
            var deptid = fc.Get("deptid");

            var deptbll = new DepartmentBLL();
            var teams = deptbll.GetSubDepartments(deptid, "班组");

            var databll = new DataItemDetailBLL();
            var dataitems = databll.GetDataItems("费用类型");

            var budgetbll = new BudgetBLL();
            var budget = budgetbll.GetBudgetSummary(year, teams.Select(x => x.DepartmentId).ToArray());
            var costbll = new CostBLL();
            var cost = costbll.GetCostSummary(year, teams.Select(x => x.DepartmentId).ToArray());
            var result = new List<BudgetSummaryModel>();
            result.AddRange(dataitems.GroupJoin(budget, x => x.ItemValue, y => y.Category, (x, y) => new BudgetSummaryModel { Category = x.ItemValue + "预算", Data = new decimal[] { y.Sum(z => z.Budget1), y.Sum(z => z.Budget2), y.Sum(z => z.Budget3), y.Sum(z => z.Budget4), y.Sum(z => z.Budget5), y.Sum(z => z.Budget6), y.Sum(z => z.Budget7), y.Sum(z => z.Budget8), y.Sum(z => z.Budget9), y.Sum(z => z.Budget10), y.Sum(z => z.Budget11), y.Sum(z => z.Budget12), } }));
            result.AddRange(dataitems.GroupJoin(cost, x => x.ItemValue, y => y.Category, (x, y) => new BudgetSummaryModel { Category = x.ItemValue + "支出", Data = new decimal[] { y.Where(z => z.Month == 1).Sum(z => z.Amount), y.Where(z => z.Month == 2).Sum(z => z.Amount), y.Where(z => z.Month == 3).Sum(z => z.Amount), y.Where(z => z.Month == 4).Sum(z => z.Amount), y.Where(z => z.Month == 5).Sum(z => z.Amount), y.Where(z => z.Month == 6).Sum(z => z.Amount), y.Where(z => z.Month == 7).Sum(z => z.Amount), y.Where(z => z.Month == 8).Sum(z => z.Amount), y.Where(z => z.Month == 9).Sum(z => z.Amount), y.Where(z => z.Month == 10).Sum(z => z.Amount), y.Where(z => z.Month == 11).Sum(z => z.Amount), y.Where(z => z.Month == 12).Sum(z => z.Amount), } }));

            result = result.OrderBy(x => x.Category).ToList();

            return Json(result);
        }
    }
}
