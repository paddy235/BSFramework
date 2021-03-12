using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PeopleManage;
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
    public class PerformancesetupSecondBLL
    {
        /// <summary>
        /// 绩效管理配置
        /// </summary>
        private IPerformancesetupSecondService service = new PerformancesetupSecondService();
        private PerformancetitleSecondBLL titlebll = new PerformancetitleSecondBLL();
        private PerformanceSecondBLL Scorebll = new PerformanceSecondBLL();
        private UserBLL userbll = new UserBLL();
        private PeopleBLL pbll = new PeopleBLL();
        private PerformanceupSecondBLL upbll = new PerformanceupSecondBLL();
        private DepartmentBLL deptbll = new DepartmentBLL();
        private WorkOrderBLL workorder = new WorkOrderBLL();
        private JobEvaluateBLL sbll = new JobEvaluateBLL();
        /// <summary>
        /// 绩效管理配置 数据操作
        /// </summary>
        /// <param name="add">新增</param>
        /// <param name="del">删除</param>
        /// <param name="Listupdate">修改</param>
        /// <param name="title">当月标题头</param>
        /// <param name="Score">当月数据修正</param>
        public void operation(string timeStr, string departmentid, List<PerformancesetupSecondEntity> add, List<PerformancesetupSecondEntity> del, List<PerformancesetupSecondEntity> Listupdate, PerformancePersonSecondEntity person)
        {
            try
            {
                var time = Convert.ToDateTime(timeStr);
                //所有配置
                var setUp = service.AllTitle(departmentid);
                //获取标题
                PerformancetitleSecondEntity title = titlebll.getTitle(time, departmentid);
                //获取数据
                // List<PerformanceSecondEntity> Score = Scorebll.getScore(time, departmentid);
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
                    if (item.accounted != old.accounted)
                    {
                        old.accounted = item.accounted;
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
                    ////数据操作
                    //foreach (var scoreItem in Score)
                    //{
                    //    var ScoreInfo = scoreItem.score.Split(',').ToList();
                    //    //找出所在位置
                    //    //var num = setUp.IndexOf(delsort);
                    //    //if (num != -1)
                    //    //{
                    //    ScoreInfo[delsort.sort] = "0";
                    //    // ScoreInfo.RemoveAt(delsort.sort);
                    //    scoreItem.score = string.Join(",", ScoreInfo);
                    //    //}
                    //}
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
                //foreach (var item in Score)
                //{
                //    item.sort = string.Join(",", sort.Select(x => x.sort));
                //}
                title.sort = string.Join(",", sort.Select(x => x.sort));
                title.name = string.Join(",", sort.Select(x => x.name));
                service.operation(add, del, Listupdate, title, new List<PerformanceSecondEntity>(), person);
                //更正数据
                //PerformanceGetData(departmentid, time);
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
        public List<PerformancesetupSecondEntity> AllTitle(string departmentid)
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
                var usetime = new DateTime(time.Year, time.Month, 1);
                var dept = deptbll.GetList();



                foreach (var item in entity)
                {
                    //所有配置
                    var setUp = service.AllTitle(item.DepartmentId);

                    //所有配置人员
                    var setUpUser = service.getDeptPerson(item.DepartmentId);
                    var makeuserid = setUpUser == null ? string.Empty : setUpUser.makeuserid;


                    var sortStr = string.Join(",", setUp.Where(x => x.isuse).Select(x => x.sort));

                    PerformancetitleSecondEntity title = titlebll.getTitle(usetime, item.DepartmentId);
                    //是否已经生成
                    if (title != null)
                    {
                        // #region 何明 2019-09-20
                        //// var deptIds = entity.Select(p => p.DepartmentId).ToList();
                        List<PerformanceSecondEntity> allData = new PerformanceSecondBLL().getScore(usetime, item.DepartmentId);
                        var alluserList = new UserBLL().GetUserList().Where(x => x.DepartmentId == item.DepartmentId && x.DeleteMark != 1 && x.EnabledMark == 1).ToList();//所有人员
                        var delData = new List<PerformanceSecondEntity>();
                        var addData = new List<PerformanceSecondEntity>();
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


                        //构造新增实体
                        foreach (var user in addUsers)
                        {
                            var one = new PerformanceSecondEntity();
                            one.performanceid = Guid.NewGuid().ToString();
                            one.score = "0,0,0";
                            one.userid = user.UserId;
                            one.usetime = usetime;
                            one.useyear = usetime.Year.ToString();
                            one.usemonth = usetime.Month.ToString();
                            one.deparmentid = item.DepartmentId;
                            one.titleid = title.titleid;
                            one.sort = "0,1,2";
                            one.deparmentname = item.FullName;
                            one.username = user.RealName;
                            one.photo = user.Photo;
                            one.quarters = user.Quarters;
                            one.planer = user.Planer;
                            addData.Add(one);
                        }
                        var ck = false;
                        if (addData != null && addData.Count > 0)
                        {
                            Scorebll.Insert(addData);//新增的数据
                            //if (addData.Any(x => x.userid == "d15d843d-85e1-40c7-bcae-02f705855a39"))
                            //{
                            //    var b = "";
                            //}
                            ck = true;
                        }
                        if (delData != null && delData.Count > 0)
                        {
                            Scorebll.Delete(delData);//删除的数据
                            ck = true;
                        }
                        if (ck)
                        {
                            //更新数据
                            PerformanceGetData(item.DepartmentId, usetime);
                        }
                    }
                    else
                    {

                        //当月绩效
                        if (setUp.Count > 0)
                        {
                            var sort = setUp.Where(x => x.isuse).OrderBy(x => x.sort).ToList();
                            PerformancetitleSecondEntity Title = new PerformancetitleSecondEntity();
                            Title.departmentid = item.DepartmentId;
                            //关系model
                            List<TitleRelation> AlltitleRelation = new List<TitleRelation>();

                            //关系model
                            List<TitleRelation> titleRelation = new List<TitleRelation>();
                            //兼容第一版序号 
                            int i = 0;
                            foreach (var Nameitem in sort)
                            {

                                if (Nameitem.isspecial)
                                {
                                    //获取特殊列
                                    var specialList = service.getisspecial(Nameitem.performancetypeid).OrderBy(x => x.sort);
                                    foreach (var specialitem in specialList)
                                    {
                                        var one = new TitleRelation();
                                        one.accounted = Nameitem.accounted;
                                        one.sort = i;
                                        one.performancetypeid = Nameitem.performancetypeid;
                                        one.name = specialitem.name;
                                        one.methodwork = specialitem.methodwork;
                                        titleRelation.Add(one);
                                        i++;
                                    }
                                }
                                else
                                {
                                    var one = new TitleRelation();
                                    one.accounted = Nameitem.accounted;
                                    one.sort = i;
                                    one.performancetypeid = Nameitem.performancetypeid;
                                    one.name = Nameitem.name;
                                    titleRelation.Add(one);
                                    i++;
                                }

                            }
                            Title.name = string.Join(",", titleRelation.Select(x => x.name));
                            Title.usemonth = usetime.Month.ToString();
                            Title.useyear = usetime.Year.ToString();
                            Title.titleid = Guid.NewGuid().ToString();
                            Title.sort = string.Join(",", titleRelation.Select(x => x.sort));
                            Title.usetime = usetime;
                            var userList = pbll.GetListByDept(item.DepartmentId).ToList();


                            List<PerformanceSecondEntity> add = new List<PerformanceSecondEntity>();
                            //如果上个月有值则优先获取过来
                            var loadScore = Scorebll.getScore(usetime.AddMonths(-1), item.DepartmentId);
                            //获取实例
                            Type type = this.GetType();
                            object obj = Activator.CreateInstance(type);

                            foreach (var user in userList)
                            {
                                var one = new PerformanceSecondEntity();
                                one.performanceid = Guid.NewGuid().ToString();
                                List<string> scoreList = new List<string>();
                                //用户上个月的值
                                var loaduserscore = loadScore.FirstOrDefault(x => x.userid == user.ID);
                                var loadscoreNum = loaduserscore != null ? loaduserscore.score.Split(',') : null;
                                List<double> scoreNum = new List<double>();
                                double oneNum = 0;//固定列绩效金额
                                double twoNum = 0;//固定列月奖基数
                                double threeNum = 0;//固定列绩效系数
                                var newlist = new List<TitleRelation>();
                                foreach (var copy in titleRelation)
                                {
                                    var copyModel = new TitleRelation();
                                    copyModel.userId = copy.userId;
                                    copyModel.accounted = copy.accounted;
                                    copyModel.sort = copy.sort;
                                    copyModel.performancetypeid = copy.performancetypeid;
                                    copyModel.name = copy.name;
                                    copyModel.methodwork = copy.methodwork;
                                    copyModel.resultNum = copy.resultNum;
                                    newlist.Add(copyModel);
                                }
                                foreach (var name in newlist)
                                {

                                    name.userId = user.ID;
                                    name.DeptId = item.DepartmentId;
                                    if (loadscoreNum != null)
                                    {
                                        twoNum = Convert.ToDouble(loadscoreNum[1]);
                                    }
                                    if (loadscoreNum != null)
                                    {
                                        threeNum = Convert.ToDouble(loadscoreNum[2]);
                                    }
                                    switch (name.name)
                                    {
                                        //等待计算填充
                                        case "绩效金额":
                                            name.resultNum = twoNum * threeNum;
                                            break;
                                        //如果上个月有值则优先获取过来
                                        case "月奖基数":

                                            name.resultNum = twoNum;
                                            break;
                                        //如果上个月有值则优先获取过来
                                        case "绩效系数":

                                            name.resultNum = threeNum;
                                            break;
                                            //防止以后添加其他列
                                            //default:
                                            //    if (!string.IsNullOrEmpty(name.methodwork))
                                            //    {
                                            //        //需要统计的后续进行结算
                                            //        if (name.name == "出勤奖" || name.name == "任务工时奖" || name.name == "绩效奖")
                                            //        {
                                            //            continue;
                                            //        }
                                            //        var makeMethod = type.GetMethod(name.methodwork);

                                            //        //返回值必定为double
                                            //        var resultNum = makeMethod.Invoke(obj, new object[] { makeuserid, titleRelation, user.DeptId, user.ID, usetime });
                                            //        var scoreResult = Convert.ToDouble(resultNum);
                                            //        name.resultNum = Double.IsNaN(scoreResult) ? 0 : scoreResult;
                                            //    }
                                            //    else
                                            //    {
                                            //        name.resultNum = 0;
                                            //    }

                                            //    break;
                                    }
                                }

                                AlltitleRelation.AddRange(newlist);
                                one.userid = user.ID;
                                one.usetime = usetime;
                                one.deparmentid = item.DepartmentId;
                                one.titleid = Title.titleid;
                                one.sort = string.Join(",", titleRelation.Select(x => x.sort));
                                one.deparmentname = item.FullName;
                                one.useyear = usetime.Year.ToString();
                                one.usemonth = usetime.Month.ToString();
                                one.username = user.Name;
                                one.photo = user.Photo;
                                one.quarters = user.Quarters;
                                one.planer = user.Planer;
                                add.Add(one);
                            }
                            //定时服务每天进行 生成时不需要进行计算  由终端进行刷新计算
                            ////对每一列进行统一计算
                            //foreach (var name in titleRelation)
                            //{
                            //    if (!string.IsNullOrEmpty(name.methodwork))
                            //    {
                            //        //需要统计的后续进行结算
                            //        if (name.name == "出勤奖" || name.name == "任务工时奖" || name.name == "绩效奖")
                            //        {
                            //            continue;
                            //        }
                            //        var makeMethod = type.GetMethod(name.methodwork);
                            //        //返回值必定为关系model AlltitleRelation
                            //        var AlltitleRelations = makeMethod.Invoke(obj, new object[] { makeuserid, AlltitleRelation, item.DepartmentId, userList, usetime });
                            //        AlltitleRelation = AlltitleRelations as List<TitleRelation>;
                            //    }
                            //}

                            ////对需要总结的数据进行计算
                            //foreach (var name in titleRelation)
                            //{
                            //    //需要统计的后续进行结算
                            //    if (name.name == "出勤奖" || name.name == "任务工时奖" || name.name == "绩效奖")
                            //    {
                            //        var makeMethod = type.GetMethod(name.methodwork);
                            //        //返回值必定为关系model AlltitleRelation
                            //        var AlltitleRelations = makeMethod.Invoke(obj, new object[] { makeuserid, AlltitleRelation, item.DepartmentId, userList, usetime });
                            //        AlltitleRelation = AlltitleRelations as List<TitleRelation>;

                            //    }
                            //}
                            //对总绩效进行计算 对数据赋值
                            foreach (var resultList in AlltitleRelation.GroupBy(x => x.userId))
                            {
                                var one = add.FirstOrDefault(x => x.userid == resultList.Key);
                                var haveCount = resultList.Where(x => x.name == "出勤奖" || x.name == "任务工时奖" || x.name == "绩效奖");
                                double total = 0;
                                if (haveCount.Count() > 0)
                                {
                                    total = resultList.Where(x => x.name == "出勤奖" || x.name == "任务工时奖" || x.name == "绩效奖").Sum(x => x.resultNum);
                                    resultList.FirstOrDefault(x => x.name == "绩效金额").resultNum = total;
                                }
                                one.score = string.Join(",", resultList.OrderBy(x => x.sort).Select(x => x.resultNum));
                            }
                            service.operation(new List<PerformancesetupSecondEntity>(), new List<PerformancesetupSecondEntity>(), new List<PerformancesetupSecondEntity>(), Title, add, null);
                            var up = new List<PerformanceupSecondEntity>();
                            var oneup = new PerformanceupSecondEntity();
                            oneup.id = Guid.NewGuid().ToString();
                            oneup.deptcode = item.EnCode;
                            oneup.titleid = Title.titleid;
                            oneup.departmentid = item.DepartmentId;
                            oneup.departmentname = item.FullName;
                            oneup.parentid = item.ParentId;
                            oneup.parentname = dept.FirstOrDefault(x => x.DepartmentId == item.ParentId).FullName;
                            oneup.parentcode = dept.FirstOrDefault(x => x.DepartmentId == item.ParentId).EnCode;
                            oneup.usetime = usetime;
                            oneup.useyear = usetime.Year.ToString();
                            oneup.usemonth = usetime.Month.ToString();
                            oneup.isup = false;
                            up.Add(oneup);
                            upbll.add(up);
                        }
                        else
                        {
                            #region 第一次执行初始化数据
                            // 第一次执行初始化数据
                            var setup = new List<PerformancesetupSecondEntity>();
                            var setupMethod = new List<PerformanceMethodSecondEntity>();
                            for (int i = 0; i < 6; i++)
                            {
                                var OneSetUp = new PerformancesetupSecondEntity();
                                OneSetUp.createtime = time;
                                OneSetUp.performancetypeid = Guid.NewGuid().ToString();
                                OneSetUp.isupdate = false;
                                OneSetUp.departmentid = item.DepartmentId;
                                #region  固定类型 加载类型

                                switch (i)
                                {
                                    case 0:
                                        OneSetUp.isuse = true;
                                        OneSetUp.name = "绩效金额";
                                        OneSetUp.type = 1;
                                        OneSetUp.sort = i;
                                        OneSetUp.isspecial = false;
                                        break;
                                    case 1:
                                        OneSetUp.isuse = true;
                                        OneSetUp.name = "月奖基数";
                                        OneSetUp.type = 1;
                                        OneSetUp.sort = i;
                                        OneSetUp.isspecial = false;
                                        break;
                                    case 2:
                                        OneSetUp.isuse = true;
                                        OneSetUp.name = "绩效系数";
                                        OneSetUp.type = 1;
                                        OneSetUp.sort = i;
                                        OneSetUp.isspecial = false;
                                        break;
                                    case 3:
                                        OneSetUp.isuse = false;
                                        OneSetUp.name = "出勤奖";
                                        OneSetUp.type = 2;
                                        OneSetUp.sort = i;
                                        OneSetUp.isspecial = true;
                                        for (int j = 0; j < 3; j++)
                                        {
                                            var oneSetUpMethod = new PerformanceMethodSecondEntity();
                                            oneSetUpMethod.performancetypeid = OneSetUp.performancetypeid;
                                            oneSetUpMethod.sort = j;
                                            switch (j)
                                            {
                                                case 0:
                                                    oneSetUpMethod.name = "应出工时";
                                                    oneSetUpMethod.methodwork = "WorkTime";
                                                    break;
                                                case 1:
                                                    oneSetUpMethod.name = "实际工时";
                                                    oneSetUpMethod.methodwork = "ActualWorkTime";
                                                    break;
                                                case 2:
                                                    oneSetUpMethod.name = "出勤奖";
                                                    oneSetUpMethod.methodwork = "WorkTimeMoney";
                                                    break;
                                            }
                                            setupMethod.Add(oneSetUpMethod);
                                        }
                                        break;
                                    case 4:
                                        OneSetUp.isuse = false;
                                        OneSetUp.name = "工时奖";
                                        OneSetUp.type = 2;
                                        OneSetUp.sort = i;
                                        OneSetUp.isspecial = true;
                                        for (int j = 0; j < 2; j++)
                                        {
                                            var oneSetUpMethod = new PerformanceMethodSecondEntity();
                                            oneSetUpMethod.performancetypeid = OneSetUp.performancetypeid;
                                            oneSetUpMethod.sort = j;
                                            switch (j)
                                            {
                                                case 0:
                                                    oneSetUpMethod.name = "任务工时";
                                                    oneSetUpMethod.methodwork = "TaskTime";
                                                    break;
                                                case 1:
                                                    oneSetUpMethod.name = "任务工时奖";
                                                    oneSetUpMethod.methodwork = "TaskTimeMoney";
                                                    break;

                                            }
                                            setupMethod.Add(oneSetUpMethod);
                                        }
                                        break;
                                    case 5:
                                        OneSetUp.isuse = false;
                                        OneSetUp.name = "绩效奖";
                                        OneSetUp.type = 2;
                                        OneSetUp.sort = i;
                                        OneSetUp.isspecial = true;
                                        for (int j = 0; j < 2; j++)
                                        {
                                            var oneSetUpMethod = new PerformanceMethodSecondEntity();
                                            oneSetUpMethod.performancetypeid = OneSetUp.performancetypeid;
                                            oneSetUpMethod.sort = j;
                                            switch (j)
                                            {
                                                case 0:
                                                    oneSetUpMethod.name = "任务评分";
                                                    oneSetUpMethod.methodwork = "TaskScore";
                                                    break;
                                                case 1:
                                                    oneSetUpMethod.name = "绩效奖";
                                                    oneSetUpMethod.methodwork = "TaskScoreMoney";
                                                    break;

                                            }
                                            setupMethod.Add(oneSetUpMethod);
                                        }
                                        break;
                                }

                                #endregion
                                setup.Add(OneSetUp);
                            }
                            PerformancetitleSecondEntity Title = new PerformancetitleSecondEntity();
                            Title.departmentid = item.DepartmentId;
                            Title.name = "绩效金额,月奖基数,绩效系数";
                            Title.titleid = Guid.NewGuid().ToString();
                            Title.useyear = usetime.Year.ToString();
                            Title.usemonth = usetime.Month.ToString();
                            Title.usetime = usetime;
                            Title.sort = "0,1,2";
                            var userList = pbll.GetListByDept(item.DepartmentId).ToList();
                            List<PerformanceSecondEntity> add = new List<PerformanceSecondEntity>();
                            foreach (var user in userList)
                            {
                                var one = new PerformanceSecondEntity();
                                one.performanceid = Guid.NewGuid().ToString();
                                one.score = "0,0,0";
                                one.userid = user.ID;
                                one.usetime = usetime;
                                one.titleid = Title.titleid;
                                one.deparmentid = item.DepartmentId;
                                one.sort = "0,1,2";
                                one.deparmentname = item.FullName;
                                one.username = user.Name;
                                one.useyear = usetime.Year.ToString();
                                one.usemonth = usetime.Month.ToString();
                                one.photo = user.Photo;
                                one.quarters = user.Quarters;
                                one.planer = user.Planer;
                                add.Add(one);
                            }
                            service.operation(setup, new List<PerformancesetupSecondEntity>(), new List<PerformancesetupSecondEntity>(), Title, add, null);
                            service.Setisspecial(setupMethod);
                            var up = new List<PerformanceupSecondEntity>();
                            var oneup = new PerformanceupSecondEntity();
                            oneup.id = Guid.NewGuid().ToString();
                            oneup.deptcode = item.EnCode;
                            oneup.titleid = Title.titleid;
                            oneup.departmentid = item.DepartmentId;
                            oneup.departmentname = item.FullName;
                            oneup.parentid = item.ParentId;
                            oneup.parentname = dept.FirstOrDefault(x => x.DepartmentId == item.ParentId).FullName;
                            oneup.parentcode = dept.FirstOrDefault(x => x.DepartmentId == item.ParentId).EnCode;
                            oneup.usetime = usetime;
                            oneup.useyear = usetime.Year.ToString();
                            oneup.usemonth = usetime.Month.ToString();
                            oneup.isup = false;
                            up.Add(oneup);
                            upbll.add(up);
                            #endregion

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

        #region 特殊列计算方法
        //标题关系model
        public class TitleRelation
        {
            public string userId { get; set; }//用户id
            public string DeptId { get; set; }//用户id
            public string name { get; set; }//列名
            public int sort { get; set; }//排序
            public string performancetypeid { get; set; } //主表id
            public double accounted { get; set; }//占比
            public string methodwork { get; set; }//调用方法
            public double resultNum { get; set; }//获取的值

        }
        /// <summary>
        /// 应出工时  排班管理中的总时长累加
        /// </summary>
        /// <returns></returns>
        public List<TitleRelation> WorkTime(string makeuserid, List<TitleRelation> titleRelation, string DeptId, List<PeopleEntity> user, DateTime usetime)
        {
            var dept = deptbll.GetEntity(DeptId);
            var deptGroup = workorder.WorkOrderGet(usetime, usetime, DeptId);
            var data = deptGroup.FirstOrDefault();
            double totalHour = 0;
            if (data != null)
            {

                var timeSort = data.timedata.Split(',');

                foreach (var item in timeSort)
                {
                    if (item == "无" || item == "休息")
                    {
                        continue;
                    }
                    var timeSplit = item.Substring(0, 11).Split('-');
                    var upTime = timeSplit[0];
                    var nextTime = timeSplit[1];
                    var upData = Convert.ToDateTime("2020-01-01 " + upTime);
                    var nextData = Convert.ToDateTime("2020-01-01 " + nextTime);
                    if (upData >= nextData)
                    {
                        nextData = nextData.AddDays(1);
                    }
                    var timeSpan = nextData - upData;
                    var hour = timeSpan.TotalHours;
                    totalHour = totalHour + hour;
                    //班组类型  01运行 02检修  03其他
                    if (dept.TeamType == "02")
                    {
                        totalHour = totalHour - 2;
                    }
                }
            }

            foreach (var item in user)
            {

                var my = titleRelation.FirstOrDefault(x => x.userId == item.ID && x.name == "应出工时");
                if (my == null)
                {
                    my.resultNum = totalHour;
                    continue;
                }


                my.resultNum = double.IsNaN(totalHour) ? 0 : Math.Round(totalHour, 2);
            }
            return titleRelation;
        }

        /// <summary>
        /// 实际工时  班前班后会签到
        /// </summary>
        /// <returns></returns>
        public List<TitleRelation> ActualWorkTime(string makeuserid, List<TitleRelation> titleRelation, string DeptId, List<PeopleEntity> user, DateTime usetime)
        {
            var deptGroup = workorder.WorkOrderGet(usetime, usetime, DeptId);
            var data = deptGroup.FirstOrDefault();
            var timeSort = new List<string>();
            if (data != null)
            {
                timeSort = data.timedata.Split(',').ToList();
            }

            var userData = meetingService.GetDayAttendance2(user, usetime, usetime.AddMonths(1).AddDays(-1), DeptId);
            foreach (var item in user)
            {
                double resultNum = 0;
                var my = titleRelation.FirstOrDefault(x => x.userId == item.ID && x.name == "实际工时");
                if (my == null)
                {
                    my.resultNum = resultNum;
                    continue;
                }
                var meet = userData.Where(x => x.userid == item.ID);
                var work = meet.Where(x => x.State == "出勤");
                double totalHour = 0;
                foreach (var times in work)
                {
                    if (timeSort.Count() > 0)
                    {
                        var time = timeSort[times.Date.Day - 1];
                        if (time == "无" || time == "休息")
                        {
                            continue;
                        }
                        var timeSplit = time.Substring(0, 11).Split('-');
                        var upTime = timeSplit[0];
                        var nextTime = timeSplit[1];
                        var upData = Convert.ToDateTime("2020-01-01 " + upTime);
                        var nextData = Convert.ToDateTime("2020-01-01 " + nextTime);
                        if (upData >= nextData)
                        {
                            nextData = nextData.AddDays(1);
                        }
                        var timeSpan = nextData - upData;
                        var hour = timeSpan.TotalHours;
                        totalHour = Math.Round(totalHour + hour, 2);
                    }
                    else
                    {
                        totalHour = totalHour + 8;
                    }

                }
                resultNum = totalHour;

                my.resultNum = double.IsNaN(resultNum) ? 0 : Math.Round(resultNum, 2);
            }
            return titleRelation;
        }

        /// <summary>
        /// 出勤奖 月奖基数（个人）*考核比例-月奖基数（所有人累加）*(考核比例)*个人缺勤工时/所有人应出勤总工时。
        /// </summary>
        /// <returns></returns>
        public List<TitleRelation> WorkTimeMoney(string makeuserid, List<TitleRelation> titleRelation, string DeptId, List<PeopleEntity> user, DateTime usetime)
        {

            //var allData = meetingService.GetMonthAttendance2(user, usetime, usetime.AddMonths(1).AddDays(-1));
            foreach (var item in user)
            {
                double resultNum = 0;
                var my = titleRelation.FirstOrDefault(x => x.userId == item.ID && x.name == "出勤奖");
                if (my == null)
                {
                    my.resultNum = resultNum;
                    continue;
                }
                //var meetData = allData.Where(x=>x.userId==item.ID);
                var allWorkTime = titleRelation.Where(x => x.name == "应出工时").Sum(x => x.resultNum);//所有人总出勤工时
                var monthmoney = titleRelation.Where(x => x.name == "月奖基数").Sum(x => x.resultNum);//月奖基数（所有人累加）
                                                                                                  //var leaveTime = meetData.Where(x => x.Category != "值班" && x.Category != "出勤" && x.Category != "").Sum(x => x.Hours);
                var oneWorkTime = titleRelation.Where(x => x.name == "实际工时" && x.userId == item.ID).Sum(x => x.resultNum);//个人出勤工时
                var oneNeedTime = titleRelation.Where(x => x.name == "应出工时" && x.userId == item.ID).Sum(x => x.resultNum);//个人应出工时
                var leaveTime = oneNeedTime - oneWorkTime;//缺勤工时
                var mymoney = titleRelation.FirstOrDefault(x => x.name == "月奖基数" && x.userId == item.ID).resultNum; //月奖基数（个人）
                var bili = titleRelation.FirstOrDefault(x => x.name == "实际工时" && x.userId == item.ID).accounted; //考核比例??  应该为绩效系数
                resultNum = mymoney * (bili / 100) - monthmoney * (bili / 100) * leaveTime / allWorkTime;

                my.resultNum = double.IsNaN(resultNum) ? 0 : Math.Round(resultNum, 2);
            }
            return titleRelation;
        }

        /// <summary>
        /// 任务工时 工时管理中获取，为个人当月总工时
        /// </summary>>
        /// <returns></returns>
        public List<TitleRelation> TaskTime(string makeuserid, List<TitleRelation> titleRelation, string DeptId, List<PeopleEntity> user, DateTime usetime)
        {
            #region 个人工时
            var TaskHour = meetingService.GetUserMonthTaskHour(DeptId, usetime.Year, usetime.Month);

            #endregion
            foreach (var item in user)
            {
                double resultNum = 0;
                var my = titleRelation.FirstOrDefault(x => x.userId == item.ID && x.name == "任务工时");
                if (my == null)
                {
                    my.resultNum = resultNum;
                    continue;
                }
                var userTaskHour = TaskHour.FirstOrDefault(x => x.UserId == item.ID);
                if (userTaskHour != null)
                {
                    resultNum = userTaskHour.Times;
                }

                my.resultNum = double.IsNaN(resultNum) ? 0 : Math.Round(resultNum, 2);
            }
            return titleRelation;
        }


        /// <summary>
        /// 任务工时奖 月奖基础总额（参与分配人员）*考核比例*月奖系数*个人工时/班组总工时（选择人个人工时*月奖系数） 不选择 月奖基础总额*考核比例
        /// </summary>
        /// <returns></returns>
        public List<TitleRelation> TaskTimeMoney(string makeuserid, List<TitleRelation> titleRelation, string DeptId, List<PeopleEntity> user, DateTime usetime)
        {
            //所有人总出勤工时
            var allWorkTime = titleRelation.Where(x => x.name == "任务工时" && makeuserid.Contains(x.userId));
            //所有人jx
            var alljx = titleRelation.Where(x => x.name == "绩效系数" && makeuserid.Contains(x.userId));
            foreach (var item in user)
            {
                double resultNum = 0;
                var my = titleRelation.FirstOrDefault(x => x.userId == item.ID && x.name == "任务工时奖");
                if (my == null)
                {
                    my.resultNum = resultNum;
                    continue;
                }
                var mymoney = titleRelation.FirstOrDefault(x => x.name == "月奖基数" && x.userId == item.ID).resultNum; //月奖基数（个人）
                var jx = titleRelation.FirstOrDefault(x => x.name == "绩效系数" && x.userId == item.ID).resultNum; //月奖系数
                if (makeuserid.Contains(item.ID))
                {
                    var bili = titleRelation.FirstOrDefault(x => x.name == "任务工时" && x.userId == item.ID).accounted;//考核比例
                    double allWorkTimeData = 0;//班组总工时（选择人个人工时*月奖系数）
                    foreach (var timeData in allWorkTime)
                    {
                        var oneJx = alljx.FirstOrDefault(x => x.userId == timeData.userId);
                        allWorkTimeData = allWorkTimeData + (oneJx == null ? 0 : oneJx.resultNum * timeData.resultNum);
                    }
                    var myWorkTime = titleRelation.Where(x => x.name == "任务工时" && x.userId == item.ID).Sum(x => x.resultNum);//个人工时
                    var allmoney = titleRelation.Where(x => x.name == "月奖基数" && makeuserid.Contains(x.userId)).Sum(x => x.resultNum); //月奖基数（所有）
                    resultNum = allmoney * (bili / 100) * jx * myWorkTime / allWorkTimeData;
                }
                else
                {
                    resultNum = mymoney * jx;
                }

                my.resultNum = double.IsNaN(resultNum) ? 0 : Math.Round(resultNum, 2);
            }

            return titleRelation;
        }

        /// <summary>
        /// 任务评分 为个人当月所有任务（包含跨天任务）的平均得分
        /// </summary>
        /// <returns></returns>
        public List<TitleRelation> TaskScore(string makeuserid, List<TitleRelation> titleRelation, string DeptId, List<PeopleEntity> user, DateTime usetime)
        {

            #region 任务
            var ScoreTimes = sbll.GetPersonScore(DeptId, usetime.Year, usetime.Month);
            #endregion

            foreach (var item in user)
            {
                double resultNum = 0;
                var my = titleRelation.FirstOrDefault(x => x.userId == item.ID && x.name == "任务评分");
                if (my == null)
                {
                    my.resultNum = resultNum;
                    continue;
                }
                //获取个人
                var userScoreTimes = ScoreTimes.FirstOrDefault(x => x.userid == item.ID);
                if (userScoreTimes != null)
                {
                    resultNum = Convert.ToDouble(userScoreTimes.Score / userScoreTimes.total);
                }

                my.resultNum = double.IsNaN(resultNum) ? 0 : Math.Round(resultNum, 2);
            }

            return titleRelation;
        }

        /// <summary>
        /// 绩效奖 月奖基数*考核比例*个人任务平均分/选择人任务平均得分  不选择 月奖基数*考核比例
        /// </summary>
        /// <returns></returns>
        public List<TitleRelation> TaskScoreMoney(string makeuserid, List<TitleRelation> titleRelation, string DeptId, List<PeopleEntity> user, DateTime usetime)
        {
            #region 任务
            var ScoreTimes = sbll.GetPersonScore(DeptId, usetime.Year, usetime.Month);
            #endregion


            foreach (var item in user)
            {
                double resultNum = 0;
                var jx = titleRelation.FirstOrDefault(x => x.userId == item.ID && x.name == "绩效奖");
                if (jx == null)
                {
                    jx.resultNum = resultNum;
                    continue;
                }
                var js = titleRelation.FirstOrDefault(x => x.userId == item.ID && x.name == "月奖基数");
                if (js.resultNum == 0)
                {
                    js.resultNum = resultNum;
                    continue;
                }

                //没有选择当前人员
                if (makeuserid.Contains(item.ID))
                {

                    //获取个人
                    var userScoreTimes = ScoreTimes.FirstOrDefault(x => x.userid == item.ID);
                    double oneScore = 0;
                    double allScore = 0;
                    if (userScoreTimes != null)
                    {
                        oneScore = Convert.ToDouble(userScoreTimes.Score / userScoreTimes.total);
                    }
                    var selectScore = ScoreTimes.Where(x => makeuserid.Contains(x.userid));

                    if (ScoreTimes.Count() > 0)
                    {
                        allScore = Convert.ToDouble(selectScore.Sum(x => x.Score) / selectScore.Sum(x => x.total));

                    }

                    //月奖基数*考核比例*个人任务平均分/选择人任务平均得分
                    resultNum = js.resultNum * (jx.accounted / 100) * oneScore / allScore;
                }
                else
                {
                    //月奖基数* 考核比例
                    resultNum = js.resultNum * (jx.accounted / 100);
                }

                jx.resultNum = double.IsNaN(resultNum) ? 0 : Math.Round(resultNum, 2);
            }
            return titleRelation;

        }
        #endregion

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
                    var getTime = new DateTime(month, i, 1);
                    var title = titlebll.getTitle(getTime, departmentid);
                    if (title != null)
                    {
                        //获取数据
                        PerformanceSecondEntity Score = Scorebll.getScoreByuser(getTime, userid);
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
        /// 获取实时数据  更新数据
        /// </summary>
        public void PerformanceGetData(string deptid, DateTime? usetime)
        {
            try
            {

                //只对当前数据进行操作

                var deptEntity = deptbll.GetEntity(deptid);
                var nowTime = DateTime.Now;
                int year = usetime.HasValue ? usetime.Value.Year : nowTime.Year;
                int month = usetime.HasValue ? usetime.Value.Month : nowTime.Month;
                var userList = pbll.GetListByDept(deptid).ToList();
                var time = usetime.HasValue ? usetime.Value : new DateTime(year, month, 1);
                var ckTime = nowTime.AddMonths(-1);
                //小于本月
                if (year < ckTime.Year || (year == ckTime.Year && month < ckTime.Month))
                {
                    return;
                }
                List<PerformanceSecondEntity> add = Scorebll.getScore(time, deptid);
                List<PerformanceSecondEntity> del = new List<PerformanceSecondEntity>();
                //如果没有数据生成数据
                var isData = false;
                if (add.Count == 0)
                {
                    add = new List<PerformanceSecondEntity>();
                    isData = true;
                }
                //所有配置
                var setUp = service.AllTitle(deptid);
                //所有配置人员
                var setUpUser = service.getDeptPerson(deptid);
                var makeuserid = setUpUser == null ? string.Empty : setUpUser.makeuserid;
                var sort = setUp.Where(x => x.isuse).OrderBy(x => x.sort).ToList();

                //关系model
                List<TitleRelation> AlltitleRelation = new List<TitleRelation>();

                //关系model
                List<TitleRelation> titleRelation = new List<TitleRelation>();
                //兼容第一版序号 
                int i = 0;
                foreach (var Nameitem in sort)
                {

                    if (Nameitem.isspecial)
                    {
                        //获取特殊列
                        var specialList = service.getisspecial(Nameitem.performancetypeid).OrderBy(x => x.sort);
                        foreach (var specialitem in specialList)
                        {
                            var one = new TitleRelation();
                            one.accounted = Nameitem.accounted;
                            one.sort = i;
                            one.performancetypeid = Nameitem.performancetypeid;
                            one.name = specialitem.name;
                            one.methodwork = specialitem.methodwork;
                            titleRelation.Add(one);
                            i++;
                        }
                    }
                    else
                    {
                        var one = new TitleRelation();
                        one.accounted = Nameitem.accounted;
                        one.sort = i;
                        one.performancetypeid = Nameitem.performancetypeid;
                        one.name = Nameitem.name;
                        titleRelation.Add(one);
                        i++;
                    }

                }
                PerformancetitleSecondEntity Title = titlebll.getTitle(time, deptid);
                if (isData)
                {
                    Title = new PerformancetitleSecondEntity();
                    Title.departmentid = deptid;
                    Title.usemonth = time.Month.ToString();
                    Title.useyear = time.Year.ToString();
                    Title.titleid = Guid.NewGuid().ToString();
                    Title.usetime = time;
                }
                Title.name = string.Join(",", titleRelation.Select(x => x.name));
                Title.sort = string.Join(",", titleRelation.Select(x => x.sort));

                //获取实例 反射调用方法
                Type type = this.GetType();
                object obj = Activator.CreateInstance(type);
                var alluserList = new UserBLL().GetUserList().Where(x => x.DepartmentId == deptid && x.DeleteMark != 1 && x.EnabledMark == 1).ToList();//所有人员

                // #endregion
                //现存人员
                var thisUsers = alluserList.Where(p => p.DepartmentId == deptid).Select(x => x.UserId).ToList();

                userList = userList.Where(x => thisUsers.Contains(x.ID)).ToList();
                //生成所有的用户关系列
                foreach (var user in userList)
                {


                    double oneNum = 0;//固定列绩效金额
                    double twoNum = 0;//固定列月奖基数
                    double threeNum = 0;//固定列绩效系数

                    var one = add.FirstOrDefault(x => x.userid == user.ID);
                    if (one == null)
                    {
                        one = new PerformanceSecondEntity();
                        //one.performanceid = Guid.NewGuid().ToString();
                        one.userid = user.ID;
                        one.usetime = time;
                        one.deparmentid = deptid;
                        one.titleid = Title.titleid;
                        one.sort = string.Join(",", titleRelation.Select(x => x.sort));
                        one.deparmentname = deptEntity.FullName;
                        one.useyear = time.Year.ToString();
                        one.usemonth = time.Month.ToString();
                        one.username = user.Name;
                        one.photo = user.Photo;
                        one.quarters = user.Quarters;
                        one.planer = user.Planer;
                        add.Add(one);


                    }
                    else
                    {
                        var scoreSplit = one.score.Split(',');
                        twoNum = Convert.ToDouble(scoreSplit[1]);
                        threeNum = Convert.ToDouble(scoreSplit[2]);
                    }

                    List<string> scoreList = new List<string>();
                    List<double> scoreNum = new List<double>();



                    var newlist = new List<TitleRelation>();
                    foreach (var copy in titleRelation)
                    {
                        var copyModel = new TitleRelation();
                        copyModel.userId = copy.userId;
                        copyModel.accounted = copy.accounted;
                        copyModel.sort = copy.sort;
                        copyModel.performancetypeid = copy.performancetypeid;
                        copyModel.name = copy.name;
                        copyModel.methodwork = copy.methodwork;
                        copyModel.resultNum = copy.resultNum;
                        newlist.Add(copyModel);
                    }
                    foreach (var name in newlist)
                    {

                        name.userId = user.ID;
                        name.DeptId = deptid;
                        switch (name.name)
                        {
                            //等待计算填充
                            case "绩效金额":
                                name.resultNum = oneNum;
                                break;
                            //
                            case "月奖基数":
                                name.resultNum = twoNum;
                                break;
                            //
                            case "绩效系数":
                                name.resultNum = threeNum;
                                break;
                                ////防止以后添加其他列
                                //default:
                                //    if (!string.IsNullOrEmpty(name.methodwork))
                                //    {
                                //        //需要统计的后续进行结算
                                //        if (name.name == "出勤奖" || name.name == "任务工时奖" || name.name == "绩效奖")
                                //        {
                                //            continue;
                                //        }
                                //        var makeMethod = type.GetMethod(name.methodwork);

                                //        //返回值必定为double
                                //        var resultNum = makeMethod.Invoke(obj, new object[] { makeuserid, titleRelation, user.DeptId, user.ID, usetime });
                                //        var scoreResult = Convert.ToDouble(resultNum);
                                //        name.resultNum = scoreResult;
                                //    }
                                //    else
                                //    {
                                //        name.resultNum = 0;
                                //    }

                                //    break;
                        }
                    }

                    AlltitleRelation.AddRange(newlist);
                }

                //获取人员不存在的数据
                del = add.Where(x => !thisUsers.Contains(x.userid)).ToList();
                service.remove(del);
                //去除数据
                add.RemoveAll(x => !thisUsers.Contains(x.userid));


                //对每一列进行统一计算
                foreach (var name in titleRelation)
                {
                    if (!string.IsNullOrEmpty(name.methodwork))
                    {
                        //需要统计的后续进行结算
                        if (name.name == "出勤奖" || name.name == "任务工时奖" || name.name == "绩效奖")
                        {
                            continue;
                        }
                        var makeMethod = type.GetMethod(name.methodwork);
                        //返回值必定为关系model AlltitleRelation
                        var AlltitleRelations = makeMethod.Invoke(obj, new object[] { makeuserid, AlltitleRelation, deptid, userList, usetime });
                        AlltitleRelation = AlltitleRelations as List<TitleRelation>;
                    }
                }

                //对需要总结的数据进行计算
                foreach (var name in titleRelation)
                {
                    //需要统计的后续进行结算
                    if (name.name == "出勤奖" || name.name == "任务工时奖" || name.name == "绩效奖")
                    {
                        var makeMethod = type.GetMethod(name.methodwork);
                        //返回值必定为关系model AlltitleRelation
                        var AlltitleRelations = makeMethod.Invoke(obj, new object[] { makeuserid, AlltitleRelation, deptid, userList, usetime });
                        AlltitleRelation = AlltitleRelations as List<TitleRelation>;

                    }
                }
                //对总绩效进行计算 对数据赋值
                foreach (var resultList in AlltitleRelation.GroupBy(x => x.userId))
                {
                    var one = add.FirstOrDefault(x => x.userid == resultList.Key);
                    var haveCount = resultList.Where(x => x.name == "出勤奖" || x.name == "任务工时奖" || x.name == "绩效奖");
                    double total = 0;
                    if (haveCount.Count() > 0)
                    {
                        total = resultList.Where(x => x.name == "出勤奖" || x.name == "任务工时奖" || x.name == "绩效奖").Sum(x => x.resultNum);
                        resultList.FirstOrDefault(x => x.name == "绩效金额").resultNum = total;
                    }

                    resultList.FirstOrDefault(x => x.name == "绩效金额").resultNum = double.IsNaN(total) ? 0 : total;
                    one.score = string.Join(",", resultList.OrderBy(x => x.sort).Select(x => x.resultNum));
                    one.sort = string.Join(",", resultList.OrderBy(x => x.sort).Select(x => x.sort));
                }

                service.operation(new List<PerformancesetupSecondEntity>(), new List<PerformancesetupSecondEntity>(), new List<PerformancesetupSecondEntity>(), Title, add, null);
                if (isData)
                {


                    var up = new List<PerformanceupSecondEntity>();
                    var oneup = new PerformanceupSecondEntity();
                    oneup.id = Guid.NewGuid().ToString();
                    oneup.deptcode = deptEntity.EnCode;
                    oneup.titleid = Title.titleid;
                    oneup.departmentid = deptEntity.DepartmentId;
                    oneup.departmentname = deptEntity.FullName;
                    oneup.parentid = deptEntity.ParentId;
                    var parent = deptbll.GetEntity(deptEntity.ParentId);
                    if (parent != null)
                    {
                        oneup.parentname = parent.FullName;
                        oneup.parentcode = parent.EnCode;
                    }
                    oneup.usetime = usetime;
                    oneup.useyear = time.Year.ToString();
                    oneup.usemonth = time.Month.ToString();
                    oneup.isup = false;
                    up.Add(oneup);
                    upbll.add(up);
                }

            }
            catch (Exception)
            {

                throw;
            }

        }


        /// <summary>
        /// 获取所有人员配置
        /// </summary>
        /// <returns></returns>
        public PerformancePersonSecondEntity getDeptPerson(string departmentid)
        {
            return service.getDeptPerson(departmentid);
        }
        /// <summary>
        /// 修改新增人员信息
        /// </summary>
        /// <param name="entity"></param>
        public void SetDeptPerson(PerformancePersonSecondEntity entity, string userid)
        {
            var user = userbll.GetEntity(userid);
            service.SetDeptPerson(entity, user);
        }

    }
}
