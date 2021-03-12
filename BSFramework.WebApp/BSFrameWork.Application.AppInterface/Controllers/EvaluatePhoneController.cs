using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.EvaluateAbout;
using BSFramework.Application.Busines.PeopleManage;
//using BSFramework.Application.Busines.HiddenTroubleManage;
//using BSFramework.Application.Busines.RiskDatabase;
//using BSFramework.Application.Busines.SaftyCheck;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.SystemManage;
//using BSFramework.Application.Web.Areas.Works.Controllers;
using BSFramework.Busines.EvaluateAbout;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Entity.EvaluateAbout;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util;
using BSFrameWork.Application.AppInterface.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{

    public class EvaluatePhoneController : BaseApiController
    {
        private UserBLL userbll = new UserBLL();
        private ScoreBLL sbll = new ScoreBLL();
        private DepartmentBLL deptbll = new DepartmentBLL();
        private PeopleBLL pbll = new PeopleBLL();
        private WorkmeetingBLL wbll = new WorkmeetingBLL();

        string strUserId = "";
        public data getModel()
        {
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<data>>(json);
            strUserId = model.UserId;
            if (model.Data != null)
            {
                if (model.Data.year == 0)
                {
                    model.Data.year = DateTime.Now.Year;
                }
                if (model.Data.month == 0)
                {
                    model.Data.month = DateTime.Now.Month;
                }
            }
            return model.Data;
        }



        //获取季度名称和id
        [HttpPost]
        public object GetQuarter()
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            var bll = new EvaluateBLL();
            var evaluations = bll.GetEvaluations(null, null, 20, 1, out total).ToList();
            return new { code = result, info = message, data = evaluations, count = total };
        }

        //http://localhost/bzapp/api/EvaluatePhone/GetList?departKey=46a6af5e-65cb-40f9-b551-f9e7ff8bbfb5
        //获取8个项目的评分 參數：账号
        [HttpPost]
        public object GetList()
        {
            var result = 0;
            var message = string.Empty;
            var model = getModel();

            DepartmentEntity depart = deptbll.GetEntity(model.deptId);
            var bll = new EvaluateBLL();
            var total = 0;
            var data = bll.GetCalcScore2("", depart.DepartmentId);


            for (int i = data.Count - 1; i > -1; i--)
            {
                if (!String.IsNullOrEmpty(model.SeasonId))
                {
                    if (data[i].SeasonId != model.SeasonId)
                    {
                        data.RemoveAt(i);
                    }
                }
            }
            return new { code = result, info = message, data = data, count = data.Count };
        }

        //http://localhost/bzapp/api/EvaluatePhone/GetEvaluations?account=dqecb
        //获取四个季度和对应的评分和排名 參數：账号
        [HttpPost]
        public object GetEvaluations()
        {
            try
            {
                var result = 0;
                var message = string.Empty;
                var model = getModel();
                //这里不仅仅是获取当前班组的数据
                //UserInfoEntity currUser = userbll.GetUserInfoEntity(strUserId);
                var bll = new EvaluateBLL();
                var deptbll = new DepartmentBLL();
                var total = 0;
                var dept = deptbll.GetEntity(model.deptId);
                List<EvaluateEntity> evaluations = bll.GetEvaluations(null, null, 100, 1, out total).Where(x => x.EvaluateId == model.SeasonId).ToList();
                var pcts = bll.GetCalcScore2("", model.deptId);
                var evaluate = new List<EvaluateGroupEntity>();
                if (evaluations.Count > 0)
                {
                    foreach (EvaluateEntity a in evaluations)
                    {
                        evaluate = bll.GetCalcScore(a.EvaluateId, null).ToList();
                        var current = evaluate.FirstOrDefault(x => x.GroupName == dept.FullName);
                        //===解决班组终端考评排名超过本部门所有班组数量的问题 heming 2020-04-26  开始===//
                        evaluate.RemoveAll(p => p.DeptId != current.DeptId); //剔除本部门外的班组
                                                                             //===== 结束=======//
                        if (current == null) current = new EvaluateGroupEntity();
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

                        List<EvaluateGroupEntity> list = new List<EvaluateGroupEntity>();
                        EvaluateGroupEntity entity = new EvaluateGroupEntity();
                        entity.Pct = seq;
                        entity.ActualScore = current.ActualScore;
                        list.Add(entity);
                        a.Groups = list;

                    }

                }

                return new { code = result, info = message, data = evaluations, count = evaluations.Count };
            }
            catch (Exception ex)
            {
                string data = null;
                return new { code = 0, info = ex.Message, data, count = 0 };
            }

        }
        //获取部门 參數：无
        [HttpPost]
        public object GetDepart()
        {
            var result = 0;
            var message = string.Empty;


            var bll = new EvaluateBLL();
            var groups = deptbll.GetAllGroups().ToList();
            return new { code = result, info = message, data = groups, count = groups.Count };
        }

        //任务评分统计 參數：用户id， 年
        [HttpPost]
        public object GetPersonScore()
        {
            var user = OperatorProvider.Provider.Current();

            var result = 0;
            var message = string.Empty;
            var model = getModel();
            //ScoreController sc = new ScoreController();
            //var data = sc.GetPersonScore(model.userKey, model.year.ToString());

            var data1 = sbll.GetPersonScore(model.userKey, model.year);
            var data2 = sbll.GetDeptScoreAvg(user.DeptId, model.year);

            //return new { code = result, info = message, data = new { data = data1, data1 = data2 }, count = 2 };
            return new { data = data1, data1 = data2 };
        }
        [HttpPost]
        //获取部门下人员
        public object GetUser()
        {
            var result = 0;
            var message = string.Empty;
            var model = getModel();
            var currUser = userbll.GetEntity(strUserId);
            var dpusers = new List<dynamic>();
            if (currUser != null)
            {
                var users = userbll.GetDeptUsers(currUser.DepartmentId).ToList();
                foreach (UserEntity u in users)
                {
                    dpusers.Add(new { value = u.UserId, text = u.RealName });
                }
            }
            return new { code = result, info = message, data = dpusers, count = dpusers.Count };
        }

        [HttpPost]
        //获取下拉框时间年
        public object GetTimeYear()
        {
            var result = 0;
            var message = string.Empty;
            var years = new List<dynamic>();
            for (int i = 0; i < 10; i++)
            {
                years.Add(new { value = (DateTime.Now.Year - i).ToString(), text = (DateTime.Now.Year - i).ToString() });
            }
            return new { code = result, info = message, data = years, count = years.Count };
        }
        [HttpPost]
        //获取下拉框时间月
        public object GetTimeMonth()
        {
            var result = 0;
            var message = string.Empty;
            var months = new List<dynamic>();
            for (int i = 1; i <= 12; i++)
            {
                months.Add(new { value = i.ToString(), text = (i + "月") });
            }
            return new { code = result, info = message, data = months, count = months.Count };
        }
        [HttpPost]
        //获取任務評分列表
        public object GetPerson()
        {
            var result = 0;
            var message = string.Empty;
            var model = getModel();
            var person = sbll.PersonJobsObject(model.userKey, model.year, model.month);
            return new { code = result, info = message, data = person, count = person.Count() };
        }
        [HttpPost]
        //评分统计
        public object GetStatistics()
        {
            var result = 0;
            var message = string.Empty;
            var model = getModel();
            var data = sbll.GetData1(model.deptId, model.year, model.month);
            //var users = userbll.GetDeptUsers(model.deptId).ToList();
            //var peoples = new List<PeopleEntity>();
            //foreach (UserEntity u in users)
            //{
            //    var p = pbll.GetEntity(u.UserId);
            //    if (p != null)
            //    {
            //        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            //        stopwatch.Start();
            //        p.Jobs = sbll.PersonFinishCount(u.UserId, model.year, model.month).ToString();
            //       p.Percent = Convert.ToInt32(sbll.PersonPercent(u.UserId, model.year, model.month)).ToString();
            //        p.Scores = sbll.PersonTotalScore(u.UserId, model.year, model.month).ToString();
            //        peoples.Add(p);
            //        stopwatch.Stop();
            //        var sec = stopwatch.Elapsed.TotalSeconds;
            //    }
            //}

            return new { code = result, info = message, data, count = data.Count };
        }
        [HttpPost]
        //月度统计
        public object GetMonthStatistics()
        {
            var result = 0;
            var message = string.Empty;
            var model = getModel();
            var score = new List<UserScoreEntity>();
            if (model.type == "1")
            {
                score = sbll.GetScore1(model.deptId, model.year, model.month);
                return new { code = result, info = message, data = score, count = score.Count };
            }
            if (model.type == "2")
            {
                score = sbll.GetScore2(model.deptId, model.year, model.month);
                return new { code = result, info = message, data = score, count = score.Count };
            }
            if (model.type == "3")
            {
                score = sbll.GetScore3(model.deptId, model.year, model.month);
                return new { code = result, info = message, data = score, count = score.Count };
            }
            return new { data = "Errow" };
        }
        [HttpPost]
        public object GetMonthStatistics2()
        {
            var result = 0;
            var message = string.Empty;
            var model = getModel();
            var finish = sbll.GetFinishTaskCount(model.deptId, model.year, model.month);
            var unfinish = sbll.GetUnfinishTaskCount(model.deptId, model.year, model.month);
            monthStatistics entity = new monthStatistics();
            entity.finish = finish;
            entity.unfinish = unfinish;
            entity.finishpercent = (Math.Round(sbll.GetScore4(model.deptId, model.year, model.month), 4)).ToString("p");
            entity.totalscore = sbll.GetTotalScore(model.deptId, model.year, model.month);
            entity.avgfinish = sbll.GetAvgTaskCount(model.deptId, model.year, model.month);
            entity.avgscore = sbll.GetAvgScore(model.deptId, model.year, model.month);
            return new { code = result, info = message, data = entity, count = 1 };
        }


        ///<summary>
        ///区间获取本月任务完成率
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetTimeStatistics(BaseDataModel<timedata> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                var score = sbll.GetScore4(dy.data.deptId, dy.data.starttime, dy.data.endtime);
                var finish = sbll.GetFinishTaskCount(dy.data.deptId, dy.data.starttime, dy.data.endtime);
                var unfinish = sbll.GetUnfinishTaskCount(dy.data.deptId, dy.data.starttime, dy.data.endtime);
                return new { code = result, info = message, data = new { score = score, finish = finish, unfinish = unfinish }, count = 1 };

            }
            catch (Exception ex)
            {
                message = ex.Message;
                result = 1;
                return new { code = result, info = message, count = 1 };

            }


        }

        public class monthStatistics
        {
            public int finish { get; set; }
            public int unfinish { get; set; }
            public String finishpercent { get; set; }
            public decimal totalscore { get; set; }
            public decimal avgfinish { get; set; }
            public decimal avgscore { get; set; }
        }







        //修改詳情
        [HttpPost]
        public object updateMeetingJob()
        {
            var result = 0;
            var message = string.Empty;
            var model = getModel();
            wbll.UpdateScoreState(model.MeetingJobId, model.userKey, model.Score, model.IsFinished);
            return new { code = result, info = message, data = "" };
        }


        public class data
        {
            //public String account { get; set; } 
            public String userKey { get; set; }
            public int year { get; set; }
            public int month { get; set; }
            public String deptId { get; set; }
            public String type { get; set; }
            public String key { get; set; }
            public String job { get; set; }
            public DateTime starttime { get; set; }
            public String IsFinished { get; set; }
            public String Score { get; set; }
            public String SeasonId { get; set; }
            public string MeetingJobId { get; set; }

        }

        public class timedata
        {

            public DateTime starttime { get; set; }
            public DateTime endtime { get; set; }
            public String deptId { get; set; }

        }

        [HttpPost]
        public object GetEvaluateCategory()
        {
            var result = 0;
            var message = string.Empty;
            var bll = new EvaluateBLL();
            var list = bll.GetBigCategories().ToList();
            return new { code = result, info = message, data = list, count = list.Count };
        }

        [HttpPost]
        public object GetOppositeScore()
        {

            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
            string evaluateId = dy.data.evaluateId;
            string categoryId = dy.data.categoryId;
            string userId = dy.userId;
            string type = dy.data.type;
            string info = "成功";
            int code = 0;
            var bll = new EvaluateBLL();

            var list = new List<EvaluateGroupEntity>();
            if (type == "0")
            {
                list = bll.GetCalcScore(evaluateId, categoryId).ToList();
            }
            else if (type == "1")
            {
                PeopleEntity p = pbll.GetEntity(userId);
                if (p != null)
                {
                    list = bll.GetCalcScore(evaluateId, categoryId).Where(x => x.DeptId == p.DeptId).ToList();

                }
                else
                {
                    info = "用户信息错误";
                    code = 1;
                }
            }
            return new { code = code, info = info, data = list, count = list.Count };
        }
        [HttpPost]
        public object GetScoreNew1()
        {
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
            string evaluateId = dy.data.evaluateId;
            string categoryId = dy.data.categoryId;
            string userId = dy.userId;
            string info = "成功";
            int code = 0;
            var bll = new EvaluateBLL();

            List<EvaluateScoreDetail> list = new List<EvaluateScoreDetail>();
            PeopleEntity p = pbll.GetEntity(userId);
            if (p != null)
            {
                list = bll.EvaluateScoreDetail(evaluateId, categoryId, p.BZID);
            }
            else
            {
                info = "用户信息错误";
                code = 1;
            }
            return new { code = code, info = info, data = list, count = list.Count };
        }

        [HttpPost]
        public object GetScoreNew()
        {
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
            string evaluateId = dy.data.evaluateId;
            string categoryId = dy.data.categoryId;
            string userId = dy.userId;
            string info = "成功";
            int code = 0;
            var bll = new EvaluateBLL();

            var list = new List<EvaluateScoreItemEntity>();
            PeopleEntity p = pbll.GetEntity(userId);
            if (p != null)
            {
                list = bll.GetCalcScoreItme(evaluateId, categoryId, p.BZID).Where(x => x.ActualScore != x.Score).ToList();//

                EvaluateCategoryItemEntity ei = new EvaluateCategoryItemEntity();
                EvaluateCategoryEntity c = new EvaluateCategoryEntity();
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                foreach (EvaluateScoreItemEntity e in list)
                {
                    ei = bll.GetCategoryItem(e.CategoryItemId);
                    if (ei != null)
                    {
                        e.Category = ei.Category.Category;
                    }
                    else
                    {
                        info = "考评内容为null";
                        code = 1;
                    }
                }
                stopwatch.Stop();
                var sec = stopwatch.Elapsed.TotalSeconds;
            }
            else
            {
                info = "用户信息错误";
                code = 1;
            }
            return new { code = code, info = info, data = list, count = list.Count };
        }
        [HttpPost]
        public object GetGroupTitle([FromBody]JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                if (dy == null) return new { code = -1, info = "查询失败", Message = "参数为空" };
                string deptId = dy.deptid;
                string evaluatId = dy.evaluatid;
                EvaluateGroupTitleBLL titleBLL = new EvaluateGroupTitleBLL();
                string name = titleBLL.GetTitleNameByGroupId(deptId, evaluatId);

                return new { code = 0, info = "查询成功", data = name };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "查询失败", ex.Message };
            }
        }
    }
}