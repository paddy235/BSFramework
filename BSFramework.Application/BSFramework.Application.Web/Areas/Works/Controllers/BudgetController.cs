using Aspose.Cells;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.BudgetAbout;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.BudgetAbout;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    public class BudgetController : MvcControllerBase
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
            var budgetbll = new BudgetBLL();
            var user = OperatorProvider.Provider.Current();

            var costdept = Config.GetValue("CostDept");

            var deptbll = new DepartmentBLL();
            var dept = new DepartmentBLL().GetEntity(user.DeptId);
            var depts = default(List<DepartmentEntity>);
            if (dept == null || dept.FullName == costdept)
            {
                dept = deptbll.GetRootDepartment();
            }
            depts = deptbll.GetSubDepartments(dept.DepartmentId, "班组");
            var data = budgetbll.GetYearBudgets(depts == null ? null : depts.Select(x => x.DepartmentId).ToArray(), year);
            return Json(data);
        }

        public ViewResult Import()
        {
            return View();
        }

        [HttpPost]
        public JsonResult DoImport()
        {
            var success = true;
            var message = "保存成功！";

            if (this.Request.Files.Count > 0)
            {
                var book = new Workbook(this.Request.Files[0].InputStream);
                var sheet = book.Worksheets[0];

                var budgetbll = new BudgetBLL();
                try
                {
                    var titleIndex = this.GetTitleRow(sheet);
                    var dataindex = this.GetDataIndex(sheet, titleIndex);

                    var budgetdata = this.GetBudgetData(sheet, titleIndex, dataindex);
                    budgetbll.AddBudgets(budgetdata);
                }
                catch (Exception e)
                {
                    success = false;
                    message = e.Message;
                }

            }

            return Json(new AjaxResult() { type = success ? ResultType.success : ResultType.error, message = message });
        }

        private List<BudgetEntity> GetBudgetData(Worksheet sheet, int titleIndex, Dictionary<int, int> dataindex)
        {
            var result = new List<BudgetEntity>();

            var user = OperatorProvider.Provider.Current();
            var costdept = Config.GetValue("CostDept");

            var deptbll = new DepartmentBLL();
            var dept = new DepartmentBLL().GetEntity(user.DeptId);
            var depts = default(List<DepartmentEntity>);
            if (dept == null || dept.FullName == costdept)
            {
                dept = deptbll.GetRootDepartment();
            }
            depts = deptbll.GetSubDepartments(dept.DepartmentId, "班组");

            var budgettype = new string[] { "材料费", "修理费" };
            for (int i = titleIndex + 1; i <= sheet.Cells.MaxDataRow; i++)
            {
                var budget = new BudgetEntity()
                {
                    BudgetId = Guid.NewGuid(),
                    Year = sheet.Cells[i, 0].StringValue.Trim(),
                    DeptName = sheet.Cells[i, 2].StringValue,
                    Category = sheet.Cells[i, 1].StringValue
                };

                try
                {
                    budget.Budget1 = (decimal)sheet.Cells[i, dataindex[1]].DoubleValue;
                    budget.Budget2 = (decimal)sheet.Cells[i, dataindex[2]].DoubleValue;
                    budget.Budget3 = (decimal)sheet.Cells[i, dataindex[3]].DoubleValue;
                    budget.Budget4 = (decimal)sheet.Cells[i, dataindex[4]].DoubleValue;
                    budget.Budget5 = (decimal)sheet.Cells[i, dataindex[5]].DoubleValue;
                    budget.Budget6 = (decimal)sheet.Cells[i, dataindex[6]].DoubleValue;
                    budget.Budget7 = (decimal)sheet.Cells[i, dataindex[7]].DoubleValue;
                    budget.Budget8 = (decimal)sheet.Cells[i, dataindex[8]].DoubleValue;
                    budget.Budget9 = (decimal)sheet.Cells[i, dataindex[9]].DoubleValue;
                    budget.Budget10 = (decimal)sheet.Cells[i, dataindex[10]].DoubleValue;
                    budget.Budget11 = (decimal)sheet.Cells[i, dataindex[11]].DoubleValue;
                    budget.Budget12 = (decimal)sheet.Cells[i, dataindex[12]].DoubleValue;
                }
                catch (Exception)
                {
                    throw new Exception(string.Format("行 {0} 预算请输入数值！", (1 + i)));
                }


                if (!budgettype.Contains(budget.Category)) throw new Exception(string.Format("行 {0} 费用类型错误！", (i + 1)));
                var currentdept = depts.Find(x => x.FullName == budget.DeptName);
                if (currentdept == null) throw new Exception(string.Format("行 {0} 单位错误！", (i + 1)));
                else budget.DeptId = currentdept.DepartmentId;

                result.Add(budget);
            }

            return result;
        }

        private Dictionary<int, int> GetDataIndex(Worksheet sheet, int titleIndex)
        {
            var result = new Dictionary<int, int>();
            for (int i = 1; i <= 12; i++)
            {
                for (int j = 1; j <= sheet.Cells.MaxDataColumn; j++)
                {
                    if (sheet.Cells[titleIndex, j].StringValue == (i + "月份"))
                    {
                        result.Add(i, j);
                        break;
                    }
                }
            }

            if (result.Count != 12)
                throw new Exception("无法识别文件！");

            return result;
        }

        private int GetTitleRow(Worksheet sheet)
        {
            for (int i = 0; i <= sheet.Cells.MaxDataRow; i++)
            {
                if (sheet.Cells[i, 0].StringValue == "年度") return i;
            }

            throw new Exception("无法识别文件！");
        }

        public ViewResult Detail(string id)
        {
            var categories = new List<SelectListItem>() { new SelectListItem() { Value = "材料费", Text = "材料费" }, new SelectListItem() { Value = "修理费", Text = "修理费" } };
            ViewData["categories"] = categories;

            var year = DateTime.Now.Year;
            var years = new List<SelectListItem>();
            for (int i = year; i <= year + 5; i++)
            {
                years.Add(new SelectListItem() { Value = i.ToString(), Text = i.ToString() });
            }
            ViewData["years"] = years;

            var user = OperatorProvider.Provider.Current();
            var costdept = Config.GetValue("CostDept");

            var deptbll = new DepartmentBLL();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            var subdepts = default(List<DepartmentEntity>);
            if (dept.FullName == costdept)
            {
                dept = deptbll.GetRootDepartment();
            }
            subdepts = deptbll.GetSubDepartments(dept.DepartmentId, "班组");
            var depts = subdepts.OrderBy(x => x.EnCode).Select(x => new SelectListItem() { Value = x.DepartmentId, Text = x.FullName }).ToList();
            ViewData["depts"] = depts;

            ViewBag.id = id;

            var budget = default(BudgetEntity);
            if (string.IsNullOrEmpty(id))
            {
                budget = new BudgetEntity()
                {
                    Year = DateTime.Now.Year.ToString()
                };
            }
            else
            {
                var budgetbll = new BudgetBLL();
                budget = budgetbll.GetDetail(Guid.Parse(id));
            }
            return View(budget);
        }

        public JsonResult Edit(string id, BudgetEntity budget)
        {
            var budgetbll = new BudgetBLL();
            var success = true;
            var message = "保存成功！";
            try
            {
                if (string.IsNullOrEmpty(id)) budgetbll.Add(budget);
                else budgetbll.Modify(budget);
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
            }
            return Json(new AjaxResult() { type = success ? ResultType.success : ResultType.error, message = message });
        }

        public JsonResult Remove(string id)
        {
            var budgetbll = new BudgetBLL();
            var success = true;
            var message = "保存成功！";
            try
            {
                budgetbll.Remove(id);
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
