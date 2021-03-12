using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Entity.WorkMeeting;
using BSFrameWork.Application.AppInterface.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class JobEvaluateController : BaseApiController
    {
        #region 构造方法
        /// <summary>
        /// 班组任务评价BLL
        /// </summary>
        private readonly JobEvaluateBLL JobEvaluatebll;
        /// <summary>
        /// User BLL
        /// </summary>
        private readonly UserBLL Userbll;

        private readonly ScoreBLL sbll;
        public JobEvaluateController()
        {
            JobEvaluatebll = new JobEvaluateBLL();
            Userbll = new UserBLL();
            sbll = new ScoreBLL();
        }
        #endregion

        /// <summary>
        /// 获取班会任务的考评项
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetAllEvaluateItem()
        {
            try
            {
                List<DataItemDetailEntity> dataItemDetails = new DataItemDetailBLL().GetDataItems("任务评价评分项");
                var data = dataItemDetails.Select(p => new JobEvaluateItem(p)).ToList();
                return new { Code = 0, Info = "查询成功", data = data };
            }
            catch (Exception ex)
            {
                return new { Code = -1, Info = "查询失败", data = ex.Message };
            }
        }

        /// <summary>
        /// 获取班会任务的考评项
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetEvaluateItem([FromBody]JObject json)
        {
            try
            {
                var rq = JsonConvert.DeserializeAnonymousType(json.Value<string>("json"), new
                {
                    userId = string.Empty,
                    data = new
                    {
                        jobId = string.Empty
                    }
                });
                if (string.IsNullOrWhiteSpace(rq.userId)) throw new ArgumentNullException("userId不能为空");
                if (rq.data == null) throw new ArgumentNullException("data不能为空");
                if (rq.data.jobId == null) throw new ArgumentNullException("data.jobId不能为空");
                UserEntity userEntity = Userbll.GetEntity(rq.userId);
                if (userEntity == null) throw new Exception("用户不存在");

                List<DataItemDetailEntity> pullList = new List<DataItemDetailEntity>();
                //取出所有的评分项
                List<DataItemDetailEntity> dataItemDetails = new DataItemDetailBLL().GetDataItems("任务评价评分项");
                //判断当前用户是否是工作负责人
                if (JobEvaluatebll.IsChecker(rq.userId, rq.data.jobId))
                    pullList.AddRange(dataItemDetails.Where(p => p.Description.StartsWith("普通")));
                //判断是否是班长
                if (userEntity.RoleName == "班长" || userEntity.RoleName == "班组长" || (userEntity.RoleName.Contains("班组级用户") && userEntity.RoleName.Contains("负责人")))
                    pullList.AddRange(dataItemDetails.Where(p => p.Description.StartsWith("特殊")));
                //判断是否有图片
                bool havePhoto = JobEvaluatebll.CheckPhoto(rq.data.jobId);
                var data = pullList.Select(p => new JobEvaluate(p));
                return new { Code = 0, Info = "查询成功", data = new { data, havePhoto } };
            }
            catch (Exception ex)
            {
                return new { Code = -1, Info = "查询失败", data = ex.Message };
            }
        }

        /// <summary>
        /// 班会任务的评论
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object JobEvaluate()
        {
            try
            {
                var rq = JsonConvert.DeserializeAnonymousType(HttpContext.Current.Request["json"], new
                {
                    userId = string.Empty,
                    data = new
                    {
                        evaluateItem = new List<JobEvaluate>(),
                        jobId = string.Empty
                    }
                });
                if (string.IsNullOrWhiteSpace(rq.userId)) throw new ArgumentNullException("userId不能为空");
                if (rq.data == null) throw new ArgumentNullException("data不能为空");
                if (rq.data.jobId == null) throw new ArgumentNullException("data.jobId不能为空");
                if (rq.data.evaluateItem == null) throw new ArgumentNullException("data.evaluateItem");
                UserEntity userEntity = Userbll.GetEntity(rq.userId);
                if (userEntity == null) throw new Exception("用户不存在");
                JobEvaluateEntity entity = new JobEvaluateEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    CreateDate = DateTime.Now,
                    CreateUserId = userEntity.UserId,
                    CreateUserName = userEntity.RealName,
                    JobId = rq.data.jobId,
                    State = true
                };

                //判断是否有图片
                //bool havePhoto = JobEvaluatebll.CheckPhoto(rq.data.jobId);
                int photoCount = JobEvaluatebll.CheckPhotoCount(rq.data.jobId);
                List<EvaluateItem> evaluateItems = new List<EvaluateItem>();
                rq.data.evaluateItem.ForEach(x =>
                {
                    EvaluateItem evaluateItem = new EvaluateItem()
                    {
                        SortCode = x.SortCode,
                        Title = x.Title,
                        Type = x.Type,
                    };
                    if (x.Option == null) throw new Exception("Option为空");
                    #region 旧 2020-06-10 分数不再受图片影响
                    //if (x.Type == "特殊")
                    //{
                    //    evaluateItem.Score = x.Option.ScoreList[x.Option.Name.IndexOf(x.Option.Value)];
                    //    if (evaluateItem.Score < 0)
                    //        evaluateItem.EvaluateContent = $" {x.Option.Value}  扣{  Math.Abs(evaluateItem.Score) }分";
                    //    else
                    //        evaluateItem.EvaluateContent = $" {x.Option.Value}  加{  evaluateItem.Score }分";

                    //}
                    //else
                    //{
                    //    if (photoCount>0)
                    //    {
                    //        evaluateItem.Score = x.Option.ScoreList[x.Option.Name.IndexOf(x.Option.Value)];

                    //        if (x.Title == "现场安全措施交底")
                    //        {
                    //            evaluateItem.Score = x.Option.ScoreList[x.Option.Name.IndexOf(x.Option.Value)] * photoCount;
                    //            evaluateItem.Score = evaluateItem.Score > 2 ? 2 : evaluateItem.Score;
                    //        }
                    //        evaluateItem.EvaluateContent = $" {x.Option.Value}  加{  evaluateItem.Score }分";
                    //    }
                    //    else
                    //    {
                    //        evaluateItem.Score = 0;
                    //        evaluateItem.EvaluateContent = $" {x.Option.Value}  加{  evaluateItem.Score }分 缺少图片支持";
                    //    }
                    //}
                    #endregion
                    #region 新  2020-06-10
                    evaluateItem.Score = x.Option.ScoreList[x.Option.Name.IndexOf(x.Option.Value)];
                    if (evaluateItem.Score < 0)
                        evaluateItem.EvaluateContent = $" {x.Option.Value}  扣{  Math.Abs(evaluateItem.Score) }分";
                    else
                        evaluateItem.EvaluateContent = $" {x.Option.Value}  加{  evaluateItem.Score }分";
                    #endregion
                    evaluateItems.Add(evaluateItem);
                });
                entity.EvaluateItems = evaluateItems;
                entity.TotalScore = evaluateItems.Sum(p => p.Score);
                JobEvaluatebll.Insert(entity);
                return new { Code = 0, Info = "操作成功" };
            }
            catch (Exception ex)
            {
                return new { Code = -1, Info = "操作失败", data = ex.Message };
            }
        }

        /// <summary>
        /// 分页获取任务评分列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetJobPageList()
        {
            try
            {
                var rq = JsonConvert.DeserializeAnonymousType(HttpContext.Current.Request["json"], new
                {
                    userId = string.Empty,
                    pageIndex = 1,
                    pageSize = 5,
                    data = new
                    {
                        evaluateState = string.Empty,
                        isMy = false,
                        startTime = string.Empty,
                        endTime = string.Empty,
                        deptId = string.Empty,
                        keyWord = string.Empty
                    }
                });
                int total = 0;
                System.Collections.IList a = JobEvaluatebll.GetJobPageList(rq.pageIndex, rq.pageSize, rq.userId, rq.data.evaluateState, rq.data.isMy, rq.data.startTime, rq.data.endTime, rq.data.deptId, rq.data.keyWord, out total);
                return new { Code = 0, Info = "查询成功", data = a, count = total };
            }
            catch (Exception ex)
            {
                return new { Code = -1, Info = "查询失败", data = ex.Message };
            }
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetJobDetail([FromBody]JObject json)
        {
            try
            {
                var rq = JsonConvert.DeserializeAnonymousType(json.Value<string>("json"), new
                {
                    userId = string.Empty,
                    data = new
                    {
                        jobId = string.Empty,
                    }
                });
                if (rq.data == null) throw new ArgumentNullException("data不能为空");
                if (rq.data.jobId == null) throw new ArgumentNullException("data.jobId不能为空");
                object jobDetail = JobEvaluatebll.GetJobDetail(rq.data.jobId);
                return new { Code = 0, Info = "查询成功", data = jobDetail };
            }
            catch (Exception ex)
            {
                return new { Code = -1, Info = "查询失败", data = ex.Message };
            }
        }

        /// <summary>
        /// 获取任务的评价详情
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetJobEvaluat([FromBody]JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic rq = JsonConvert.DeserializeObject<ExpandoObject>(res);
                if (rq.data == null) throw new ArgumentNullException("data不能为空");
                if (rq.data.jobId == null) throw new ArgumentNullException("data.jobId不能为空");
                string jobId = rq.data.jobId;


                //JobEvaluateEntity jobEvaluate = JobEvaluatebll.GetJobEvaluat(jobId);
                JobEvaluateEntity jobEvaluate = new JobEvaluateEntity();
                List<JobEvaluateEntity> jobs = JobEvaluatebll.GetAllByJobId(jobId);
                if (jobs != null && jobs.Count > 0)
                {
                    jobs.ForEach(p =>
                    {
                        jobEvaluate.EvaluateItems.AddRange(p.EvaluateItems);
                    });
                }
                return new { Code = 0, Info = "查询成功", data = jobEvaluate };
            }
            catch (Exception ex)
            {
                return new { Code = -1, Info = "查询失败", data = ex.Message };
            }
        }

        /// <summary>
        /// 获取任务评价的开启状态
        /// </summary>
        /// <returns></returns>

        #region 班组终端统计
        [HttpPost]
        public object GetMonthStatistics2()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var data = JsonConvert.DeserializeObject<ParamBucket<data>>(json);
            if (data.Data != null)
            {
                if (data.Data.year == 0)
                {
                    data.Data.year = DateTime.Now.Year;
                }
                if (data.Data.month == 0)
                {
                    data.Data.month = DateTime.Now.Month;
                }
                data.Data.userKey = data.UserId;
            }
            var model = data.Data;
            var finish = JobEvaluatebll.GetFinishTaskCount(model.deptId, model.year, model.month);
            var unfinish = JobEvaluatebll.GetUnfinishTaskCount(model.deptId, model.year, model.month);
            monthStatistics entity = new monthStatistics();
            entity.finish = finish;
            entity.unfinish = unfinish;
            entity.finishpercent = (Math.Round(JobEvaluatebll.GetScore4(model.deptId, model.year, model.month), 4)).ToString("p");
            Hashtable ht = JobEvaluatebll.GetTotalScore(model.deptId, model.year, model.month);
            entity.totalscore = Convert.ToDecimal(ht["TotalScore"]);
            entity.avgfinish = JobEvaluatebll.GetAvgTaskCount(model.deptId, model.year, model.month);
            entity.avgscore = Convert.ToDecimal(ht["AvgScore"]);
            return new { code = result, info = message, data = entity, count = 1 };
        }

        /// <summary>
        /// 月度统计
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //月度统计
        public object GetMonthStatistics()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var data = JsonConvert.DeserializeObject<ParamBucket<data>>(json);
            if (data.Data != null)
            {
                if (data.Data.year == 0)
                {
                    data.Data.year = DateTime.Now.Year;
                }
                if (data.Data.month == 0)
                {
                    data.Data.month = DateTime.Now.Month;
                }
                data.Data.userKey = data.UserId;
            }
            var model = data.Data;
            var score = new List<UserScoreEntity>();
            if (model.type == "1")
            {
                score = JobEvaluatebll.GetPersonScore(model.deptId, model.year, model.month);
                return new { code = result, info = message, data = score, count = score.Count };
            }
            if (model.type == "2")
            {
                score = JobEvaluatebll.GetScore2(model.deptId, model.year, model.month);
                return new { code = result, info = message, data = score, count = score.Count };
            }
            if (model.type == "3")
            {
                score = sbll.GetScore3(model.deptId, model.year, model.month);
                return new { code = result, info = message, data = score, count = score.Count };
            }
            if (model.type == "4")//工时统计
            {
                score = sbll.GetTaskHourStatistics(model.deptId, model.year, model.month);
                return new { code = result, info = message, data = score, count = score.Count };
            }
            return new { data = "Errow" };
        }


        //任务评分统计 參數：用户id， 年
        [HttpPost]
        public object GetPersonScore()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = JsonConvert.DeserializeObject<ParamBucket<data>>(json);
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
                model.Data.userKey = model.UserId;
            }
            var data = JobEvaluatebll.GetPersonScore(model.Data.userKey, model.Data.year.ToString());
            return new { code = result, info = message, data = data};

        }

        [HttpPost]
        //获取任務評分列表
        public object GetPerson()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = JsonConvert.DeserializeObject<ParamBucket<data>>(json);
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
                model.Data.userKey = model.UserId;
            }
            var person = JobEvaluatebll.PersonJobsObject(model.Data.userKey, model.Data.year, model.Data.month);
            return new { code = result, info = message, data = person, count = person.Count() };
        }


        [HttpPost]
        //评分统计
        public object GetStatistics()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = JsonConvert.DeserializeObject<ParamBucket<data>>(json);
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
                model.Data.userKey = model.UserId;
            }
            var data = JobEvaluatebll.GetData1(model.Data.deptId, model.Data.year, model.Data.month);
            return new { code = result, info = message, data, count = data.Count };
        }

        #region 方法


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

        public class monthStatistics
        {
            public int finish { get; set; }
            public int unfinish { get; set; }
            public String finishpercent { get; set; }
            public decimal totalscore { get; set; }
            public decimal avgfinish { get; set; }
            public decimal avgscore { get; set; }
        }
        #endregion
        #endregion
    }
}
