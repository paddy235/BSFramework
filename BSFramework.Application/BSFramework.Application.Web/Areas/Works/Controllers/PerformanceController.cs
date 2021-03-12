using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PerformanceManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PerformanceManage;
using BSFramework.Application.Web.Areas.BaseManage.Models;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    /// <summary>
    /// 绩效管理
    /// </summary>
    public class PerformanceController : MvcControllerBase
    {
        private PerformanceBLL bll = new PerformanceBLL();
        private PerformancetitleBLL titleBll = new PerformancetitleBLL();
        private PerformanceupBLL upbll = new PerformanceupBLL();
        private DepartmentBLL deptbll = new DepartmentBLL();

        /// <summary>
        /// 展示页
        /// </summary>
        /// <returns></returns>
        public ViewResult Index()
        {
            var user = OperatorProvider.Provider.Current();
            var nowTime = DateTime.Now.AddMonths(-1);
            var time = new DateTime(nowTime.Year, nowTime.Month, 1);

            if (user.DeptId == "0")
            {
                ViewBag.DeptId = "0";
                ViewBag.parentid = "0";
            }
            else
            {
                var dept = deptbll.GetEntity(user.DeptId);
                if (!dept.IsSpecial)
                {
                    ViewBag.DeptId = user.DeptId;
                    ViewBag.parentid = user.DeptId;
                }
                else
                {
                    ViewBag.DeptId = "0";
                    ViewBag.parentid = "0";

                }


            }
            var tree = deptbll.GetAuthorizationDepartment(user.DeptId);
            ViewBag.treedept = tree.DepartmentId;
            ViewBag.gettime = time.ToString("yyyy-MM-dd");
            ViewBag.settime = time.Year + "年" + time.Month + "月";
            return View();
        }

        /// <summary>
        /// 展示页
        /// </summary>
        /// <returns></returns>
        public ViewResult Indexup(string time, string deptid)
        {

            ViewBag.time = time;
            ViewBag.deptid = deptid;
            return View();
        }
        /// <summary>
        /// 获取树
        /// </summary>
        /// <returns></returns>

        public ActionResult GetDept()
        {
            var user = OperatorProvider.Provider.Current();
            var dept = deptbll.GetEntity(user.DeptId);
            if (user.DeptId == "0")
            {
                dept = new DepartmentEntity() { IsSpecial = true };
            }
            var list = default(List<string>);
            string category = "厂级,部门,班组";
            list = category.Split(',').ToList();
            var depts = bll.getDepartmentList("", category);
            if (dept != null)
            {
                if (!dept.IsSpecial && user.DeptId != "0")
                {
                    var Getdepts = depts.Where(x => x.DepartmentId == user.DeptId).ToList();
                    depts = GetChildren(depts, Getdepts);
                    var one = depts.FirstOrDefault(x => x.DepartmentId == user.DeptId);
                    one.ParentId = "0";
                }
            }
            var getDepts = depts.Where(x => x.Nature == "厂级");
            foreach (var item in getDepts)
            {
                item.ParentId = "0";
            }
            var treeitems = depts.Select(x => new TreeEntity() { id = x.DepartmentId, text = x.FullName, value = x.DepartmentId, parentId = x.ParentId, isexpand = true, complete = true, hasChildren = depts.Count(y => y.ParentId == x.DepartmentId) > 0, showcheck = true, Attribute = "Nature", AttributeValue = x.Nature }).ToList();
            return Content(treeitems.TreeToJson());
        }

        private List<DepartmentEntity> GetChildren(List<DepartmentEntity> dept, List<DepartmentEntity> getdept)
        {
            var current = getdept;
            var deptStr = string.Join(",", getdept.Select(x => x.DepartmentId));
            var subquery = dept.Where(x => deptStr.Contains(x.ParentId)).ToList();
            while (subquery.Count() > 0)
            {
                current = current.Concat(subquery).ToList();
                deptStr = string.Join(",", subquery.Select(x => x.DepartmentId));
                subquery = dept.Where(x => deptStr.Contains(x.ParentId)).ToList();
            }
            return current;
        }
        /// <summary>
        /// 递归树
        /// </summary>
        /// <param name="data"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<TreeModel> GetChildren(List<DepartmentEntity> data, string id, bool showcheck)
        {
            return data.Where(x => x.ParentId == id).Select(x => new TreeModel { id = x.DepartmentId, value = x.DepartmentId, text = x.FullName, isexpand = data.Count(y => y.ParentId == x.DepartmentId) > 0, hasChildren = data.Count(y => y.ParentId == x.DepartmentId) > 0, ChildNodes = GetChildren(data, x.DepartmentId, showcheck), showcheck = showcheck }).ToList();
        }
        /// <summary>
        /// 获取GetScore
        /// </summary>
        /// <param name="time"></param>
        /// <param name="deptid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetScore(string time, string deptid)
        {
            var ckTime = DateTime.Now.ToString("yyyy-MM-01") == time;

            var list = default(List<string>);
            string category = "厂级,部门,班组";
            list = category.Split(',').ToList();
            var depts = bll.getDepartmentList("", category);
            string deptName = "";
            if (deptid == "0")
            {

                foreach (var item in depts)
                {
                    if (item.Nature == "部门")
                    {
                        var dept = depts.Where(x => x.ParentId == item.DepartmentId).ToList();
                        if (dept.Count() > 0)
                        {
                            deptid = dept[0].DepartmentId;
                            deptName = dept[0].FullName;
                            break;
                        }

                    }

                }
            }
            else
            {

                var Nature = deptbll.GetEntity(deptid);
                if (Nature.Nature == "部门")
                {
                    var one = depts.FirstOrDefault(x => x.ParentId == deptid);
                    deptid = one.DepartmentId;
                    deptName = one.FullName;
                }
                if (Nature.Nature == "厂级")
                {
                    foreach (var item in depts)
                    {
                        if (item.Nature == "部门")
                        {
                            var dept = depts.Where(x => x.ParentId == item.DepartmentId).ToList();
                            if (dept.Count() > 0)
                            {
                                deptid = dept[0].DepartmentId;
                                deptName = dept[0].FullName;
                                break;
                            }

                        }

                    }
                }
                if (Nature.Nature == "班组")
                {
                    deptName = Nature.FullName;
                }

            }
            if (ckTime)
            {
                var jsonStr = new
                {
                    rows = "",
                    records = 0,
                    isup = "未提交",
                    total = 0,
                    deptname = deptName
                };
                return Content(jsonStr.ToJson());
            }
            var data = bll.getScore(time, deptid).OrderBy(x => x.planer).ThenBy(x => x.username).ToList();
            double total = 0;
            foreach (var item in data)
            {
                var score = Convert.ToDouble(item.score.Split(',')[0]);
                total = total + score;
            }
            var month = upbll.getListByMonth(time).Where(x => x.departmentid == deptid).ToList();
            var ck = month.FirstOrDefault(x => x.departmentid == deptid);
            bool isup = ck == null ? false : ck.isup;
            List<SortModel> result = new List<SortModel>();
            foreach (var item in data)
            {
                var resultinfo = new SortModel();
                string itemJson = new object().ToJson();
                string myTitleStr = string.Empty;
                myTitleStr = item.score;
                var strSp = myTitleStr.Split(',');
                var sort = item.sort.Split(',').ToList();
                int num = 0;

                decimal jxScore = 0;//绩效得分

                for (int i = 0; i < strSp.Count() - 1; i++)
                {
                    if (sort.Contains(i.ToString()))
                    {
                        if (i == strSp.Count() - 2)
                        {
                            itemJson = itemJson.Insert(1, "\"title" + num + "\":" + strSp[i]);
                        }
                        else
                        {
                            itemJson = itemJson.Insert(1, "\"title" + num + "\":" + strSp[i] + ",");
                        }

                        num++;
                    }
                    if (i == 1)//第二个是绩效
                    {
                        decimal.TryParse(strSp[i], out jxScore);
                    }
                }

                itemJson = itemJson.Insert(1, "\"username\":\"" + item.username + "\",");
                itemJson = itemJson.Insert(1, "\"quarters\":\"" + item.quarters + "\",");
                resultinfo.Info = itemJson;
                resultinfo.Value = jxScore;
                resultinfo.Index = num;
                // resultinfo = itemJson.ToJObject();

                result.Add(resultinfo);
            }

            result = result.OrderByDescending(p => p.Value).ToList();
            var ki = new List<KeyIndex>();
            var valueList = result.Select(p => p.Value).Distinct().ToList();
            for (int i = 0; i < valueList.Count; i++)
            {
                ki.Add(new KeyIndex() { Value = valueList[i], Index = i + 1 });
            }

            for (int i = 1; i <= result.Count; i++)
            {
                result[i - 1].Info = result[i - 1].Info.Insert(1, "\"title" + result[i - 1].Index + "\":" + ki.FirstOrDefault(p => p.Value == result[i - 1].Value).Index.ToString() + ",");
            }
            var JsonData = new
            {
                rows = result.Select(p => p.Info.ToJObject()),
                records = data.Count,
                isup = isup ? "已提交" : ck == null ? "未生成绩效表" : "未提交",
                total = total,
                deptname = deptName
            };
            return Content(JsonData.ToJson());

        }

        /// <summary>
        /// 获取title
        /// </summary>
        /// <param name="time"></param>
        /// <param name="deptid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult getWork(string time, string deptid)
        {
            var ckTime = DateTime.Now.ToString("yyyy-MM-01") == time;
            if (ckTime)
            {
                var result = new
                {
                    monthcount = 0,
                    yearcount = 0,

                };
                return Content(JsonConvert.SerializeObject(result));
            }
            var dept = deptbll.GetEntity(deptid);
            if (deptid == "0")
            {
                dept = new DepartmentEntity() { EnCode = "0", IsSpecial = true };
            }
            if (dept.IsSpecial)
            {
                var month = upbll.getListByMonth(time).Where(x => x.isup == false).ToList();
                // var year = upbll.getListByYear(time.Split('-')[0]).ToList();
                var result = new
                {
                    monthcount = month.Count,
                    // yearcount = year.Count,

                };
                return Content(JsonConvert.SerializeObject(result));
            }
            else
            {
                var month = upbll.getListByMonth(time).Where(x => x.isup == false && x.deptcode.StartsWith(dept.EnCode)).ToList();
                //var year = upbll.getListByYear(time.Split('-')[0]).Where(x => x.parentid == deptid || dept.DepartmentId == deptid).ToList();
                var result = new
                {
                    monthcount = month.Count,
                    // yearcount = year.Count,

                };
                return Content(JsonConvert.SerializeObject(result));

            }

        }



        /// <summary>
        /// 获取title
        /// </summary>
        /// <param name="time"></param>
        /// <param name="deptid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult getupList(string time, string deptid)
        {


            var data = new List<PerformanceupEntity>();
            if (time.Split('-').Length > 1)
            {
                data = upbll.getListByMonth(time).Where(x => x.isup == false).ToList();
            }
            else
            {
                data = upbll.getListByYear(time).Where(x => x.isup == false).ToList();
            }

            var dept = deptbll.GetEntity(deptid);
            if (deptid == "0")
            {
                dept = new DepartmentEntity() { IsSpecial = true };
            }
            if (!dept.IsSpecial)
            {
                data = data.Where(x => x.deptcode.StartsWith(dept.EnCode)).ToList();
            }
            var cktime = new DateTime();

            if (time.Length < 6)
            {
                var now = DateTime.Now;
                cktime = new DateTime(int.Parse(time), now.Month, 1);
                data = data.Where(x => DateTime.Parse(x.usetime).CompareTo(cktime) < 0).ToList();

            }
            data = data.OrderBy(x => x.usetime).ThenBy(x => x.departmentname).ToList();
            var rows = new List<object>();
            var groupData = data.OrderBy(x => x.parentcode).GroupBy(x => x.parentcode);
            foreach (var item in groupData)
            {

                foreach (var orderData in item.OrderBy(x => x.deptcode))
                {
                    rows.Add(new
                    {
                        time = Convert.ToDateTime(orderData.usetime).ToString("yyyy年MM月"),
                        parentname = orderData.parentname,
                        departmentname = orderData.departmentname
                    });
                }
            }
            var JsonData = new
            {
                rows = rows,
                records = data.Count
            };
            return Content(JsonData.ToJson());

        }
        /// <summary>
        /// 获取title
        /// </summary>
        /// <param name="time"></param>
        /// <param name="deptid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetTitle(string time, string deptid)
        {
            var ckTime = DateTime.Now.ToString("yyyy-MM-01") == time;
            if (ckTime)
            {
                return Content("");
            }
            var list = default(List<string>);
            string category = "厂级,部门,班组";
            list = category.Split(',').ToList();
            var depts = bll.getDepartmentList("", category);
            if (deptid == "0")
            {

                foreach (var item in depts)
                {
                    if (item.Nature == "部门")
                    {
                        var dept = depts.Where(x => x.ParentId == item.DepartmentId).ToList();
                        if (dept.Count() > 0)
                        {
                            deptid = dept[0].DepartmentId;
                            break;
                        }

                    }

                }
            }
            else
            {

                var Nature = deptbll.GetEntity(deptid);
                if (Nature.Nature == "部门")
                {
                    deptid = depts.FirstOrDefault(x => x.ParentId == deptid).DepartmentId;
                }
                if (Nature.Nature == "厂级")
                {
                    foreach (var item in depts)
                    {
                        if (item.Nature == "部门")
                        {
                            var dept = depts.Where(x => x.ParentId == item.DepartmentId).ToList();
                            if (dept.Count() > 0)
                            {
                                deptid = dept[0].DepartmentId;
                                break;
                            }

                        }

                    }
                }

            }
            var data = titleBll.getTitle(time, deptid);
            if (data == null)
            {
                return Content("");
            }
            var title = data.name.Split(',').ToList();
            title.Add("绩效排名");
            return Content(JsonConvert.SerializeObject(title));
        }

        #region v2.0
        private PerformanceupSecondBLL upbllSecond = new PerformanceupSecondBLL();
        private PerformanceSecondBLL bllSecond = new PerformanceSecondBLL();
        private PerformancetitleSecondBLL titleBllSecond = new PerformancetitleSecondBLL();
        /// <summary>
        /// 展示页
        /// </summary>
        /// <returns></returns>
        public ViewResult IndexSecond()
        {
            var user = OperatorProvider.Provider.Current();
            var nowTime = DateTime.Now.AddMonths(-1);
            var time = new DateTime(nowTime.Year, nowTime.Month, 1);

            if (user.DeptId == "0")
            {
                ViewBag.DeptId = "0";
                ViewBag.parentid = "0";
            }
            else
            {
                var dept = deptbll.GetEntity(user.DeptId);
                if (!dept.IsSpecial)
                {
                    ViewBag.DeptId = user.DeptId;
                    ViewBag.parentid = user.DeptId;
                }
                else
                {
                    ViewBag.DeptId = "0";
                    ViewBag.parentid = "0";

                }


            }
            var tree = deptbll.GetAuthorizationDepartment(user.DeptId);
            ViewBag.treedept = tree.DepartmentId;
            ViewBag.gettime = time.ToString("yyyy-MM-dd");
            ViewBag.settime = time.Year + "年" + time.Month + "月";
            return View();
        }

        /// <summary>
        /// 展示页
        /// </summary>
        /// <returns></returns>
        public ViewResult IndexupSecond(string time, string deptid)
        {

            ViewBag.time = time;
            ViewBag.deptid = deptid;
            return View();
        }
        /// <summary>
        /// 获取title
        /// </summary>
        /// <param name="time"></param>
        /// <param name="deptid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult getupListSecond(string time, string deptid)
        {


            var data = new List<PerformanceupSecondEntity>();
            if (time.Split('-').Length > 1)
            {
                data = upbllSecond.getListByMonth(time).Where(x => x.isup == false).ToList();
            }
            else
            {
                data = upbllSecond.getListByYear(time).Where(x => x.isup == false).ToList();
            }

            var dept = deptbll.GetEntity(deptid);
            if (deptid == "0")
            {
                dept = new DepartmentEntity() { IsSpecial = true };
            }
            if (!dept.IsSpecial)
            {
                data = data.Where(x => x.deptcode.StartsWith(dept.EnCode)).ToList();
            }
            var cktime = new DateTime();

            if (time.Length < 6)
            {
                var now = DateTime.Now;
                cktime = new DateTime(int.Parse(time), now.Month, 1);
                data = data.Where(x => x.usetime.Value.CompareTo(cktime) < 0).ToList();

            }
            data = data.OrderBy(x => x.usetime).ThenBy(x => x.departmentname).ToList();
            var rows = new List<object>();
            var groupData = data.OrderBy(x => x.parentcode).GroupBy(x => x.parentcode);
            foreach (var item in groupData)
            {

                foreach (var orderData in item.OrderBy(x => x.deptcode))
                {
                    rows.Add(new
                    {
                        time = Convert.ToDateTime(orderData.usetime).ToString("yyyy年MM月"),
                        parentname = orderData.parentname,
                        departmentname = orderData.departmentname
                    });
                }
            }
            var JsonData = new
            {
                rows = rows,
                records = data.Count
            };
            return Content(JsonData.ToJson());

        }
        /// <summary>
        /// 获取title
        /// </summary>
        /// <param name="time"></param>
        /// <param name="deptid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult getWorkSecond(string time, string deptid)
        {
            var ckTime = DateTime.Now.ToString("yyyy-MM-01") == time;
            if (ckTime)
            {
                var result = new
                {
                    monthcount = 0,
                    yearcount = 0,

                };
                return Content(JsonConvert.SerializeObject(result));
            }
            var dept = deptbll.GetEntity(deptid);
            if (deptid == "0")
            {
                dept = new DepartmentEntity() { EnCode = "0", IsSpecial = true };
            }
            if (dept.IsSpecial)
            {
                var month = upbllSecond.getListByMonth(time).Where(x => x.isup == false).ToList();
                // var year = upbll.getListByYear(time.Split('-')[0]).ToList();
                var result = new
                {
                    monthcount = month.Count,
                    // yearcount = year.Count,

                };
                return Content(JsonConvert.SerializeObject(result));
            }
            else
            {
                var month = upbllSecond.getListByMonth(time).Where(x => x.isup == false && x.deptcode.StartsWith(dept.EnCode)).ToList();
                //var year = upbll.getListByYear(time.Split('-')[0]).Where(x => x.parentid == deptid || dept.DepartmentId == deptid).ToList();
                var result = new
                {
                    monthcount = month.Count,
                    // yearcount = year.Count,

                };
                return Content(JsonConvert.SerializeObject(result));

            }

        }
        /// <summary>
        /// 获取GetScore
        /// </summary>
        /// <param name="time"></param>
        /// <param name="deptid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetScoreSecond(string time, string deptid)
        {
            var ckTime = DateTime.Now.ToString("yyyy-MM-01") == time;

            var list = default(List<string>);
            string category = "厂级,部门,班组";
            list = category.Split(',').ToList();
            var depts = bllSecond.getDepartmentList("", category);
            string deptName = "";
            if (deptid == "0")
            {

                foreach (var item in depts)
                {
                    if (item.Nature == "部门")
                    {
                        var dept = depts.Where(x => x.ParentId == item.DepartmentId).ToList();
                        if (dept.Count() > 0)
                        {
                            deptid = dept[0].DepartmentId;
                            deptName = dept[0].FullName;
                            break;
                        }

                    }

                }
            }
            else
            {

                var Nature = deptbll.GetEntity(deptid);
                if (Nature.Nature == "部门")
                {
                    var one = depts.FirstOrDefault(x => x.ParentId == deptid);
                    deptid = one.DepartmentId;
                    deptName = one.FullName;
                }
                if (Nature.Nature == "厂级")
                {
                    foreach (var item in depts)
                    {
                        if (item.Nature == "部门")
                        {
                            var dept = depts.Where(x => x.ParentId == item.DepartmentId).ToList();
                            if (dept.Count() > 0)
                            {
                                deptid = dept[0].DepartmentId;
                                deptName = dept[0].FullName;
                                break;
                            }

                        }

                    }
                }
                if (Nature.Nature == "班组")
                {
                    deptName = Nature.FullName;
                }

            }
            if (ckTime)
            {
                var jsonStr = new
                {
                    rows = "",
                    records = 0,
                    isup = "未提交",
                    total = 0,
                    deptname = deptName
                };
                return Content(jsonStr.ToJson());
            }
            var worktime = Convert.ToDateTime(time);
            var data = bllSecond.getScore(worktime, deptid).OrderBy(x => x.planer).ThenBy(x => x.username).ToList();
            double total = 0;
            foreach (var item in data)
            {
                var score = Convert.ToDouble(item.score.Split(',')[0]);
                total = total + score;
            }
            var month = upbllSecond.getListByMonth(time).Where(x => x.departmentid == deptid).ToList();
            var ck = month.FirstOrDefault(x => x.departmentid == deptid);
            bool isup = ck == null ? false : ck.isup;
            List<SortModel> result = new List<SortModel>();
            foreach (var item in data)
            {
                var resultinfo = new SortModel();
                string itemJson = new object().ToJson();
                string myTitleStr = string.Empty;
                myTitleStr = item.score;
                var strSp = myTitleStr.Split(',');
                var sort = item.sort.Split(',').ToList();
                int num = 0;

                for (int i = 0; i < strSp.Count(); i++)
                {

                    if (i == 0)
                    {
                        itemJson = itemJson.Insert(1, "\"title" + num + "\":" + strSp[i]);
                    }
                    else
                    {
                        itemJson = itemJson.Insert(1, "\"title" + num + "\":" + strSp[i] + ",");
                    }

                    num++;
                }
                itemJson = itemJson.Insert(1, "\"username\":\"" + item.username + "\",");
                itemJson = itemJson.Insert(1, "\"quarters\":\"" + item.quarters + "\",");

                resultinfo.Info = itemJson;

                result.Add(resultinfo);
            }


            var JsonData = new
            {
                rows = result.Select(p => p.Info.ToJObject()),
                records = data.Count,
                isup = isup ? "已提交" : ck == null ? "未生成绩效表" : "未提交",
                total = total,
                deptname = deptName
            };
            return Content(JsonData.ToJson());

        }   /// <summary>
            /// 获取title
            /// </summary>
            /// <param name="time"></param>
            /// <param name="deptid"></param>
            /// <returns></returns>
        [HttpGet]
        public ActionResult GetTitleSecond(string time, string deptid)
        {
            var ckTime = DateTime.Now.ToString("yyyy-MM-01") == time;
            if (ckTime)
            {
                return Content("");
            }
            var list = default(List<string>);
            string category = "厂级,部门,班组";
            list = category.Split(',').ToList();
            var depts = bllSecond.getDepartmentList("", category);
            if (deptid == "0")
            {

                foreach (var item in depts)
                {
                    if (item.Nature == "部门")
                    {
                        var dept = depts.Where(x => x.ParentId == item.DepartmentId).ToList();
                        if (dept.Count() > 0)
                        {
                            deptid = dept[0].DepartmentId;
                            break;
                        }

                    }

                }
            }
            else
            {

                var Nature = deptbll.GetEntity(deptid);
                if (Nature.Nature == "部门")
                {
                    deptid = depts.FirstOrDefault(x => x.ParentId == deptid).DepartmentId;
                }
                if (Nature.Nature == "厂级")
                {
                    foreach (var item in depts)
                    {
                        if (item.Nature == "部门")
                        {
                            var dept = depts.Where(x => x.ParentId == item.DepartmentId).ToList();
                            if (dept.Count() > 0)
                            {
                                deptid = dept[0].DepartmentId;
                                break;
                            }

                        }

                    }
                }

            }
            var worktime = Convert.ToDateTime(time);
            var data = titleBllSecond.getTitle(worktime, deptid);
            if (data == null)
            {
                return Content("");
            }
            var title = data.name.Split(',').ToList();
            return Content(JsonConvert.SerializeObject(title));
        }
        #endregion

    }

    public class SortModel
    {
        public string Info { get; set; }
        public decimal Value { get; set; }
        /// <summary>
        /// 下标
        /// </summary>
        public int Index { get; set; }

    }

    public class KeyIndex
    {
        public decimal Value { get; set; }
        public int Index { get; set; }
    }
}
