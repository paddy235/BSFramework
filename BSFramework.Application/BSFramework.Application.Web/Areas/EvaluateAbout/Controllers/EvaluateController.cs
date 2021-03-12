using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Web.Areas.EvaluateAbout.Models;
using BSFramework.Busines.EvaluateAbout;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Entity.EvaluateAbout;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ThoughtWorks.QRCode.Codec;
using Aspose.Cells;
using System.Drawing;
using BSFramework.Util.Offices;
using System.Data;
using BSFramework.Application.Busines.EvaluateAbout;
using BSFramework.Application.Entity.EvaluateAbout;

namespace BSFramework.Application.Web.Areas.EvaluateAbout.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class EvaluateController : MvcControllerBase
    {
        public JsonResult GetData1(string evaluateid)
        {
            var bll = new EvaluateBLL();
            var data = bll.GetData1(evaluateid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetData2(string evaluateid)
        {
            var bll = new EvaluateBLL();
            var data = bll.GetData2(evaluateid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Deducts(string id)
        {
            ViewBag.id = id;
            return View();
        }
        [HttpGet]
        public JsonResult GetDeducts(string id)
        {
            var dpbll = new DepartmentBLL();
            var bll = new EvaluateBLL();
            int total = 0;
            var items = bll.GetAllCategoryItems("");
            var data = new List<DeductInfo>();
            var entity = new DeductInfo();
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            foreach (EvaluateCategoryItemEntity e in items)
            {
                entity = new DeductInfo();
                dt = bll.GetGroups(id, e.ItemId, "0");
                dt1 = bll.GetGroups(id, e.ItemId, "1");
                if (dt1.Rows.Count > 0)
                {
                    if (dt.Rows.Count > 0)
                    {
                        entity.Category = e.Category.Category;
                        entity.ItemContent = e.ItemContent;
                        entity.Times = dt.Rows.Count;
                        foreach (DataRow row in dt.Rows)
                        {
                            entity.GroupNames += row["groupname"].ToString() + ",";
                        }
                        entity.Percent = Math.Round((double)dt.Rows.Count / dt1.Rows.Count * 100, 2);
                        if (entity.GroupNames.EndsWith(",")) entity.GroupNames = entity.GroupNames.TrimEnd(',');
                        data.Add(entity);
                    }


                }

            }
            data = data.OrderByDescending(x => x.Times).ToList();
            return Json(new { rows = data, records = total, page = 1, total = data.Count() }, JsonRequestBehavior.AllowGet);
        }

        public ViewResult Index8()
        {
            var user = OperatorProvider.Provider.Current();
            var bll = new EvaluateBLL();
            var total = 0;
            var evaluations = bll.GetEvaluations(null, null, 10, 1, out total).Where(x => x.IsPublished == true).ToList();
            var categories = bll.GetBigCategories().Select(x => x.Category).ToArray();
            var deptbll = new DepartmentBLL();
            var depts = deptbll.GetAllGroups().ToList();
            if (evaluations.Count > 0)
            {
                var evaluateid = evaluations.OrderByDescending(x => x.CreateTime).FirstOrDefault().EvaluateId;
                depts.RemoveAll(x => x.DepartmentId == user.DeptId);
                //ViewBag.DeptId = user.DeptId;
                //ViewBag.DeptName = user.DeptName;
                ViewBag.groups = depts.Select(x => new { x.DepartmentId, x.FullName }).ToList();
                ViewBag.categories = categories;
                ViewBag.evaluateid = evaluations.OrderByDescending(x => x.CreateTime).FirstOrDefault().EvaluateId;

                var pcts = bll.GetCalcScore(evaluateid, null);
                ViewBag.pcts = pcts;
            }
            return View();
        }

        public ActionResult Index()
        {
            var user = OperatorProvider.Provider.Current();
            bool canAdd = false;
            if (user.IsSystem || ConfigurationManager.AppSettings["EvaluateDept"].ToString().Contains(user.DeptName))
            {
                canAdd = true;
            }
            ViewBag.allowadd = canAdd;
            return View();
        }
        public ActionResult EvalDepts(string id)
        {
            ViewBag.eid = id;
            return View();
        }

        public ViewResult Edit2(string id)
        {
            var user = OperatorProvider.Provider.Current();
            var evaluate = default(EvaluateEntity);

            if (string.IsNullOrEmpty(id))
            {
                evaluate = new EvaluateEntity()
                {
                    PublishDate = DateTime.Today,
                    EvaluateUser = user.UserName,
                    EvaluateUserId = user.UserId,
                    LimitTime = DateTime.Today.AddDays(1)
                };
            }
            else
            {
                evaluate = new EvaluateBLL().GetEvaluate(id);
                ViewBag.name = evaluate.EvaluateSeason;
            }

            ViewBag.evaluateid = evaluate.EvaluateId;

            return View(evaluate);
        }

        [HttpPost]
        public JsonResult Edit2(EvaluateEntity model)
        {
            var bll = new EvaluateBLL();

            var success = true;
            var message = "保存成功";

            try
            {
                if (string.IsNullOrEmpty(model.EvaluateId))
                {
                    model.EvaluateStatus = "考评汇总";
                    model.IsEvaluated = true;
                    model.IsCalculated = true;
                    //现需求默认一添加就可以发布
                    bll.AddEvaluate(model);
                }
                else
                    bll.EditEvaluate(model);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }
        public JsonResult GetDeptData(string eid, int rows, int page)
        {
            var bll = new EvaluateBLL();
            var user = OperatorProvider.Provider.Current();

            var total = bll.GetAllDeptsById(eid).Count();
            var data = bll.GetAllDeptsById(eid).Skip(rows * (page - 1)).Take(rows).ToList();

            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / rows) }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetData(string name, int rows, int page)
        {
            var bll = new EvaluateBLL();
            //bll.EnsureEvaluate();

            var user = OperatorProvider.Provider.Current();

            var total = 0;
            var data = bll.GetEvaluationsFoWeb(name, user.UserId, rows, page, out total);

            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / rows) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EvaluateDetail(string id)
        {
            var user = OperatorProvider.Provider.Current();
            var bll = new EvaluateBLL();
            var evaluate = bll.GetEvaluate(id);
            ViewBag.evaluateid = evaluate.EvaluateId;

            var userDept = new DepartmentBLL().GetList().FirstOrDefault(p => p.DepartmentId == user.DeptId);
            IList<EvaluateCategoryItemEntity> items;
            //if (userDept == null || userDept.IsOrg=="0") 
            //{
            //    items = bll.GetAllCategoryItems(null); //普通部门（底下带班组的部门）打本部门底下班组的所有的数据
            //}
            //else
            //{
            //    items = bll.GetAllCategoryItems(user.DeptName);//厂级部门打所有部门的特定项
            //}
            // var items = bll.GetAllCategoryItems(null);//需求变动 ，目前所有部门均可打分
            items = bll.GetAllCategoryItems(null);
            //先查是不是考评部门，是考评部门就筛选出对于的考评标准
            var childDept = new DepartmentBLL().GetList().Where(p => p.ParentId == user.DeptId).Select(x => x.DepartmentId);
            //if (items.Any(x => x.EvaluateDept.Contains(user.DeptName)))
            //{
            //    //有则进行筛选
            //    items = items.Where(p => p.EvaluateDept == user.DeptName).ToList();
            //}
            //else if (childDept != null && childDept.Count()>0 )
            //{
            //    //没有的话，判断适用班组里面是否有本部门底下的班组，有：打所有的数据,没有的话走一次部门的筛选
            //    if (items.Any())
            //    {

            //    }


            //}
            //else
            //{
            //    //两条都不符合，还是进行一次部门的筛选
            //    items = items.Where(p => p.EvaluateDept == user.DeptName).ToList();
            //}

            //如果本部门底下的班组出现在了适用班组里面，则可以打所有的,反之，则进行一次部门筛选
            if (!childDept.Any(x => evaluate.DeptScope.Contains(x)))
            {
                items = items.Where(p => p.EvaluateDept == user.DeptName).ToList();
            }

            var categories = items.Select(x => x.Category).Select(x => new { x.CategoryId, x.Category }).Distinct().GroupJoin(items, x => x.CategoryId, y => y.CategoryId, (x, y) => new { x.CategoryId, x.Category, Items = y.Select(n => new { n.ItemId, n.ItemContent, n.Score }) });
            ViewBag.categories = HttpUtility.JavaScriptStringEncode(Newtonsoft.Json.JsonConvert.SerializeObject(categories));

            var entity = bll.GetEvaluateionDetail(id, user.DeptName, user.DeptId);
            ViewBag.details = HttpUtility.JavaScriptStringEncode(Newtonsoft.Json.JsonConvert.SerializeObject(entity));

            return View();
        }

        public ViewResult EvaluateAll(string id)
        {
            var user = OperatorProvider.Provider.Current();
            var bll = new EvaluateBLL();
            var evaluate = bll.GetEvaluate(id);
            ViewBag.evaluateid = evaluate.EvaluateId;
            var items = bll.GetAllCategoryItems(null);
            var categories = items.Select(x => x.Category).Select(x => new { x.CategoryId, x.Category }).Distinct().GroupJoin(items, x => x.CategoryId, y => y.CategoryId, (x, y) => new { x.CategoryId, x.Category, Items = y.Select(n => new { n.ItemId, n.ItemContent, n.Score }) });
            ViewBag.categories = HttpUtility.JavaScriptStringEncode(Newtonsoft.Json.JsonConvert.SerializeObject(categories));

            var entity = bll.GetEvaluateionDetail(id, null);
            ViewBag.details = HttpUtility.JavaScriptStringEncode(Newtonsoft.Json.JsonConvert.SerializeObject(entity));

            return View();
        }
        [HttpPost]
        public JsonResult GetDataNew(string id, string deptname)
        {
            var user = OperatorProvider.Provider.Current();
            var bll = new EvaluateBLL();
            var evaluate = bll.GetEvaluate(id);
            ViewBag.evaluateid = evaluate.EvaluateId;
            if (deptname == "全部") deptname = "";
            var items = bll.GetAllCategoryItems(deptname);
            var categories = items.Select(x => x.Category).Select(x => new { x.CategoryId, x.Category }).Distinct().GroupJoin(items, x => x.CategoryId, y => y.CategoryId, (x, y) => new { x.CategoryId, x.Category, Items = y.Select(n => new { n.ItemId, n.ItemContent, n.Score }) });
            //ViewBag.categories = HttpUtility.JavaScriptStringEncode(Newtonsoft.Json.JsonConvert.SerializeObject(categories));

            var entity = bll.GetEvaluateionDetail(id, null);
            //ViewBag.details = HttpUtility.JavaScriptStringEncode(Newtonsoft.Json.JsonConvert.SerializeObject(entity));

            return Json(new { categories = categories, details = entity });
        }

        [HttpPost]
        public JsonResult Publish(string id)
        {
            var success = true;
            var message = "发布成功";
            var bll = new EvaluateBLL();
            var user = OperatorProvider.Provider.Current();
            try
            {
                bll.Publish(id);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new { success = success, message = message });
        }
        /// <summary>
        /// 点击考评状态列，可根据部门查询
        /// 发布结果
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deptname"></param>
        /// <returns></returns>
        public ViewResult EvaluateAllNew(string id)
        {
            var user = OperatorProvider.Provider.Current();
            var bll = new EvaluateBLL();
            var evaluate = bll.GetEvaluate(id);
            ViewBag.evaluateid = evaluate.EvaluateId;
            List<EvaluateCategoryItemEntity> items = new List<EvaluateCategoryItemEntity>();
            //if (user.UserId == evaluate.EvaluateUserId)
            //{
            //    items = bll.GetAllCategoryItems(null).ToList();
            //}
            //else
            //{
            //    items = bll.GetAllCategoryItems(user.DeptName).ToList();
            //}
            items = bll.GetAllCategoryItems(null).ToList();//改：所有人都可查看评分信息
            var categories = items.Select(x => x.Category).Select(x => new { x.CategoryId, x.Category }).Distinct().GroupJoin(items, x => x.CategoryId, y => y.CategoryId, (x, y) => new { x.CategoryId, x.Category, Items = y.Select(n => new { n.ItemId, n.ItemContent, n.Score }) });
            ViewBag.categories = HttpUtility.JavaScriptStringEncode(Newtonsoft.Json.JsonConvert.SerializeObject(categories));

            var entity = bll.GetEvaluateionDetail(id, null);
            ViewBag.details = HttpUtility.JavaScriptStringEncode(Newtonsoft.Json.JsonConvert.SerializeObject(entity));
            ViewBag.ispublished = entity.IsPublished;
            var departBll = new DepartmentBLL();
            var contentDepart = departBll.GetList().Where(x => x.Nature == "部门").ToList();
            contentDepart.Insert(0, new DepartmentEntity() { FullName = "全部" });
            ViewData["Depart"] = contentDepart.Select(x => new SelectListItem() { Value = x.FullName, Text = x.FullName });
            return View();
        }
        public ActionResult Export(string id)
        {
            DepartmentBLL dpbll = new DepartmentBLL();
            var user = OperatorProvider.Provider.Current();
            var bll = new EvaluateBLL();
            var evaluate = bll.GetEvaluate(id);
            var items = bll.GetAllCategoryItems(null);
            var categories = items.Select(x => x.Category).Select(x => new { x.CategoryId, x.Category }).Distinct().GroupJoin(items, x => x.CategoryId, y => y.CategoryId, (x, y) => new { x.CategoryId, x.Category, Items = y.Select(n => new { n.ItemId, n.ItemContent, n.Score }) }).ToList();
            var entity = bll.GetEvaluateionDetail(id, null);

            Workbook workbook = new Workbook();

            Worksheet sheet = (Worksheet)workbook.Worksheets[0];
            Cells cells = sheet.Cells;
            int cols = entity.Groups.Count() + 3; //总列数
            int rows = items.Count();

            //表头样式
            Style style1 = workbook.Styles[workbook.Styles.Add()];
            style1.Font.Color = Color.Black;
            style1.Font.Size = 11;
            style1.Font.IsBold = true;
            style1.HorizontalAlignment = TextAlignmentType.Center;
            style1.VerticalAlignment = TextAlignmentType.Center;


            Style style2 = workbook.Styles[workbook.Styles.Add()];
            style2.Font.Color = Color.Black;
            style2.Font.Size = 10;
            style2.Font.IsBold = true;
            style2.HorizontalAlignment = TextAlignmentType.Center;
            style2.VerticalAlignment = TextAlignmentType.Center;

            Style style3 = workbook.Styles[workbook.Styles.Add()];
            style3.Font.Color = Color.Black;
            style3.Font.Size = 10;
            style3.HorizontalAlignment = TextAlignmentType.Center;
            style3.VerticalAlignment = TextAlignmentType.Center;

            var depts = entity.Groups.Distinct(new DrugCompareByNameAndLevel()).ToList(); //去重，部门数量

            int j = 3;
            for (int i = 0; i < depts.Count(); i++)
            {
                var name = depts[i].DeptName;
                cells[0, j].PutValue(name);
                cells[0, j].SetStyle(style1);
                int groups = entity.Groups.Where(x => x.DeptName == name).Count();//班组数量
                cells.Merge(0, j, 1, groups);//跨列
                j += groups;

            }
            for (int i = 0; i < entity.Groups.Count; i++)
            {
                cells[1, i + 3].PutValue(entity.Groups[i].GroupName);
                cells[1, i + 3].SetStyle(style2);
            }
            cells[1, 0].PutValue("考评要素"); cells[1, 0].SetStyle(style2);
            cells[1, 1].PutValue("考评内容"); cells[1, 1].SetStyle(style2);
            cells[1, 2].PutValue("标准分"); cells[1, 2].SetStyle(style2);

            int k = 2; int n1 = 0;
            for (int i = 0; i < categories.Count(); i++)
            {
                var list = categories[i].Items.ToList();
                cells[k, 0].PutValue(categories[i].Category);
                n1 = list.Count(); //跨行数
                cells[k, 0].SetStyle(style3);
                cells.Merge(k, 0, n1, 1);
                for (int l = 0; l < n1; l++)
                {
                    cells[l + k, 1].PutValue(list[l].ItemContent);
                    cells[l + k, 2].PutValue(list[l].Score);
                    cells[l + k, 1].SetStyle(style3);
                    cells[l + k, 2].SetStyle(style3);
                }
                k += n1;
            }

            for (int i = 0; i < entity.Groups.Count(); i++) //列
            {
                k = 2; //开始行数 

                for (int o = 0; o < categories.Count(); o++)
                {
                    var list = categories[o].Items.ToList();
                    n1 = list.Count();
                    for (int l = 0; l < n1; l++)
                    {
                        string score = "/";
                        var eid = list[l].ItemId;
                        var ei = entity.Groups[i].Items.Where(x => x.EvaluateContentId == eid).ToList();
                        if (ei.Count() > 0)
                        {
                            var first = ei.First();
                            score = first.ActualScore.ToString();
                            first.MarksRecord.ForEach(x =>
                            {
                                score += "\r\n" + (x.Score > 0 ? "加分：" : "减分：") + x.Score + "    \r原因：" + x.Cause;
                            });
                        }
                        style3.IsTextWrapped = true;
                        cells[l + k, i + 3].PutValue(score);
                        cells[l + k, i + 3].SetStyle(style3);
                    }
                    k += n1;
                }
            }

            var path = Server.MapPath("~/Content/export/");
            path = path.Substring(0, path.LastIndexOf("\\") + 1);

            workbook.Save(Path.Combine(path, "班组考评.xlsx"), Aspose.Cells.SaveFormat.Xlsx);
            ExcelHelper.DownLoadFile(Path.Combine(path, "班组考评.xlsx"), "班组考评.xlsx");
            return Success("导出成功。", new { });
        }

        [HttpPost]
        public JsonResult SubmitEvaluations(string id, EvaluateModel model)
        {
            var success = true;
            var message = "保存成功";

            var bll = new EvaluateBLL();
            try
            {
                bll.Evaluate(id, model.EvaluateItems.Select(x => new EvaluateItemEntity() { EvaluateItemId = x.EvaluateItemId, ActualScore = x.ActualScore.Value }).ToList());
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }

        public ViewResult Edit(string id)
        {
            var user = OperatorProvider.Provider.Current();
            var bll = new EvaluateBLL();
            var evaluateitem = bll.GetEvaluateItem(id);
            var categoryitem = bll.GetCategoryItem(evaluateitem.EvaluateContentId);
            var userDept = new DepartmentBLL().GetEntity(user.DeptId);
            ViewBag.UserDept = userDept;
            List<EvaluateMarksRecordsEntity> marksRecords = bll.GetMarksRecords(id);
            ViewBag.MarksRecords = marksRecords;
            ViewBag.ThisUser = user;
            return View(new EvaluateItemModel() { EvaluateItemId = evaluateitem.EvaluateItemId, EvaluateContentId = evaluateitem.EvaluateContentId, EvaluateContent = evaluateitem.EvaluateContent, ActualScore = evaluateitem.ActualScore, EvaluateDept = user.DeptName, EvaluatePerson = user.UserName, EvaluateTime = DateTime.Now, Score = evaluateitem.Score, EvaluateStandard = categoryitem.ItemStandard, Reason = evaluateitem.Reason });
        }

        [HttpPost]
        public JsonResult Edit(string id, EvaluateItemModel model)
        {
            var bll = new EvaluateBLL();

            var success = true;
            var message = "保存成功";

            try
            {
                bll.EditEvaluateItem(new EvaluateItemEntity() { EvaluateItemId = id, ActualScore = model.ActualScore.Value, Reason = model.Reason, EvaluateDept = model.EvaluateDept, EvaluatePerson = model.EvaluatePerson, EvaluateTime = model.EvaluateTime });
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message), resultdata = model.ActualScore });
        }

        public ViewResult Index2(string type)
        {
            var bll = new EvaluateBLL();
            var total = 0;
            var data = bll.GetEvaluations(null, null, 50, 1, out total).OrderByDescending(x => x.CreateTime).ToList(); //
            var categories = bll.GetCategories();
            ViewBag.season = "";
            if (data.Count > 0) ViewBag.season = data.First().EvaluateSeason;
            ViewBag.categories = categories;

            var pcts = bll.GetCalcScore(data.Count == 0 ? "xxx" : data.First().EvaluateId, null);
            ViewBag.type = type;
            ViewBag.pcts = pcts;

            return View(data.ToList());
        }

        public JsonResult GetCalcScore(string id, string categoryid)
        {
            var bll = new EvaluateBLL();
            var pcts = bll.GetCalcScore(id, categoryid);
            return Json(pcts, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCalcScore1(string id, string categoryid)
        {
            var dpbll = new DepartmentBLL();
            var use = OperatorProvider.Provider.Current();
            var dept = dpbll.GetEntity(use.DeptId);
            var pdept = dpbll.GetEntity(dept.ParentId);
            var bll = new EvaluateBLL();
            var pcts = bll.GetCalcScore(id, categoryid);
            pcts = pcts.Where(x => x.DeptId == pdept.DepartmentId).ToList();
            return Json(pcts, JsonRequestBehavior.AllowGet);
        }
        public decimal CutDecimal(decimal d, int n)
        {
            string strdecimal = d.ToString();
            int index = strdecimal.IndexOf(".");
            if (index == -1 || strdecimal.Length < index + n + 1)
            {
                strdecimal = string.Format("{0:F" + n + "}", d);

            }
            else
            {
                int length = index;
                if (n != 0)
                {
                    length = index + n + 1;
                }
                strdecimal = strdecimal.Substring(0, length);
            }
            return Decimal.Parse(strdecimal);
        }
        [HttpGet]
        public JsonResult GetCalcScoreNew(string id, string categoryid, int indexType = 0)
        {
            var bll = new EvaluateBLL();
            int total = 0;
            var ids = bll.GetEvaluationsFoWeb("", "", 10000, 1, out total).OrderByDescending(x => x.CreateTime).Select(x => x.EvaluateId).ToList();
            if (string.IsNullOrEmpty(id) && ids.Count() > 0) id = ids[0];
            var data = bll.GetCalcScoreNew(id, categoryid, indexType);


            int idx = ids.IndexOf(id) + 1;
            if (idx < ids.Count())
            {
                id = ids[idx];//上一次考评id
                var data1 = bll.GetCalcScoreNew(id, categoryid, indexType);
                for (int i = 0; i < data.Count(); i++)
                {
                    if (data1 != null && data1.Count > 0)
                    {
                        if (i < data1.Count)
                        {
                            data[i].Score2 = data1[i].Score1;//上次得分
                        }
                        else
                        {
                            data[i].Score2 = 0;
                        }
                    }
                    if (data[i].Score != 0)
                    {
                        decimal reScore = (data[i].Score1 / data[i].Score * 100);
                        double Percent = (double)Math.Round(reScore, 2);//得分率
                        data[i].Percent = Percent + "%";
                    }
                    else
                    {
                        data[i].Percent = "0%";
                    }
                    if (data[i].Score2 != 0)
                    {
                        decimal Percent = Math.Round(Convert.ToDecimal((data[i].Score1 - data[i].Score2) / data[i].Score2 * 100), 2);//环比
                        data[i].Percent1 = Percent + "%";
                    }
                    else
                    {
                        data[i].Percent1 = "0%";
                    }
                }
            }
            else
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].Score2 = 0;
                    if (data[i].Score != 0)
                    {

                        decimal Percent = Math.Round((data[i].Score1 / data[i].Score * 100), 2);//得分率
                        data[i].Percent = Percent + "%";
                    }
                    else
                    {
                        data[i].Percent = "0%";
                    }
                    data[i].Percent1 = "0%";
                }
            }
            var use = OperatorProvider.Provider.Current();
            IList<Group> newdata = new List<Group>();

            foreach (IGrouping<string, Group> group in data.GroupBy(x => x.GroupBy))
            {
                int n = 1;
                foreach (Group g in group.OrderByDescending(a => a.Score1))
                {
                    g.Index = n;
                    newdata.Add(g);
                    n++;
                }
            }
            return Json(new { rows = newdata, records = total, page = 1, total = total }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetIndexCalcScore()
        {
            var user = OperatorProvider.Provider.Current();
            string id = string.Empty;
            var bll = new EvaluateBLL();
            DepartmentBLL dept = new DepartmentBLL();
            int total = 0;
            var ids = bll.GetEvaluations("", "", 10000, 1, out total).OrderByDescending(x => x.CreateTime).Select(x => x.EvaluateId).ToList();
            if (ids.Count() > 0) id = ids[0];
            var tree = dept.GetAuthorizationDepartment(user.DeptId);
            var data = bll.GetCalcScoreIndex(id, "", tree.EnCode);
            for (int i = 0; i < data.Count(); i++)
            {
                data[i].Score2 = 0;
                if (data[i].Score != 0)
                {
                    var point = data[i].Score1 / data[i].Score * 100;
                    decimal Percent = Math.Round(point, 2);//得分率
                    data[i].Percent = Percent + "%";
                }
                else
                {
                    data[i].Percent = "0%";
                }
                data[i].Percent1 = "0%";
            }
            IList<GroupIndex> newdata = new List<GroupIndex>();

            foreach (IGrouping<string, GroupIndex> group in data.GroupBy(x => x.Category))
            {
                int num = group.Count();
                var one = new GroupIndex();
                one.Category = group.Key;
                one.Score = group.Sum(p => p.Score);
                one.Score1 = group.Sum(p => p.Score1);
                //foreach (GroupIndex g in group)
                //{
                //    one.Score = one.Score + g.Score;
                //    one.Score1 = one.Score1 + g.Score1;
                //    num++;
                //}
                double Percent = 0;
                double Percent1 = Math.Round(Convert.ToDouble(one.Score1) / num, 2);//平均分
                if (one.Score1 != 0 && one.Score != 0)
                {
                    Percent = Math.Round(Convert.ToDouble(one.Score1 / one.Score * 100), 2);//得分率

                }

                if (Double.IsInfinity(Percent))
                {
                    Percent = 0;
                }
                one.Percent = Percent.ToString();
                one.Percent1 = Percent1.ToString();
                newdata.Add(one);
            }
            newdata = newdata.OrderBy(x => double.Parse(x.Percent)).ToList();
            int index = 1;
            foreach (var item in newdata)
            {
                item.Index = index;
                item.Percent = item.Percent + "%";
                index++;
            }
            return Json(new { data = newdata }, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 首页-实时扣分
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetDeductsIndex()
        {
            #region 旧
            //var bll = new EvaluateBLL();
            //var total = 0;
            //var Timedata = bll.GetEvaluations(null, null, 10, 1, out total).OrderByDescending(x => x.CreateTime).Where(x => x.IsPublished == true).ToList(); //
            //string id = Timedata.Count() > 0 ? Timedata[0].EvaluateId : "";
            //var user = OperatorProvider.Provider.Current();
            //DepartmentBLL dept = new DepartmentBLL();
            //var tree = dept.GetAuthorizationDepartment(user.DeptId);
            //total = 0;
            //var dpbll = new DepartmentBLL();
            //var deptList = dept.GetSubDepartments(tree.DepartmentId, "");
            //var items = bll.GetAllCategoryItemsIndex(string.Join(",", deptList.Select(x => x.FullName)));
            //var data = new List<DeductInfo>();
            //DataTable dt = new DataTable();
            //DataTable dt1 = new DataTable();
            ////foreach (EvaluateCategoryItemEntity e in items)
            ////{
            //var weidthList = new WeightSetBLL().GetList(null).ToList();
            //IList<EvaluateCategoryEntity> categoryList = new EvaluateBLL().GetAllCategories();
            //dt = bll.GetGroupsIndex(id, "", "0");
            //// dt1 = bll.GetGroupsIndex(id, e.ItemId, "1");
            ////if (dt1.Rows.Count > 0)
            ////{
            //if (dt.Rows.Count > 0)
            //{
            //    for (int i = 0; i < dt.Rows.Count; i++)
            //    {
            //        #region 得到权重
            //        //根据categoryId判断权重
            //        WeightSetEntity weight = null;
            //        var rootId = string.Empty;
            //        if (weidthList.Any(x => x.Id == dt.Rows[i]["categoryid"].ToString()))
            //        {
            //            weight = weidthList.Where(x => x.Id == dt.Rows[i]["categoryid"].ToString()).FirstOrDefault();
            //        }
            //        else
            //        {
            //            var categoryObj = categoryList.Where(x => x.CategoryId == dt.Rows[i]["categoryid"].ToString()).FirstOrDefault();
            //            if (categoryObj != null)
            //            {
            //                rootId = categoryObj.ParentCategoryId;
            //                if (weidthList.Any(x => x.Id == categoryObj.ParentCategoryId))
            //                {
            //                    weight = weidthList.Where(x => x.Id == categoryObj.ParentCategoryId).FirstOrDefault();
            //                }
            //                else
            //                {
            //                    var sendsObj = categoryList.Where(x => x.CategoryId == rootId).FirstOrDefault();
            //                    if (sendsObj != null)
            //                    {
            //                        rootId = sendsObj.ParentCategoryId;
            //                        if (weidthList.Any(x => x.Id == sendsObj.ParentCategoryId))
            //                        {
            //                            weight = weidthList.Where(x => x.Id == sendsObj.ParentCategoryId).FirstOrDefault();
            //                        }
            //                        else
            //                        {
            //                            var rootObj = categoryList.Where(x => x.CategoryId == rootId).FirstOrDefault();
            //                            if (rootObj != null)
            //                            {
            //                                rootId = rootObj.ParentCategoryId;
            //                                if (weidthList.Any(x => x.Id == rootObj.ParentCategoryId))
            //                                {
            //                                    weight = weidthList.Where(x => x.Id == rootObj.ParentCategoryId).FirstOrDefault();
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }


            //        #endregion
            //        var entity = new DeductInfo();
            //        entity.ItemContent = dt.Rows[i]["reason"].ToString();
            //        entity.GroupNames = dt.Rows[i]["groupname"].ToString();
            //        if (weight != null && weight.Weight != null)
            //        {
            //            entity.Times = Math.Round((Convert.ToDecimal(dt.Rows[i]["Score"]) - Convert.ToDecimal(dt.Rows[i]["actualscore"])) * weight.Weight.Value, 2);
            //        }
            //        else
            //        {
            //            entity.Times = Int32.Parse(dt.Rows[i]["Score"].ToString()) - Int32.Parse(dt.Rows[i]["actualscore"].ToString());
            //        }
            //        entity.CreateTime = dt.Rows[i]["modifytime"].ToString();

            //        if (entity.Times > 0)
            //        {
            //            data.Add(entity);

            //        }
            //    }

            //}


            //// }

            //// }
            //data = data.OrderByDescending(x => x.CreateTime).ToList();
            //return Json(new { data = data }, JsonRequestBehavior.AllowGet);
            #endregion
            #region 新 heming
            List<DeductInfo> data = new List<DeductInfo>();
            var items = new EvaluateBLL().GetIndexGradeInfo();//首页实时扣分
            if (items != null && items.Count > 0)
            {
                var dte = items.GetEnumerator();
                while (dte.MoveNext())
                {
                    var dr = dte.Current;
                    DeductInfo deduct = new DeductInfo()
                    {
                        ItemContent = dr.Cause?.ToString(),
                        GroupNames = dr.GroupName?.ToString(),
                        CreateTime = dr.CreateDate.ToString("yyyy/mm/dd")
                    };
                    decimal score = dr.Score;
                    //if (dr["Weight"] is DBNull)
                    //{
                    //    deduct.Times = score;
                    //}
                    //else
                    //{
                    //    decimal weight = Convert.ToDecimal(dr["Weight"]);
                    //    deduct.Times = Math.Round(score * weight, 2);
                    //}
                    data.Add(deduct);
                }
            }
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
            #endregion
        }
        //public JsonResult GetCalcScore2(string id)
        //{
        //    var user = OperatorProvider.Provider.Current();
        //    var bll = new EvaluateBLL();
        //    var pcts = bll.GetCalcScore2(id, user.DeptId);
        //    return Json(pcts, JsonRequestBehavior.AllowGet);
        //}

        public ViewResult Index3()
        {
            var user = OperatorProvider.Provider.Current();
            var bll = new EvaluateBLL();
            var total = 0;
            var evaluations = bll.GetEvaluations(null, null, 10, 1, out total).Where(x => x.EvaluateSeason != null && x.EvaluateSeason.Contains(DateTime.Now.Year.ToString()) && x.IsPublished == true).ToList();
            var pcts = bll.GetCalcScore2(DateTime.Now.Year.ToString(), user.DeptId);
            var evaluate = new List<EvaluateGroupEntity>();
            if (evaluations.Count > 0)
            {
                evaluate = bll.GetCalcScore(evaluations[0].EvaluateId, null).ToList();
            }
            var current = evaluate.FirstOrDefault(x => x.GroupName == user.DeptName);
            if (current == null) current = new EvaluateGroupEntity();

            ViewBag.evaluations = evaluations;
            ViewBag.pcts = pcts;
            var seq = 1;
            if (evaluate.Count > 0)
            {
                var lastScore = evaluate[0].Pct;
                for (int i = 0; i < evaluate.Count; i++)
                {
                    if (evaluate[i].Pct != lastScore)
                    {
                        lastScore = evaluate[i].Pct;
                        seq++;
                    }
                    if (evaluate[i].GroupName == current.GroupName)
                        break;
                }
            }
            ViewBag.seq = seq;
            ViewBag.total = current.ActualScore;

            var pct = new EvaluateCalcEntity() { Data = new List<EvaluateItemCalcEntity>() };
            if (evaluations.Count > 0)
            {
                pct = pcts.FirstOrDefault(x => x.Season == evaluations[0].EvaluateSeason);
            }


            return View(pct);
        }

        public JsonResult GetCalcScore2(string name)
        {
            var user = OperatorProvider.Provider.Current();
            var bll = new EvaluateBLL();
            var total = 0;
            var evaluations = bll.GetEvaluations(null, null, 10, 1, out total);
            var evaltion = evaluations.FirstOrDefault(x => x.EvaluateSeason == name);
            var pcts = bll.GetCalcScore2(DateTime.Now.Year.ToString(), user.DeptId);
            var evaluate = bll.GetCalcScore(evaltion.EvaluateId, null);
            var current = evaluate.FirstOrDefault(x => x.GroupName == user.DeptName);
            var seq = 1;
            if (evaluate.Count > 0)
            {
                var lastScore = evaluate[0].Pct;
                for (int i = 0; i < evaluate.Count; i++)
                {
                    if (evaluate[i].Pct != lastScore)
                    {
                        lastScore = evaluate[i].Pct;
                        seq++;
                    }
                    if (evaluate[i].GroupName == current.GroupName)
                        break;
                }
            }
            var totalscore = current.ActualScore;
            var pct = pcts.FirstOrDefault(x => x.Season == name);
            return Json(new { total = totalscore, seq, pct }, JsonRequestBehavior.AllowGet);
        }


        public ViewResult Index4()
        {
            var user = OperatorProvider.Provider.Current();
            var bll = new EvaluateBLL();
            var total = 0;
            var evaluations = bll.GetEvaluations(null, null, 10, 1, out total).Where(x => x.EvaluateSeason.Contains(DateTime.Now.Year.ToString()) && x.IsPublished == true).ToList();
            var categories = bll.GetBigCategories().Select(x => x.Category).ToArray();
            var deptbll = new DepartmentBLL();
            var groups = deptbll.GetAllGroups().ToList();
            groups.RemoveAll(x => x.DepartmentId == user.DeptId);
            ViewBag.DeptId = user.DeptId;
            ViewBag.DeptName = user.DeptName;
            ViewBag.groups = groups;
            ViewBag.categories = categories;
            ViewBag.evaluations = evaluations;
            return View();
        }

        public JsonResult GetCalcScore3(string evaluateid, string groupid, string othergroupid)
        {
            var bll = new EvaluateBLL();
            var data = bll.GetCalcScore3(evaluateid, groupid, othergroupid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCalcScore4(string evaluateid, string groupid)
        {
            var bll = new EvaluateBLL();
            var data = bll.GetCalcScore4(evaluateid, groupid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Submit(string id)
        {
            var success = true;
            var message = "提交成功";
            var bll = new EvaluateBLL();
            var user = OperatorProvider.Provider.Current();
            try
            {
                bll.Submit(id, user.DeptId);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message), resultdata = id });
        }

        public JsonResult SubmitAll(string id)
        {
            var success = true;
            var message = "提交成功";
            var bll = new EvaluateBLL();
            var user = OperatorProvider.Provider.Current();
            try
            {
                bll.SubmitAll(id);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message), resultdata = id });
        }

        /// <summary>
        /// 首页班组考评分布页
        /// </summary>
        /// <returns></returns>
        public ViewResult IndexEvaluatePartView()
        {
            Operator user = OperatorProvider.Provider.Current();
            var company = new DepartmentBLL().GetCompany(user.DeptId);//获取当前登录人所在的电厂
            ViewBag.deptid = company.DepartmentId;
            return View();
        }

        /// <summary>
        /// 首页班组考评排名
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexStatisticsPartView()
        {
            var bll = new EvaluateBLL();
            int total = 0;
            var ids = bll.GetEvaluationsFoWeb("", "", 10000, 1, out total).OrderByDescending(x => x.CreateTime).Select(x => x.EvaluateId).ToList();
             string id = ids[0];
            var data = bll.GetCalcScoreNew(id, "", 0);
            for (int i = 0; i < data.Count(); i++)
            {
                if (data[i].Score != 0)
                {
                    decimal reScore = (data[i].Score1 / data[i].Score * 100);
                    double Percent = (double)Math.Round(reScore, 2);//得分率
                    data[i].Percent = Percent + "%";
                    data[i].Score1 = Convert.ToDecimal(Percent);
                }
                else
                {
                    data[i].Percent = "0%";
                    data[i].Score1 = 0;
                }
            }

            return View(data.OrderByDescending(x => x.Score1).ToList());
        }


        #region 终端考评排名
        public ActionResult Index5(string name)
        {
            var dpbll = new DepartmentBLL();
            var bll = new EvaluateBLL();
            var total = 0;
            var data = bll.GetEvaluations(null, null, 10, 1, out total).Where(x => x.IsPublished == true).OrderByDescending(x => x.CreateTime).ToList(); ;
            ViewBag.season = "";
            if (data.Count > 0) ViewBag.season = data.First().EvaluateSeason;
            var categories = bll.GetCategories();
            var use = OperatorProvider.Provider.Current();
            var dept = dpbll.GetEntity(use.DeptId);
            var pdept = dpbll.GetEntity(dept.ParentId);
            ViewBag.deptname = use.DeptName;
            ViewBag.name = name;
            ViewBag.categories = categories;
            ViewBag.pdeptname = pdept.FullName;
            var pcts = bll.GetCalcScore(data.Count == 0 ? "xxx" : data.First(row => row.EvaluateSeason == name).EvaluateId, null);
            pcts = pcts.Where(x => x.DeptId == pdept.DepartmentId).ToList();
            ViewBag.pcts = pcts;

            return View(data);
        }
        public ActionResult Index6(string name)
        {
            var user = OperatorProvider.Provider.Current();
            var bll = new EvaluateBLL();
            var total = 0;
            var evaluations = bll.GetEvaluations(null, null, 10, 1, out total).Where(x => x.EvaluateSeason.Contains(DateTime.Now.Year.ToString()) && x.IsPublished == true).OrderByDescending(x => x.CreateTime).ToList();
            ViewBag.evaluations = evaluations;
            var Score = bll.GetCalcScoreItme(evaluations.Count == 0 ? "xxx" : evaluations.First(row => row.EvaluateSeason == name).EvaluateId, null, user.DeptId);
            var categories = bll.GetCategories();
            ViewBag.name = name;
            ViewBag.scoreitem = Score;
            ViewBag.categories = categories;

            return View();
        }
        public ActionResult GetScoreItem(string id, string CategoryId)
        {
            var user = OperatorProvider.Provider.Current();
            var bll = new EvaluateBLL();
            var Score = bll.GetCalcScoreItme(id, CategoryId, user.DeptId);
            var contentStr = BSFramework.Util.Json.ToJson(Score);
            ViewBag.scoreitem = Score;
            return Content(contentStr);
        }
        /// <summary>
        /// 删除单条考评打分
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public ActionResult RemoveForm(string keyValue)
        {
            try
            {
                var bll = new EvaluateBLL();
                var user = OperatorProvider.Provider.Current();

                int Count = bll.GetAllDeptCountById(keyValue);
                if (Count > 0)//已经有部门打分，不允许删除
                {
                    return Error("该条考评计划已有评分，无法删除！");
                }
                else
                {
                    bll.DelEvaluateById(keyValue);
                    return Success("操作成功");
                }
            }
            catch (Exception ex)
            {
                return Error("删除失败：" + ex.Message);
            }
        }
        #endregion
        #region 管理考评

        public ViewResult Index7()
        {
            var user = OperatorProvider.Provider.Current();
            var bll = new EvaluateBLL();
            var total = 0;
            var evaluations = bll.GetEvaluations(null, null, 10, 1, out total).Where(x => x.EvaluateSeason.Contains(DateTime.Now.Year.ToString())).ToList();
            var categories = bll.GetBigCategories().Select(x => x.Category).ToArray();
            var deptbll = new DepartmentBLL();
            var groups = deptbll.GetAllGroups().ToList();
            groups.RemoveAll(x => x.DepartmentId == user.DeptId);
            ViewBag.DeptId = user.DeptId;
            ViewBag.DeptName = user.DeptName;
            ViewBag.groups = groups;
            ViewBag.categories = categories;
            ViewBag.evaluations = evaluations;
            return View();
        }
        /// <summary>
        /// 选择考评范围
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckScope()
        {
            var bll = new DepartmentBLL();
            var depts = bll.GetAllGroups();
            var deptsEnumerator = depts.GetEnumerator();
            List<TreeModel> data = new List<TreeModel>();
            while (deptsEnumerator.MoveNext())
            {
                var current = deptsEnumerator.Current;
                data.Add(new TreeModel(true) { id = current.DepartmentId, isexpand = true, text = current.FullName, value = current.DepartmentId, hasChildren = false });
            }
            return View(data);
        }
        /// <summary>
        /// 加减分页面
        /// </summary>
        /// <param name="id">加减分的主键ID</param>
        /// <returns></returns>     
        public ActionResult AddScoreForm(string id = "")
        {
            EvaluateMarksRecordsEntity entity = new EvaluateMarksRecordsEntity();
            entity.EvaluateItemId = Request["itemId"];
            if (!string.IsNullOrWhiteSpace(id))
            {
                entity = new EvaluateBLL().GetMarksRecordEntity(id);
            }
            return View(entity);
        }
        [HttpPost]
        public ActionResult AddScoreForm(EvaluateMarksRecordsEntity entity)
        {
            try
            {
                Operator user = OperatorProvider.Provider.Current();
                entity.CreateUserId = entity.ModifyUserId = user.UserId;
                entity.CreateUserName = entity.ModifyUser = user.UserName;


                DepartmentEntity userDpet = new DepartmentBLL().GetEntity(user.DeptId);
                if (userDpet == null) throw new Exception("用户所在部门为空");
                var ScoreType = Request.Form["ScoreType"];
                if (ScoreType == "0")//加分
                    entity.Score = Math.Abs(entity.Score);
                else
                    entity.Score = -(Math.Abs(entity.Score));//减分
                var bll = new EvaluateBLL();
                if (!string.IsNullOrWhiteSpace(entity.Id))
                {
                    var old = bll.GetMarksRecordEntity(entity.Id);
                    if (old == null)
                    {
                        bll.AddMarksRecord(entity);
                    }
                    else
                    {
                        //如果当前数据时部门级的，并且数据是公司级人员修改的,添加一条修改记录
                        if (old.IsOrg == 0 && userDpet.IsSpecial)
                        {
                            new EvaluateReviseBLL().Insert(old, entity);
                        }

                        old.Cause = entity.Cause;
                        old.Score = entity.Score;
                        bll.UpdateMarksRecord(old);
                    }
                }
                else
                {
                    bll.AddMarksRecord(entity);
                }
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                return Error("操作失败：" + ex.Message);
            }
        }
        /// <summary>
        /// 删除考评记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RemoveMarksRecord(string id)
        {
            var user = OperatorProvider.Provider.Current();
            try
            {
                var bll = new EvaluateBLL();
                var entity = bll.GetMarksRecordEntity(id);
                entity.CreateUserId = user.UserId;
                entity.CreateUserName = user.UserName;
                bll.RemoveMarksRecord(id);
                if (entity.IsOrg == 0)
                {
                    new EvaluateReviseBLL().Insert(user.DeptId, entity);
                }

                return Success("删除成功");
            }
            catch (Exception ex)
            {
                return Error("删除失败" + ex.Message);
            }
        }
        #endregion

        public JsonResult GetEvaluateJson(string deptid)
        {
            //var user = OperatorProvider.Provider.Current();
            //var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            var depts = new DepartmentBLL().GetSubDepartments(deptid, "班组");
            var deptids = depts.Select(x => x.DepartmentId).ToArray();

            var bll = new EvaluateBLL();
            var current = bll.GetLastEvaluate();
            if (current == null) return Json(null);
            var evaluate1 = bll.GetEvaluate1(current);
            var evaluate2 = bll.GetEvaluate2(current);
            var categories = bll.GetAllCategories();
            var categorydata = categories.Where(x => categories.Where(y => y.ParentCategoryId == null).Any(y => y.CategoryId == x.ParentCategoryId)).Select(x => new { x.CategoryId, x.Category, x.CreateTime });
            var basedata = bll.GetEvaluateContent(new string[] { current.EvaluateId, evaluate1 == null ? "xxxx" : evaluate1.EvaluateId, evaluate2 == null ? "xxxx" : evaluate2.EvaluateId }, deptids);
            var data = basedata.GroupBy(x => x.EvaluateItemId, (x, y) => new { EvaluateItemId = x, Items = y.GroupBy(m => m.EvaluateContentId, (m, n) => new { EvaluateContentId = m, avg = n.GroupBy(i => i.EvaluateGroupId, (i, j) => new { EvaluateGroupId = i, ActualScore = j.Sum(p => p.Score) == 0 ? j.Sum(p => p.ActualScore) == 0 ? 0m : 0.1m : (decimal)j.Sum(p => p.ActualScore) / j.Sum(p => p.Score) * 100 }).Average(p => p.ActualScore).ToString("f2"), max = n.GroupBy(i => i.EvaluateGroupId, (i, j) => new { EvaluateGroupId = i, ActualScore = j.Sum(p => p.Score) == 0 ? j.Sum(p => p.ActualScore) == 0 ? 0m : 0.1m : (decimal)j.Sum(p => p.ActualScore) / j.Sum(p => p.Score) * 100 }).Max(p => p.ActualScore).ToString("f2") }) });
            if (data.Count() == 0) return Json(new List<object>() { new { name = "实时状况", data = string.Empty }, new { name = "班组最高分", data = string.Empty }, new { name = "同比对标", data = string.Empty }, new { name = "环比对标", data = string.Empty } });
            var data1 = categorydata.GroupJoin(data.FirstOrDefault(x => x.EvaluateItemId == current.EvaluateId).Items, x => x.CategoryId, y => y.EvaluateContentId, (x, y) => new { x.Category, x.CreateTime, Score = y.FirstOrDefault() == null ? "0" : y.FirstOrDefault().avg }).OrderBy(x => x.CreateTime);
            var data2 = categorydata.GroupJoin(data.FirstOrDefault(x => x.EvaluateItemId == current.EvaluateId).Items, x => x.CategoryId, y => y.EvaluateContentId, (x, y) => new { x.Category, x.CreateTime, Score = y.FirstOrDefault() == null ? "0" : y.FirstOrDefault().max }).OrderBy(x => x.CreateTime);
            var data3Items = evaluate1 == null ? null : data.FirstOrDefault(x => x.EvaluateItemId == evaluate1.EvaluateId)?.Items;
            var data3 = evaluate1 == null ? null : data3Items == null ? null : (categorydata.GroupJoin(
              data3Items,
                x => x.CategoryId,
                y => y.EvaluateContentId,
                (x, y) => new
                {
                    x.Category,
                    x.CreateTime,
                    Score = y.FirstOrDefault() == null ? "0" : y.FirstOrDefault().avg
                }).OrderBy(x => x.CreateTime));
            var data4 = evaluate2 == null ? null : categorydata.GroupJoin(data.FirstOrDefault(x => x.EvaluateItemId == evaluate2.EvaluateId).Items, x => x.CategoryId, y => y.EvaluateContentId, (x, y) => new { x.Category, x.CreateTime, Score = y.FirstOrDefault() == null ? "0" : y.FirstOrDefault().avg }).OrderBy(x => x.CreateTime);
            return Json(new List<object>() { new { name = "实时状况", data = data1 }, new { name = "班组最高分", data = data2 }, new { name = "同比对标", data = data3 }, new { name = "环比对标", data = data4 } });
        }

        #region 班组称号设置
        public ActionResult SetTitleForm()
        {
            DesignationBLL designationBLL = new DesignationBLL();
            var list = designationBLL.GetList(null);
            return View(list);
        }
        /// <summary>
        /// 设置称号
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public ActionResult SaveGroupTitle(EvaluateGroupTitleEntity entity)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(entity.GroupId))
                {
                    return Error("操作失败：找不到对应班组的Id，请重新选择要设置称号的班组");
                }
                EvaluateGroupTitleBLL titleBLL = new EvaluateGroupTitleBLL();
                if (!string.IsNullOrWhiteSpace(entity.TitleId) && string.IsNullOrWhiteSpace(entity.TId))
                {
                    //称号为空，则删除班组跟称号的关联关系
                    titleBLL.Remove(entity.TitleId);
                }
                else
                {
                    //称号不为空，先查询。看有没有关联关系，有的话就修改，没有就新增
                    EvaluateGroupTitleEntity oldEntity = titleBLL.GetEntity(entity.TitleId);
                    if (oldEntity != null)
                    {
                        //修改
                        titleBLL.Update(entity);
                    }
                    else
                    {
                        //新增
                        titleBLL.Insert(entity);
                    }
                }
                return Success("操作成功！");
            }
            catch (Exception ex)
            {
                return Error("操作失败:" + ex.Message);
            }
        }
        #endregion
    }
    public class DrugCompareByNameAndLevel : IEqualityComparer<EvaluateGroupEntity>
    {
        public bool Equals(EvaluateGroupEntity x, EvaluateGroupEntity y)
        {
            if (x == null || y == null)
            {
                return false;
            }
            if (x.DeptId == y.DeptId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int GetHashCode(EvaluateGroupEntity obj)
        {
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return obj.DeptId.GetHashCode();
            }
        }

    }
    public class DrugCompareByNameAndLevel1 : IEqualityComparer<EvaluateCategoryEntity>
    {
        public bool Equals(EvaluateCategoryEntity x, EvaluateCategoryEntity y)
        {
            if (x == null || y == null)
            {
                return false;
            }
            if (x.CategoryId == y.CategoryId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int GetHashCode(EvaluateCategoryEntity obj)
        {
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return obj.CategoryId.GetHashCode();
            }
        }

    }


}
