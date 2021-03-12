using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PerformanceManage;
using BSFramework.Application.IService.PerformanceManage;
using BSFramework.Application.Service.PerformanceManage;
using BSFramework.IService.WorkMeeting;
using BSFramework.Service.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.PerformanceManage
{
    /// <summary>
    /// 绩效管理配置
    /// </summary>
    public class PerformancesetupBLL
    {
        /// <summary>
        /// 绩效管理配置
        /// </summary>
        private IPerformancesetupService service = new PerformancesetupService();
        private PerformancetitleBLL titlebll = new PerformancetitleBLL();
        private PerformanceBLL Scorebll = new PerformanceBLL();
        private UserBLL userbll = new UserBLL();
        private PeopleBLL pbll = new PeopleBLL();
        private PerformanceupBLL upbll = new PerformanceupBLL();
        private DepartmentBLL deptbll = new DepartmentBLL();
        /// <summary>
        /// 绩效管理配置 数据操作
        /// </summary>
        /// <param name="add">新增</param>
        /// <param name="del">删除</param>
        /// <param name="Listupdate">修改</param>
        /// <param name="title">当月标题头</param>
        /// <param name="Score">当月数据修正</param>
        public void operation(string time, string departmentid, List<PerformancesetupEntity> add, List<PerformancesetupEntity> del, List<PerformancesetupEntity> Listupdate)
        {
            try
            {
                //所有配置
                var setUp = service.AllTitle(departmentid);
                //获取标题
                PerformancetitleEntity title = titlebll.getTitle(time, departmentid);
                //获取数据
                List<PerformanceEntity> Score = Scorebll.getScore(time, departmentid);
                // 在处理删除 在处理新增
                #region 修改
                foreach (var item in Listupdate)
                {
                    //修改title
                    var old = setUp.First(x => x.performancetypeid == item.performancetypeid);
                    if (item.name != old.name)
                    {
                        old.name = item.name;
                    }
                    if (item.isuse != old.isuse)
                    {
                        old.isuse = item.isuse;
                    }
                }
                #endregion
                #region 删除
                //根据序号去对数据进行更正
                foreach (var item in del)
                {
                    var count = setUp.Count();
                    var delsort = setUp.First(x => x.performancetypeid == item.performancetypeid);

                    ////将后面数据重新排序
                    //for (int i = delsort.sort + 1; i < count; i++)
                    //{
                    //    //配置操作
                    //    if (Listupdate.Select(x => x.performancetypeid).ToList().Contains(setUp[i].performancetypeid))
                    //    {
                    //        var one = Listupdate.First(x => x.performancetypeid == setUp[i].performancetypeid);
                    //        one.sort = setUp[i].sort - 1;
                    //        setUp[i].sort = setUp[i].sort + 1;
                    //    }
                    //    else
                    //    {
                    //        setUp[i].sort = setUp[i].sort - 1;
                    //        Listupdate.Add(setUp[i]);
                    //    }
                    //}
                    //数据操作
                    foreach (var scoreItem in Score)
                    {
                        var ScoreInfo = scoreItem.score.Split(',').ToList();
                        //找出所在位置
                        //var num = setUp.IndexOf(delsort);
                        //if (num != -1)
                        //{
                        ScoreInfo[delsort.sort] = "0";
                        // ScoreInfo.RemoveAt(delsort.sort);
                        scoreItem.score = string.Join(",", ScoreInfo);
                        //}
                    }
                    setUp.Remove(delsort);
                }
                #endregion
                #region 新增

                foreach (var item in add)
                {
                    var typeList = setUp.Where(x => x.type == 1);
                    var sortType = typeList.Count();
                    //如果为第一类型
                    if (item.type == 1)
                    {

                        var count = setUp.Count();
                        ////将后面数据重新排序
                        //for (int i = sortType; i < count; i++)
                        //{
                        //    if (Listupdate.Select(x => x.performancetypeid).ToList().Contains(setUp[i].performancetypeid))
                        //    {
                        //        var one = Listupdate.First(x => x.performancetypeid == setUp[i].performancetypeid);
                        //        one.sort = setUp[i].sort + 1;
                        //        setUp[i].sort = setUp[i].sort + 1;
                        //    }
                        //    else
                        //    {
                        //        setUp[i].sort = setUp[i].sort + 1;
                        //        Listupdate.Add(setUp[i]);
                        //    }
                        //}
                        item.sort = sortType;
                        item.performancetypeid = Guid.NewGuid().ToString();
                        item.departmentid = departmentid;
                        item.createtime = DateTime.Now;
                        setUp.Add(item);
                        setUp = setUp.OrderBy(x => x.sort).ToList();

                        ////数据操作
                        //foreach (var scoreItem in Score)
                        //{
                        //    var ScoreInfo = scoreItem.score.Split(',').ToList();
                        //    //找出所在位置
                        //    var num = setUp.IndexOf(item);
                        //    if (num != -1)
                        //    {
                        //        //处理数据
                        //        var frist = ScoreInfo.Take(num).ToList();
                        //        var lastnum = ScoreInfo.Count();
                        //        var last = ScoreInfo.Skip(num).Take(lastnum - num).ToList();
                        //        ScoreInfo.Clear();
                        //        ScoreInfo.AddRange(frist);
                        //        ScoreInfo.Add("0");
                        //        ScoreInfo.AddRange(last);
                        //        scoreItem.score = string.Join(",", ScoreInfo);
                        //    }

                        //}

                    }
                    else
                    {
                        var count = setUp.Count();
                        item.departmentid = departmentid;
                        item.createtime = DateTime.Now;
                        if (sortType == 3)
                        {
                            item.sort = count;
                        }
                        else
                        {
                            item.sort = count + 1;
                        }

                        item.performancetypeid = Guid.NewGuid().ToString();
                        setUp.Add(item);
                        setUp = setUp.OrderBy(x => x.sort).ToList();
                        //加入数据
                        //数据操作
                        //foreach (var scoreItem in Score)
                        //{
                        //    var ScoreInfo = scoreItem.score.Split(',').ToList();
                        //    ScoreInfo.Add("0");
                        //    scoreItem.score = string.Join(",", ScoreInfo);
                        //}

                    }


                }

                #endregion
                var sort = setUp.Where(x => x.isuse).ToList();
                foreach (var item in Score)
                {
                    item.sort = string.Join(",", sort.Select(x => x.sort));
                }
                title.sort = string.Join(",", sort.Select(x => x.sort));
                title.name = string.Join(",", sort.Select(x => x.name));
                service.operation(add, del, Listupdate, title, Score);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 获取所有配置
        /// </summary>
        /// <returns></returns>
        public List<PerformancesetupEntity> AllTitle(string departmentid)
        {
            return service.AllTitle(departmentid);
        }
        /// <summary>
        ///初始化基础数据
        /// </summary>
        public void BaseDataOperation(List<DepartmentEntity> entity)
        {
            try
            {

                var time = DateTime.Now;
                var usetime = new DateTime(time.Year, time.Month, 1).ToString("yyyy-MM-dd");
                var dept = deptbll.GetList();



                foreach (var item in entity)
                {
                    //所有配置
                    var setUp = service.AllTitle(item.DepartmentId);
                    var sortStr = string.Join(",", setUp.Where(x => x.isuse).Select(x => x.sort));
                    PerformancetitleEntity title = titlebll.getTitle(usetime, item.DepartmentId);

                    if (title != null)
                    {
                        // #region 何明 2019-09-20
                        //// var deptIds = entity.Select(p => p.DepartmentId).ToList();
                        List<PerformanceEntity> allData = new PerformanceBLL().getScore(usetime, item.DepartmentId);
                        var alluserList = new UserBLL().GetUserList().Where(x => x.DepartmentId == item.DepartmentId && x.DeleteMark != 1 && x.EnabledMark == 1).ToList();//所有人员
                        var delData = new List<PerformanceEntity>();
                        var addData = new List<PerformanceEntity>();
                        // #endregion

                        // //根据人员，来做对应的数据处理

                        // //检查各部门人员

                        var thisUsers = alluserList.Where(p => p.DepartmentId == item.DepartmentId).Select(x => x.UserId).ToList();
                        // if (Debugger.IsAttached && (thisUsers.Contains("18d4ee5a-e239-49c3-b018-993cb711d0e4") || thisUsers.Contains("d15d843d-85e1-40c7-bcae-02f705855a39")))
                        // {
                        //     //拿用户熊红做测试用
                        //     var a = string.Empty;

                        // }
                        // //找到已经不在本班组的数据，并删除数据
                        var thisDelList = allData.Where(p => p.deparmentid == item.DepartmentId && !thisUsers.Contains(p.userid)).ToList();
                        delData.AddRange(thisDelList);
                        // //找到本班组需要新增的人
                        var addedUsers = allData.Where(p => p.deparmentid == item.DepartmentId).Select(x => x.userid).ToList();
                        var addUsers = alluserList.Where(p => p.DepartmentId == item.DepartmentId && !addedUsers.Contains(p.UserId)).ToList();//不在数据内的用户的Id
                                                                                                                                              //构造新增实体
                        foreach (var user in addUsers)
                        {
                            var one = new PerformanceEntity();
                            one.performanceid = Guid.NewGuid().ToString();
                            one.score = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
                            one.userid = user.UserId;
                            one.usetime = usetime;
                            one.deparmentid = item.DepartmentId;
                            one.titleid = title.titleid;
                            one.sort = sortStr;
                            one.deparmentname = item.FullName;
                            one.username = user.RealName;
                            one.photo = user.Photo;
                            one.quarters = user.Quarters;
                            one.planer = user.Planer;
                            addData.Add(one);
                        }
                        if (addData != null && addData.Count > 0)
                        {
                            Scorebll.Insert(addData);//新增的数据
                            //if (addData.Any(x => x.userid == "d15d843d-85e1-40c7-bcae-02f705855a39"))
                            //{
                            //    var b = "";
                            //}
                        }
                        if (delData != null && delData.Count > 0)
                        {
                            Scorebll.Delete(delData);//删除的数据
                        }

                    }
                    else
                    {


                        if (setUp.Count > 0)
                        {
                            var sort = setUp.Where(x => x.isuse).ToList();
                            PerformancetitleEntity Title = new PerformancetitleEntity();
                            Title.departmentid = item.DepartmentId;
                            Title.name = string.Join(",", sort.Select(x => x.name));
                            Title.titleid = Guid.NewGuid().ToString();
                            Title.sort = sortStr;
                            Title.usetime = usetime;
                            var userList = pbll.GetListByDept(item.DepartmentId).ToList();
                            List<PerformanceEntity> add = new List<PerformanceEntity>();
                            foreach (var user in userList)
                            {
                                var one = new PerformanceEntity();
                                one.performanceid = Guid.NewGuid().ToString();
                                one.score = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
                                one.userid = user.ID;
                                one.usetime = usetime;
                                one.deparmentid = item.DepartmentId;
                                one.titleid = Title.titleid;
                                one.sort = sortStr;
                                one.deparmentname = item.FullName;
                                one.username = user.Name;
                                one.photo = user.Photo;
                                one.quarters = user.Quarters;
                                one.planer = user.Planer;
                                add.Add(one);
                            }
                            service.operation(new List<PerformancesetupEntity>(), new List<PerformancesetupEntity>(), new List<PerformancesetupEntity>(), Title, add);
                            var up = new List<PerformanceupEntity>();
                            var oneup = new PerformanceupEntity();
                            oneup.id = Guid.NewGuid().ToString();
                            oneup.deptcode = item.EnCode;
                            oneup.titleid = Title.titleid;
                            oneup.departmentid = item.DepartmentId;
                            oneup.departmentname = item.FullName;
                            oneup.parentid = item.ParentId;
                            oneup.parentname = dept.FirstOrDefault(x => x.DepartmentId == item.ParentId).FullName;
                            oneup.parentcode = dept.FirstOrDefault(x => x.DepartmentId == item.ParentId).EnCode;
                            oneup.usetime = usetime;
                            oneup.useyear = time.Year.ToString();
                            oneup.isup = false;
                            up.Add(oneup);
                            upbll.add(up);
                        }
                        else
                        {
                            var setup = new List<PerformancesetupEntity>();

                            for (int i = 0; i < 9; i++)
                            {
                                var OneSetUp = new PerformancesetupEntity();
                                OneSetUp.createtime = time;
                                OneSetUp.performancetypeid = Guid.NewGuid().ToString();
                                OneSetUp.isupdate = false;
                                OneSetUp.departmentid = item.DepartmentId;
                                #region  固定类型
                                if (i == 2)
                                {
                                    continue;
                                }
                                switch (i)
                                {
                                    case 0:
                                        OneSetUp.isuse = true;
                                        OneSetUp.name = "绩效金额（元）";
                                        OneSetUp.type = 1;
                                        OneSetUp.sort = i;
                                        break;
                                    case 1:
                                        OneSetUp.isuse = true;
                                        OneSetUp.name = "绩效得分";
                                        OneSetUp.type = 1;
                                        OneSetUp.sort = i;
                                        break;
                                    case 3:
                                        OneSetUp.isuse = true;
                                        OneSetUp.name = "实际出勤（班次）";
                                        OneSetUp.type = 2;
                                        OneSetUp.sort = i;
                                        break;
                                    case 4:
                                        OneSetUp.isuse = true;
                                        OneSetUp.name = "值班（次）";
                                        OneSetUp.type = 2;
                                        OneSetUp.sort = i;
                                        break;
                                    case 5:
                                        OneSetUp.isuse = true;
                                        OneSetUp.name = "完成任务数";
                                        OneSetUp.type = 2;
                                        OneSetUp.sort = i;
                                        break;
                                    case 6:
                                        OneSetUp.isuse = true;
                                        OneSetUp.name = "任务总分";
                                        OneSetUp.type = 2;
                                        OneSetUp.sort = i;
                                        break;
                                    case 7:
                                        OneSetUp.isuse = true;
                                        OneSetUp.name = "奖励（元）";
                                        OneSetUp.type = 2;
                                        OneSetUp.sort = i;
                                        break;
                                    case 8:
                                        OneSetUp.isuse = true;
                                        OneSetUp.name = "考核（元）";
                                        OneSetUp.type = 2;
                                        OneSetUp.sort = i;
                                        break;

                                }

                                #endregion
                                setup.Add(OneSetUp);
                            }

                            PerformancetitleEntity Title = new PerformancetitleEntity();
                            Title.departmentid = item.DepartmentId;
                            Title.name = "绩效金额（元）,绩效得分,实际出勤（班次）,值班（次）,完成任务数,任务总分,奖励（元）,考核（元）";
                            Title.titleid = Guid.NewGuid().ToString();
                            Title.usetime = usetime;
                            Title.sort = "0,1,3,4,5,6,7,8";
                            var userList = pbll.GetListByDept(item.DepartmentId).ToList();
                            List<PerformanceEntity> add = new List<PerformanceEntity>();
                            foreach (var user in userList)
                            {
                                var one = new PerformanceEntity();
                                one.performanceid = Guid.NewGuid().ToString();
                                one.score = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
                                one.userid = user.ID;
                                one.usetime = usetime;
                                one.titleid = Title.titleid;
                                one.deparmentid = item.DepartmentId;
                                one.sort = "0,1,3,4,5,6,7,8";
                                one.deparmentname = item.FullName;
                                one.username = user.Name;
                                one.photo = user.Photo;
                                one.quarters = user.Quarters;
                                one.planer = user.Planer;
                                add.Add(one);
                            }

                            service.operation(setup, new List<PerformancesetupEntity>(), new List<PerformancesetupEntity>(), Title, add);
                            var up = new List<PerformanceupEntity>();
                            var oneup = new PerformanceupEntity();
                            oneup.id = Guid.NewGuid().ToString();
                            oneup.deptcode = item.EnCode;
                            oneup.titleid = Title.titleid;
                            oneup.departmentid = item.DepartmentId;
                            oneup.departmentname = item.FullName;
                            oneup.parentid = item.ParentId;
                            oneup.parentname = dept.FirstOrDefault(x => x.DepartmentId == item.ParentId).FullName;
                            oneup.parentcode = dept.FirstOrDefault(x => x.DepartmentId == item.ParentId).EnCode;
                            oneup.usetime = usetime;
                            oneup.useyear = time.Year.ToString();
                            oneup.isup = false;
                            up.Add(oneup);
                            upbll.add(up);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        /// <summary>
        /// 获取个人绩效
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public object PerformanceByuser(string userid, string time, string departmentid)
        {
            //所有数据
            var message = string.Empty;
            try
            {
                List<object> list = new List<object>();
                var month = Convert.ToInt32(time);
                for (int i = 12; i >= 1; i--)
                {
                    string getTime = new DateTime(month, i, 1).ToString("yyyy-MM-dd");
                    var title = titlebll.getTitle(getTime, departmentid);
                    if (title != null)
                    {
                        //获取数据
                        PerformanceEntity Score = Scorebll.getScoreByuser(getTime, userid);
                        var one = new
                        {
                            title = title.name,
                            score = Score.score,
                            sort = Score.sort,
                            month = i

                        };
                        list.Add(one);
                    }
                }
                return new { code = 0, info = message, data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
        }

        private WorkmeetingIService meetingService = new WorkmeetingService();
        /// <summary>
        /// 获取实时数据
        /// </summary>
        public void PerformanceGetData(string deptid, DateTime? usetime)
        {
            try
            {
                var nowTime = DateTime.Now;
                int year = usetime.HasValue ? usetime.Value.Year : nowTime.Year;
                int month = usetime.HasValue ? usetime.Value.Month : nowTime.Month;
                var userList = pbll.GetListByDept(deptid).ToList();
                var time = usetime.HasValue ? usetime.Value.ToString("yyyy-MM-dd") : nowTime.ToString("yyyy-MM-dd");
                List<PerformanceEntity> add = Scorebll.getScore(time, deptid);
                ///实际出勤（班次）,值班（次）,完成任务数,任务总分  3 4 5 6
                #region 获取个人出勤天数
                var workday = meetingService.GetUserMeetingTimes(deptid, year, month);
                #endregion
                #region 值班
                var workTimes = meetingService.GetUserDutyTimes(deptid, year, month);
                #endregion
                #region 任务数
                var ScoreTimes = meetingService.GetUserJobs(deptid, year, month);
                #endregion
                #region 个人总分
                var Score = meetingService.GetUserMonthScore(deptid, year, month);
                #endregion
                foreach (var user in userList)
                {
                    var one = add.FirstOrDefault(x => x.userid == user.ID);
                    if (one != null)
                    {
                        var ScoreList = one.score.Split(',').ToList();
                        var workdayData = workday.FirstOrDefault(x => x.UserId == user.ID);
                        if (workdayData != null)
                        {
                            ScoreList[3] = workdayData.Times.ToString();
                        }
                        var workTimesData = workTimes.FirstOrDefault(x => x.UserId == user.ID);
                        if (workTimesData != null)
                        {
                            ScoreList[4] = workTimesData.Times.ToString();
                        }
                        var ScoreTimesData = ScoreTimes.FirstOrDefault(x => x.UserId == user.ID);
                        if (ScoreTimesData != null)
                        {
                            ScoreList[5] = ScoreTimesData.Times.ToString();
                        }
                        var ScoreData = Score.FirstOrDefault(x => x.UserId == user.ID);
                        if (ScoreData != null)
                        {
                            ScoreList[6] = ScoreData.Times.ToString();
                        }
                        one.score = string.Join(",", ScoreList);
                    }

                }
                service.operation(new List<PerformancesetupEntity>(), new List<PerformancesetupEntity>(), new List<PerformancesetupEntity>(), new PerformancetitleEntity(), add);

            }
            catch (Exception)
            {

                throw;
            }

        }

    }
}
