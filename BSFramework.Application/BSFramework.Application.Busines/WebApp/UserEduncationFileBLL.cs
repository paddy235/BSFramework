using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.EducationManage;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.EducationManage;
using BSFramework.Application.Entity.WebApp;
using BSFramework.Application.IService.Activity;
using BSFramework.Application.IService.EducationManage;
using BSFramework.Application.Service.Activity;
using BSFramework.Application.Service.EducationManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.IService.WorkMeeting;
using BSFramework.Service.WorkMeeting;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.WebApp
{
    public class UserEduncationFileBLL
    {
        private IEduBaseInfoService ed = new EduBaseInfoService();

        //危险预知训练
        private WorkmeetingIService work = new WorkmeetingService();
        //安全培训学时统计包含 项目培训（安全培训模块中的集中培训）、教育培训-事故预想、危险预知训练、班组活动-安全日活动
        //技术培训学时统计包含 教育培训-技术问答、 教育培训-技术讲课
        //其他培训学时统计包含 班组活动-政治学习

        /// 班组活动-安全日活动 班组活动-政治学习
        private ActivityIService Aservice = new ActivityService();

        //危险预知训练
        private DangerIService Dservice = new DangerService();

        //教育培训-事故预想 教育培训-技术问答 教育培训-技术讲课
        private IEduBaseInfoService baseService = new EduBaseInfoService();
        //parentType ''：全部，1：安全培训，2：技术培训，3：其他学时培训
        //childrenType ''：全部，1：项目培训，2：事故预想，3：危险预知训练，4：安全日活动，5：技术问答，6：技术讲课，7：政治学习
        public Eduncationdata getEduncationList(string Account, string deptId, string username, string userid, DateTime? from, DateTime? to, string parentType, string childrenType, string name, int page, int pagesize, out int total)
        {

            //数据交互
            var pxpt = Config.GetValue("bzjypx");
            var webclient = new WebClient();
            var data = new Eduncationdata();
            #region 初始化数据源实体
            //安全培训学时-项目培训
            var xmpx = new List<jzpxlist>();
            //安全培训学时-安全日活动
            var aqrhd = new List<ActivityEntity>();
            //安全培训-危险预知训练
            var wxyzxl = new List<DangerEntity>();
            //3安全培训 - 事故预想
            var sgyx = new List<EduBaseInfoEntity>();

            //2 技术培训-技术问答  
            var jswd = new List<EduBaseInfoEntity>();
            //2 技术培训-技术讲课  
            var jsjk = new List<EduBaseInfoEntity>();
            //其他培训学时-政治学习
            var zzxx = new List<ActivityEntity>();
            total = 0;
            #endregion

            //缺少  教育培训-技术讲课
            #region 获取数据
            //查询所有
            if (string.IsNullOrEmpty(parentType) || parentType == "1")
            {
                if (string.IsNullOrEmpty(childrenType) || string.IsNullOrEmpty(parentType) || childrenType.Contains("4"))
                {
                    //安全培训学时-安全日活动
                    aqrhd = Aservice.GetListApp(userid, from, to, name, page, pagesize, "安全日活动", out total).ToList();
                }
                if (string.IsNullOrEmpty(childrenType) || string.IsNullOrEmpty(parentType) || childrenType.Contains("1"))
                {
                    //安全培训学时-项目培训
                    NameValueCollection postVal = new NameValueCollection();
                    postVal.Add("json", "{'allowPaging':true,'business':'GetProjectList','data':{'ProType':'2','Useraccount':'" + Account + "'},'method':null,'pageIndex':1,'pageSize':10000,'tokenId':null,'userId':null}");
                    var getData = webclient.UploadValues(pxpt, postVal);
                    var result = System.Text.Encoding.UTF8.GetString(getData);
                    var dy = JsonConvert.DeserializeObject<jzpxModel>(result);
                    xmpx = dy.Data.Projects;
                    if (xmpx != null)
                    {
                        if (from != null) xmpx = xmpx.Where(x => x.starttime >= Convert.ToDateTime(from)).ToList();
                        if (to != null) xmpx = xmpx.Where(x => x.endtime <= Convert.ToDateTime(to).AddDays(1)).ToList();
                    }
                    else
                    {
                        xmpx = new List<jzpxlist>();
                    }
                }
                if (string.IsNullOrEmpty(childrenType) || string.IsNullOrEmpty(parentType) || childrenType.Contains("3"))
                {
                    //安全培训-危险预知训练
                    wxyzxl = Dservice.GetRecordsApp(username, name, from, to, pagesize, page, out total);
                }
                if (string.IsNullOrEmpty(childrenType) || string.IsNullOrEmpty(parentType) || childrenType.Contains("2"))
                {
                    //3安全培训 - 事故预想
                    try
                    {
                        sgyx = baseService.GetListApp(deptId).Where(x => x.EduType == "3" && x.Flow == "1").ToList(); ;
                        sgyx = sgyx.Where(x => x.AttendPeopleId.Split(',').Contains(userid)).ToList();
                        if (from != null) sgyx = sgyx.Where(x => x.ActivityDate >= Convert.ToDateTime(from)).ToList();
                        if (to != null) sgyx = sgyx.Where(x => x.ActivityDate <= Convert.ToDateTime(to).AddDays(1)).ToList();
                        if (to != null && from != null) sgyx = sgyx.Where(x => x.ActivityDate >= Convert.ToDateTime(from) && x.ActivityDate <= Convert.ToDateTime(to).AddDays(1)).ToList();
                        total = sgyx.Count();
                        if (page > 0 && pagesize > 0)
                        {
                            sgyx = sgyx.Skip(pagesize * (page - 1)).Take(pagesize).ToList();
                        }
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }

                }
            }
            if (string.IsNullOrEmpty(parentType) || parentType == "2")
            {
                if (string.IsNullOrEmpty(childrenType) || string.IsNullOrEmpty(parentType) || childrenType.Contains("5"))
                {
                    //2 技术培训-技术问答  
                    var jswddata = baseService.GetListApp(deptId).Where(x => x.EduType == "2" && x.Flow == "1").ToList();
                    foreach (var item in jswddata)
                    {
                        if (string.IsNullOrEmpty(item.AttendPeopleId))
                        {
                            if (item.TeacherId.Contains(userid) || item.RegisterPeopleId.Contains(userid))
                            {
                                jswd.Add(item);
                            }
                        }
                        else
                        {
                            if (item.AttendPeopleId.Split(',').Contains(userid))
                            {
                                jswd.Add(item);
                            }
                        }
                    }

                    if (from != null) jswd = jswd.Where(x => x.ActivityDate >= Convert.ToDateTime(from)).ToList();
                    if (to != null) jswd = jswd.Where(x => x.ActivityDate <= Convert.ToDateTime(to).AddDays(1)).ToList();
                    if (to != null && from != null) jswd = jswd.Where(x => x.ActivityDate >= Convert.ToDateTime(from) && x.ActivityDate <= Convert.ToDateTime(to).AddDays(1)).ToList();
                    total = jswd.Count();
                    if (page > 0 && pagesize > 0)
                    {
                        jswd = jswd.Skip(pagesize * (page - 1)).Take(pagesize).ToList();
                    }
                }
            }
            if (string.IsNullOrEmpty(parentType) || parentType == "2")
            {
                if (string.IsNullOrEmpty(childrenType) || string.IsNullOrEmpty(parentType) || childrenType.Contains("6"))
                {
                    // 技术培训-技术讲课  
                    jsjk = baseService.GetListApp(deptId).Where(x => x.EduType == "1" && x.Flow == "1").ToList();
                    jsjk = jsjk.Where(x => x.AttendPeopleId.Split(',').Contains(userid)).ToList();
                    if (from != null) jsjk = jsjk.Where(x => x.ActivityDate >= Convert.ToDateTime(from)).ToList();
                    if (to != null) jsjk = jsjk.Where(x => x.ActivityDate <= Convert.ToDateTime(to).AddDays(1)).ToList();
                    if (to != null && from != null) jsjk = jsjk.Where(x => x.ActivityDate >= Convert.ToDateTime(from) && x.ActivityDate <= Convert.ToDateTime(to).AddDays(1)).ToList();
                    total = jsjk.Count();
                    if (page > 0 && pagesize > 0)
                    {
                        jsjk = jsjk.Skip(pagesize * (page - 1)).Take(pagesize).ToList();
                    }
                }
            }
            if (string.IsNullOrEmpty(parentType) || parentType == "3")
            {
                if (string.IsNullOrEmpty(childrenType) || string.IsNullOrEmpty(parentType) || childrenType.Contains("7"))
                {
                    //其他培训学时-政治学习
                    zzxx = Aservice.GetListApp(userid, from, to, name, page, pagesize, "政治学习", out total).ToList();
                }
            }
            #endregion


            #region 进行统计
            var aqpxEduncation = new UserEduncationModel();
            aqpxEduncation.EduncationFile = new List<UserEduncationFileModel>();
            aqpxEduncation.totalTime = 0;
            var jspxEduncation = new UserEduncationModel();
            jspxEduncation.EduncationFile = new List<UserEduncationFileModel>();
            jspxEduncation.totalTime = 0;
            var qtpxEduncation = new UserEduncationModel();
            qtpxEduncation.EduncationFile = new List<UserEduncationFileModel>();
            qtpxEduncation.totalTime = 0;
            //安全培训学时
            var aqpxxs = new List<UserEduncationFileModel>();
            //技术培训学时
            var jspxxs = new List<UserEduncationFileModel>();
            //其他培训学时
            var qtpxxs = new List<UserEduncationFileModel>();
            #region 安全培训学时
            #region 安全培训学时-安全日活动

            var aqrhdEduncation = new List<UserEduncationFileModel>();
            long aqrhdTotalSeconds = 0;
            foreach (var item in aqrhd)
            {
                var One = new UserEduncationFileModel();
                //var TotalTime = string.Empty;
                //总秒数
                var OneTimeSpan = (long)(item.EndTime - item.StartTime).TotalSeconds;
                //超过两小时
                if (OneTimeSpan > 7200)
                {
                    OneTimeSpan = 7200;
                    //TotalTime = "2时";
                }
                //else
                //{
                //    var re = getTimeDetail(OneTimeSpan);
                //    //时
                //    if (re[0] > 0)
                //    {
                //        TotalTime += TotalTime + re[0] + "时";
                //    }
                //    //分
                //    if (re[1] > 0)
                //    {
                //        TotalTime += TotalTime + re[1] + "分";
                //    }
                //    //秒
                //    if (re[2] > 0)
                //    {
                //        TotalTime += TotalTime + re[2] + "秒";
                //    }
                //}
                aqrhdTotalSeconds += OneTimeSpan;
                One.id = item.ActivityId;
                One.trainingTime = item.StartTime.ToString("yyyy-MM-dd") + "-" + item.EndTime.ToString("yyyy-MM-dd");
                One.trainingName = item.Subject;
                One.trainingType = "4";
                One.trainingTimeLength = OneTimeSpan;
                aqrhdEduncation.Add(One);
            }
            aqpxEduncation.EduncationFile.AddRange(aqrhdEduncation);
            aqpxEduncation.totalTime += aqrhdTotalSeconds;
            #endregion
            #region 安全培训-危险预知训练
            var wxyzxlEduncation = new List<UserEduncationFileModel>();
            long wxyzxlTotalSeconds = 0;
            foreach (var item in wxyzxl)
            {
                var One = new UserEduncationFileModel();
                var TotalTime = string.Empty;
                //总秒数
                // var OneTimeSpan = ((DateTime)item.OperDate - (DateTime)item.JobTime).TotalSeconds.ToInt();
                //固定10分中
                long OneTimeSpan = 600;
                //TotalTime = "10分";
                wxyzxlTotalSeconds += OneTimeSpan;
                One.id = item.Id;
                One.trainingTime = Convert.ToDateTime(item.JobTime).ToString("yyyy-MM-dd") + "-" + Convert.ToDateTime(item.OperDate).ToString("yyyy-MM-dd");
                One.trainingName = item.JobName;
                One.trainingType = "3";
                One.trainingTimeLength = OneTimeSpan;
                wxyzxlEduncation.Add(One);
            }
            aqpxEduncation.EduncationFile.AddRange(wxyzxlEduncation);
            aqpxEduncation.totalTime += wxyzxlTotalSeconds;
            #endregion
            #region 安全培训-事故预想
            var sgyxEduncation = new List<UserEduncationFileModel>();
            long sgyxTotalSeconds = 0;
            foreach (var item in sgyx)
            {
                var One = new UserEduncationFileModel();
                var TotalTime = string.Empty;
                //总秒数
                var OneTimeSpan = ((DateTime)item.ActivityEndDate - (DateTime)item.ActivityDate).TotalSeconds.ToInt();
                //超过两小时
                if (OneTimeSpan > 7200)
                {
                    OneTimeSpan = 7200;
                    //TotalTime = "2时";
                }
                //var re = getTimeDetail(OneTimeSpan);
                ////时
                //if (re[0] > 0)
                //{
                //    TotalTime += TotalTime + re[0] + "时";
                //}
                ////分
                //if (re[1] > 0)
                //{
                //    TotalTime += TotalTime + re[1] + "分";
                //}
                ////秒
                //if (re[2] > 0)
                //{
                //    TotalTime += TotalTime + re[2] + "秒";
                //}

                sgyxTotalSeconds += OneTimeSpan;
                One.id = item.ID;
                One.trainingTime = Convert.ToDateTime(item.ActivityDate).ToString("yyyy-MM-dd") + "-" + Convert.ToDateTime(item.ActivityEndDate).ToString("yyyy-MM-dd");
                One.trainingName = item.Theme;
                One.trainingType = "2";
                One.trainingTimeLength = OneTimeSpan;
                sgyxEduncation.Add(One);
            }
            aqpxEduncation.EduncationFile.AddRange(sgyxEduncation);
            aqpxEduncation.totalTime += sgyxTotalSeconds;
            #endregion
            #region 安全培训-项目培训
            var xmpxEduncation = new List<UserEduncationFileModel>();
            long xmpxTotalSeconds = 0;
            foreach (var item in xmpx)
            {
                //获取详情
                NameValueCollection postVal = new NameValueCollection();
                postVal.Add("json", "{'allowPaging':true,'business':'GetRecord','data':{'ProjectId':'" + item.projId + "','Useraccount':'" + Account + "'},'method':null,'pageIndex':1,'pageSize':10000,'tokenId':null,'userId':null}");
                var getData = webclient.UploadValues(pxpt, postVal);
                var result = System.Text.Encoding.UTF8.GetString(getData);
                var dy = JsonConvert.DeserializeObject<jzpxDetail>(result);
                if (dy.Data.UserList == null)
                {
                    continue;
                }
                var getUser = dy.Data.UserList.Where(x => x.USERNAME.Contains(username));
                if (getUser.Count() == 0)
                {
                    continue;
                }
                var One = new UserEduncationFileModel();
                long TotalTime = 0;
                //总秒数
                var time = dy.Data.TrainRecord.pageTime.Replace("时", ",").Replace("分", ",").Replace("秒", "");
                var timesplit = time.Split(',');
                for (int i = 0; i < timesplit.Length; i++)
                {
                    if (i == 0)
                    {
                        TotalTime += Convert.ToInt32(timesplit[i]) * 3600;
                    }
                    if (i == 1)
                    {
                        TotalTime += Convert.ToInt32(timesplit[i]) * 60;
                    }
                    if (i == 2)
                    {
                        TotalTime += Convert.ToInt32(timesplit[i]);
                    }
                }

                //var re = getTimeDetail(OneTimeSpan);
                ////时
                //if (re[0] > 0)
                //{
                //    TotalTime += TotalTime + re[0] + "时";
                //}
                ////分
                //if (re[1] > 0)
                //{
                //    TotalTime += TotalTime + re[1] + "分";
                //}
                ////秒
                //if (re[2] > 0)
                //{
                //    TotalTime += TotalTime + re[2] + "秒";
                //}
                xmpxTotalSeconds += TotalTime;
                One.id = item.projId;
                One.trainingTime = item.starttime.ToString("yyyy-MM-dd") + "-" + item.endtime.ToString("yyyy-MM-dd");
                One.trainingName = item.proname;
                One.trainingType = "1";
                One.trainingTimeLength = TotalTime;
                xmpxEduncation.Add(One);
            }
            aqpxEduncation.EduncationFile.AddRange(xmpxEduncation);
            aqpxEduncation.totalTime += xmpxTotalSeconds;
            #endregion
            #endregion
            #region 技术培训学时
            #region 技术培训-技术问答
            var jswdEduncation = new List<UserEduncationFileModel>();
            long jswdTotalSeconds = 0;
            foreach (var item in jswd)
            {
                var One = new UserEduncationFileModel();
                var TotalTime = string.Empty;
                //总秒数
                var OneTimeSpan = ((DateTime)item.ActivityEndDate - (DateTime)item.ActivityDate).TotalSeconds.ToInt();
                //超过两小时
                if (OneTimeSpan > 7200)
                {
                    OneTimeSpan = 7200;
                    //TotalTime = "2时";
                }
                //var re = getTimeDetail(OneTimeSpan);
                ////时
                //if (re[0] > 0)
                //{
                //    TotalTime += TotalTime + re[0] + "时";
                //}
                ////分
                //if (re[1] > 0)
                //{
                //    TotalTime += TotalTime + re[1] + "分";
                //}
                ////秒
                //if (re[2] > 0)
                //{
                //    TotalTime += TotalTime + re[2] + "秒";
                //}

                jswdTotalSeconds += OneTimeSpan;
                One.id = item.ID;
                One.trainingTime = Convert.ToDateTime(item.ActivityDate).ToString("yyyy-MM-dd") + "-" + Convert.ToDateTime(item.ActivityEndDate).ToString("yyyy-MM-dd");
                One.trainingName = item.Theme;
                One.trainingType = "5";
                One.trainingTimeLength = OneTimeSpan;
                jswdEduncation.Add(One);
            }
            jspxEduncation.EduncationFile.AddRange(jswdEduncation);
            jspxEduncation.totalTime += jswdTotalSeconds;
            #endregion
            #region 技术培训-技术讲课
            var jsjkEduncation = new List<UserEduncationFileModel>();
            long jsjkTotalSeconds = 0;
            foreach (var item in jsjk)
            {
                var One = new UserEduncationFileModel();
                var TotalTime = string.Empty;
                //总秒数
                var OneTimeSpan = ((DateTime)item.ActivityEndDate - (DateTime)item.ActivityDate).TotalSeconds.ToInt();
                //超过两小时
                if (OneTimeSpan > 7200)
                {
                    OneTimeSpan = 7200;
                    //TotalTime = "2时";
                }
                //var re = getTimeDetail(OneTimeSpan);
                ////时
                //if (re[0] > 0)
                //{
                //    TotalTime += TotalTime + re[0] + "时";
                //}
                ////分
                //if (re[1] > 0)
                //{
                //    TotalTime += TotalTime + re[1] + "分";
                //}
                ////秒
                //if (re[2] > 0)
                //{
                //    TotalTime += TotalTime + re[2] + "秒";
                //}

                jsjkTotalSeconds += OneTimeSpan;
                One.id = item.ID;
                One.trainingTime = Convert.ToDateTime(item.ActivityDate).ToString("yyyy-MM-dd") + "-" + Convert.ToDateTime(item.ActivityEndDate).ToString("yyyy-MM-dd");
                One.trainingName = item.Theme;
                One.trainingType = "6";
                One.trainingTimeLength = OneTimeSpan;
                jsjkEduncation.Add(One);
            }
            jspxEduncation.EduncationFile.AddRange(jsjkEduncation);
            jspxEduncation.totalTime += jsjkTotalSeconds;
            #endregion
            #endregion
            #region 其他培训学时
            #region 其他培训学时-政治学习
            var zzxxEduncation = new List<UserEduncationFileModel>();
            long zzxxTotalSeconds = 0;
            foreach (var item in zzxx)
            {
                var One = new UserEduncationFileModel();
                var TotalTime = string.Empty;
                //总秒数
                var OneTimeSpan = (item.EndTime - item.StartTime).TotalSeconds.ToInt();
                if (OneTimeSpan > 7200)
                {
                    OneTimeSpan = 7200;
                    //TotalTime = "2时";
                }
                //var re = getTimeDetail(OneTimeSpan);
                ////时
                //if (re[0] > 0)
                //{
                //    TotalTime += TotalTime + re[0] + "时";
                //}
                ////分
                //if (re[1] > 0)
                //{
                //    TotalTime += TotalTime + re[1] + "分";
                //}
                ////秒
                //if (re[2] > 0)
                //{
                //    TotalTime += TotalTime + re[2] + "秒";
                //}

                zzxxTotalSeconds += OneTimeSpan;
                One.id = item.ActivityId;
                One.trainingTime = item.StartTime.ToString("yyyy-MM-dd") + "—" + item.EndTime.ToString("yyyy-MM-dd");
                One.trainingName = item.Subject;
                One.trainingType = "7";
                One.trainingTimeLength = OneTimeSpan;
                zzxxEduncation.Add(One);
            }
            qtpxEduncation.EduncationFile.AddRange(zzxxEduncation);
            qtpxEduncation.totalTime += zzxxTotalSeconds;
            #endregion
            #endregion

            #region 学时汇总
            data.aqpxEduncation = aqpxEduncation;
            data.jspxEduncation = jspxEduncation;
            data.qtpxEduncation = qtpxEduncation;
            data.allTime = aqpxEduncation.totalTime + jspxEduncation.totalTime + qtpxEduncation.totalTime;
            #endregion
            #endregion
            return data;
        }


        private int[] getTimeDetail(int TotalSeconds)
        {
            int[] re = new int[3];
            //是否小时
            re[0] = TotalSeconds / 3600 > 1 ? (int)Math.Floor((double)(TotalSeconds / 3600)) : 0;
            //是否分钟
            re[1] = TotalSeconds % 3600 > 0 ? TotalSeconds % 3600 : 0;
            //是否秒
            re[2] = TotalSeconds % 600 > 0 ? TotalSeconds % 600 : 0;
            return re;

        }
        ///<summary>
        ///首页（管理层界面-班组）
        /// </summary>
        /// <returns></returns>
        public manageindex GetTotalWork(string deptId, string userid)
        {
            manageindex index = new manageindex();
            var deptcode = new DepartmentBLL().GetEntity(deptId).EnCode;
            var depts = new DepartmentBLL().GetSubDepartments(deptId, null);
            var deptlist = depts.Select(x => x.DepartmentId).ToArray();
            var deptstr = string.Join(",", deptlist);
            object[] total = new object[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            var timeNow = DateTime.Now;
            var from = new DateTime(timeNow.Year, timeNow.Month, 1);
            var to = new DateTime(timeNow.Year, timeNow.Month, 1).AddMonths(1).AddDays(-1);
            int dataTotal = 0;
            //班前班后会
            var dic = new Dictionary<string, string>();
            dic.Add("meetingstarttime", from.ToString("yyyy-MM-dd"));
            dic.Add("meetingstarttimetwo", to.AddDays(1).ToString("yyyy-MM-dd"));
            work.GetDataNew(1, 10000, out dataTotal, dic, deptlist);
            index.bqbhh = dataTotal;
            //危险预知训练
            var yxyzdata = Dservice.GetTrainingsApp(deptstr, from, to.AddDays(1));
            dataTotal = yxyzdata.Count();
            index.wxyzxl = dataTotal;
            //安全日活动 班务会 政治学习 民主管理会
            var hddata = Aservice.GetList(2, deptstr, from, to.AddDays(1));
            var hdtotaldata = hddata.Where(x => x.ActivityType == "安全日活动").ToList();
            dataTotal = hdtotaldata.Count();
            index.aqrhd = dataTotal;
            hdtotaldata = hddata.Where(x => x.ActivityType == "班务会").ToList();
            dataTotal = hdtotaldata.Count();
            index.bwh = dataTotal;
            hdtotaldata = hddata.Where(x => x.ActivityType == "政治学习").ToList();
            dataTotal = hdtotaldata.Count();
            index.zzxx = dataTotal;
            hdtotaldata = hddata.Where(x => x.ActivityType == "民主管理会" || x.ActivityType == "民主生活会").ToList();
            dataTotal = hdtotaldata.Count();
            index.mzglh = dataTotal;
            // 技术讲课 技术问答 事故预想 反事故演习
            var jsdata = baseService.GetList(new string[] { deptstr }, from, to.AddDays(1)).ToList();
            /// 教育培训类型  1.技术讲课   2.技术问答  3.事故预想 4.反事故预想 5.新技术问答
            var Totaldata = jsdata.Where(x => x.EduType == "1").ToList();
            dataTotal = Totaldata.Count();
            index.jsjk = dataTotal;
            Totaldata = jsdata.Where(x => x.EduType == "2" || x.EduType == "5").ToList();
            dataTotal = Totaldata.Count();
            index.jswd = dataTotal;
            Totaldata = jsdata.Where(x => x.EduType == "3").ToList();
            dataTotal = Totaldata.Count();
            index.sgyx = dataTotal;
            Totaldata = jsdata.Where(x => x.EduType == "4").ToList();
            dataTotal = Totaldata.Count();
            index.fsgyx = dataTotal;
            //人均培训学时
            decimal xs = 0;
            var redata = ed.GetGroupCount(deptcode, from, to.AddDays(1));
            redata.Columns.Add("total");
            redata.Columns.Add("avg");
            EducationBLL edubll = new EducationBLL();
            foreach (DataRow row in redata.Rows)
            {
                decimal learns = 0;
                var edulist = edubll.GetAllList().Where(x => x.BZId == row[0].ToString() && x.ActivityDate >= from && x.ActivityDate < to);
                foreach (EduBaseInfoEntity e in edulist)
                {
                    learns += Convert.ToDecimal(e.LearnTime * e.AttendNumber);
                }
                row["total"] = learns;
                if (row[3].ToString() == "" || row[2].ToString() == "0")
                {
                    row["avg"] = "0";

                }
                else
                {
                    row["avg"] = Math.Round(Convert.ToDecimal(row[3]) / Convert.ToDecimal(row[2]), 2);
                    xs = xs + Math.Round(Convert.ToDecimal(row[3]) / Convert.ToDecimal(row[2]), 2);
                }
            }
            index.rjpxxs = xs;
            //获取应急演练列表
            var dyresult = webClientEmergency<GetDrillRecordBaseList>("EmergencyPlatform/GetDrillRecordBaseList",
            "{\"userid\":\"" + userid + "\",\"pageindex\":" + 1 + ",\"pagesize\":" + 2000 + ",\"data\":{\"startdate\":\"" + from.ToString("yyyy-MM-dd") + "\",\"enddate\":\"" + to.AddDays(1).ToString("yyyy-MM-dd") + "\"} }");
            if (dyresult.Code == 0)
            {
                if (dyresult.data.Count > 0)
                {
                    dataTotal = dyresult.Count;
                }
            }
            index.yjylcs = dataTotal;
            return index;
        }

        public decimal GetedDecimal(string deptId, string userid)
        {
            var deptcode = new DepartmentBLL().GetEntity(deptId).EnCode;
            var timeNow = DateTime.Now;
            var from = new DateTime(timeNow.Year, timeNow.Month, 1);
            var to = new DateTime(timeNow.Year, timeNow.Month, 1).AddMonths(1).AddDays(-1);
            //人均培训学时
            decimal xs = 0;
            var redata = ed.GetGroupCount(deptcode, from, to.AddDays(1));
            redata.Columns.Add("total");
            redata.Columns.Add("avg");
            EducationBLL edubll = new EducationBLL();
            foreach (DataRow row in redata.Rows)
            {
                decimal learns = 0;
                var edulist = edubll.GetAllList().Where(x => x.BZId == row[0].ToString() && x.ActivityDate >= from && x.ActivityDate < to);
                foreach (EduBaseInfoEntity e in edulist)
                {
                    learns += Convert.ToDecimal(e.LearnTime * e.AttendNumber);
                }
                row["total"] = learns;
                if (row[3].ToString() == "" || row[2].ToString() == "0")
                {
                    row["avg"] = "0";

                }
                else
                {
                    row["avg"] = Math.Round(Convert.ToDecimal(row[3]) / Convert.ToDecimal(row[2]), 2);
                    xs = xs + Math.Round(Convert.ToDecimal(row[3]) / Convert.ToDecimal(row[2]), 2);
                }
            }

            return xs;
        }

        public class manageindex
        {

            /// <summary>
            /// 班前班后会
            /// </summary>
            public int bqbhh { get; set; }
            /// <summary>
            /// 危险预知训练
            /// </summary>
            public int wxyzxl { get; set; }
            /// <summary>
            /// 安全日活动
            /// </summary>
            public int aqrhd { get; set; }
            /// <summary>
            /// 班务会
            /// </summary>
            public int bwh { get; set; }
            /// <summary>
            /// 政治学习
            /// </summary>
            public int zzxx { get; set; }

            /// <summary>
            /// 民主管理会
            /// </summary>
            public int mzglh { get; set; }
            /// <summary>
            /// 技术讲课
            /// </summary>
            public int jsjk { get; set; }
            /// <summary>
            /// 技术问答
            /// </summary>
            public int jswd { get; set; }
            /// <summary>
            /// 事故预想
            /// </summary>
            public int sgyx { get; set; }
            /// <summary>
            /// 反事故演习
            /// </summary>
            public int fsgyx { get; set; }
            /// <summary>
            /// 人均培训学时
            /// </summary>
            public decimal rjpxxs { get; set; }

            /// <summary>
            /// 应急演练次数
            /// </summary>
            public int yjylcs { get; set; }


        }


        private static T webClientEmergency<T>(string url, string val) where T : class
        {
            var webclient = new WebClient();
            var ApiIp = Config.GetValue("ErchtmsApiUrl");
            NameValueCollection postVal = new NameValueCollection();
            postVal.Add("json", val);
            var getData = webclient.UploadValues(ApiIp + url, postVal);
            var result = System.Text.Encoding.UTF8.GetString(getData);
            return JsonConvert.DeserializeObject<T>(result);
        }
        public class BaseResultEmergency
        {
            public int Code { get; set; }
            public int Count { get; set; }
            public string Info { get; set; }
        }
        public class GetDrillRecordBaseList : BaseResultEmergency
        {
            public List<GetDrillRecordBaseListDetail> data { get; set; }
        }
        public class GetDrillRecordBaseListDetail
        {

            public string id { get; set; }
            public string name { get; set; }
            public string drilltime { get; set; }
            public string filepath { get; set; }
        }
    }
}
