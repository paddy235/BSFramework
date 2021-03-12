using BSFramework.Entity.WorkMeeting;
using BSFramework.IService.WorkMeeting;
using BSFramework.Data.Repository;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;
using BSFramework.Util.Extension;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using System.Data.Common;
using BSFramework.Data;
using System.Text;
using System.Configuration;
using BSFramework.Application.Entity.PeopleManage;
using System.Data;
using BSFramework.Data.Repository;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Application.Entity.EducationManage;
using BSFramework.Application.Entity.Activity;
using BSFramework.Service.WorkMeeting;
using BSFramework.Application.Service.EducationManage;
using BSFramework.Application.Service.Activity;
using System.Reflection;
using BSFramework.Util;
using System.IO;
using Newtonsoft.Json;
using BSFramework.Entity.EvaluateAbout;
using BSFramework.Service.EvaluateAbout;
using BSFramework.Application.Entity.PublicInfoManage.ViewMode;
using BSFramework.Application.IService.PublicInfoManage;
using BSFramework.Util.Log;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.PublicInfoManage
{
    public class AdminPrettyService : RepositoryFactory<WorkmeetingEntity>, IAdminPrettyService
    {
        public List<DepartmentEntity> GetChildren(string deptcode)
        {
            if (string.IsNullOrEmpty(deptcode)) deptcode = "0";
            var sql = "select * from base_department where encode like '" + deptcode + "%'";
            IRepository db = new RepositoryFactory().BaseRepository();
            var list = db.FindList<DepartmentEntity>(sql);
            return list.Where(x => x.Nature == "班组").ToList();
        }
        /// <summary>
        /// 主界面左侧指标 当前月数据
        /// </summary>
        /// <param name="deptcode"></param>
        /// <returns></returns>
        public List<KeyValue> FindCount(string deptcode)
        {
            var query = new RepositoryFactory<MainCount>().BaseRepository();
            var sql = string.Format(@"select b1.jsjk BZ_JSJK,b2.jswd BZ_JSWD,b3.sgyx BZ_SGYX,b4.fsgyx BZ_FSGXX,c.total BZ_BQBHH,d.total BZ_AQRHD,d1.total BZ_AQXXR,
		e.total BZ_WXYZXL,f.total BZ_WCRW,g.total BZ_SYRW,h.total BZ_ZZXX,i.total BZ_LDBHJC,j.total BZ_BWH,k.total BZ_MZGLH,
		l.total BZ_ZDXX,r.total BZ_JNJL,m.total BZ_DDZP,n.total BZ_JXSBBZ,o.total BZ_QCCG,p.total BZ_GLCXCG,q.total BZ_7SCXTA,
        s.total BZ_RSFXYK from (select count(1) jsjk from wg_edubaseinfo where edutype = '1' and flow = '1'
   and bzid in(select departmentid from base_department where encode like '{0}%')
	 and DATE_FORMAT(activitydate,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) b1,
(select count(1) jswd from wg_edubaseinfo where (edutype = '2' or edutype = '5') and flow = '1'
   and bzid in(select departmentid from base_department where encode like '{0}%')
	 and DATE_FORMAT(activitydate,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) b2,
(select count(1) sgyx from wg_edubaseinfo where edutype = '3' and flow = '1'
   and bzid in(select departmentid from base_department where encode like '{0}%')
   and DATE_FORMAT(activitydate,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) b3,
(select count(1) fsgyx from wg_edubaseinfo where edutype = '4' and flow = '1'
   and bzid in(select departmentid from base_department where encode like '{0}%')
   and DATE_FORMAT(activitydate,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) b4,
(select count(1) total from wg_workmeeting where isover = 1
   and groupid in(select departmentid from base_department where encode like '{0}%')
		and DATE_FORMAT(meetingstarttime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) c,
(select count(1) total from wg_activity where activitytype = '安全日活动'
	 and state = 'Finish'
   and groupid in(select departmentid from base_department where encode like '{0}%')
   and DATE_FORMAT(starttime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) d,
(select count(1) total from wg_edactivity where activitytype = '安全学习日'
	 and state = 'Finish'
   and groupid in(select departmentid from base_department where encode like '{0}%')
   and DATE_FORMAT(starttime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) d1,
(select count(1) total from wg_danger where status = '2' 
   and groupid in(select departmentid from base_department where encode like '{0}%')
   and DATE_FORMAT(jobtime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) e,
(select sum(total) total from (select w.groupid,w.meetingid,count(c.jobid) total from wg_workmeeting w 
						left join wg_meetingandjob j
            on w.meetingid = j.endmeetingid
						left join wg_meetingjob c
						on j.jobid = c.jobid
             and c.isfinished = 'finish'
						where w.meetingtype='班后会' and DATE_FORMAT(w.meetingstarttime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')
						and w.groupid in(select departmentid from base_department where encode like '{0}%')
            group by w.meetingid) f1) f,
(select sum(total) total from (select w.groupid,w.meetingid,count(c.jobid) total from wg_workmeeting w 
						left join wg_meetingandjob j
            on w.meetingid = j.endmeetingid
						left join wg_meetingjob c
						on j.jobid = c.jobid
						where w.meetingtype='班后会' and DATE_FORMAT(w.meetingstarttime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')
						and w.groupid in(select departmentid from base_department where encode like '{0}%')
            group by w.meetingid) f1) g,
(select count(1) total from wg_activity where activitytype = '政治学习'
						and state = 'Finish'
						and groupid in(select departmentid from base_department where encode like '{0}%')
						and DATE_FORMAT(starttime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) h,
(select count(1) total from wg_activity where activitytype = '劳动保护监查'
						and state = 'Finish'
						and groupid in(select departmentid from base_department where encode like '{0}%')
						and DATE_FORMAT(starttime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) i,
(select count(1) total from wg_activity where activitytype = '班务会'
						and state = 'Finish'
						and groupid in(select departmentid from base_department where encode like '{0}%')
						and DATE_FORMAT(starttime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) j,
(select count(1) total from wg_activity where activitytype = '民主管理会'
						and state = 'Finish'
						and groupid in(select departmentid from base_department where encode like '{0}%')
						and DATE_FORMAT(starttime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) k,
(select count(1) total from wg_activity where activitytype = '制度学习'
						and state = 'Finish'
						and groupid in(select departmentid from base_department where encode like '{0}%')
						and DATE_FORMAT(starttime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) l,
(select count(1) total from wg_activity where activitytype = '节能记录'
						and state = 'Finish'
						and groupid in(select departmentid from base_department where encode like '{0}%')
						and DATE_FORMAT(starttime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) r,
(select count(1) total from wg_sevenspicture a join base_fileinfo b
                        on a.id = b.recid 
                        where a.deptid in(select departmentid from base_department where encode like '{0}%')
						and DATE_FORMAT(planeStartDate,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) m,
(select count(1) total from (select a.fullname,count(b.id) total from base_department a
                        left join wg_performanceup b
                        on a.departmentid = b.departmentid and DATE_FORMAT(b.usetime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')
                        where a.encode like '{0}%'
                        and a.nature = '班组'
                        group by a.fullname) n1 where n1.total > 0) n,
(select count(1) total from wg_qcactivity where 
					    deptid in(select departmentid from base_department where encode like '{0}%')
						and DATE_FORMAT(subjecttime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) o,
(select count(1) total from wg_workinnovation where 
					    deptid in(select departmentid from base_department where encode like '{0}%')
						and DATE_FORMAT(reporttime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) p,
(select count(1) total from wg_sevensoffice  where 
					    deptid in(select departmentid from base_department where encode like '{0}%')
						and DATE_FORMAT(createdate,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) q,
(select count(1) total FROM `wg_traininguser` AS `Extent1` INNER JOIN 
	 (select *  from wg_humandangertraining  where 
					    deptid in(select departmentid from base_department where encode like '{0}%')
						and DATE_FORMAT(createtime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m'))
						 AS `Extent2` ON `Extent1`.`TrainingId` = `Extent2`.`TrainingId` 
where DATE_FORMAT(TrainingTime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m') and IsDone=1 and IsMarked=1) s", deptcode);
            var data = query.FindList(sql).FirstOrDefault();
            var list = new List<KeyValue>();
            PropertyInfo[] propertys = data.GetType().GetProperties();
            foreach (PropertyInfo pinfo in propertys)
            {
                list.Add(new KeyValue { key = pinfo.Name, value = pinfo.GetValue(data, null).ToString() });
            }
            var wc = Convert.ToInt32(list.Where(x => x.key == "BZ_WCRW").FirstOrDefault().value);
            var all = Convert.ToInt32(list.Where(x => x.key == "BZ_SYRW").FirstOrDefault().value);
            decimal perc = 0;
            if (wc != 0 && all != 0)
            {
                perc = Math.Round(Convert.ToDecimal(wc * 100) / Convert.ToDecimal(all), 2);
            }
            list[3].value = perc.ToString() + "%";


            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deptid">班组id</param>
        ///<param name="TerminalType">统计类型</param>
        /// <returns></returns>
        public List<KeyValue> FindCount1(string deptid, string TerminalType)
        {
            var from = new DateTime(DateTime.Now.Year, 1, 1);
            var to = new DateTime(DateTime.Now.Year, 1, 1);
            if (!string.IsNullOrEmpty(TerminalType))
            {
                var now = DateTime.Now;
                //TerminalType 月  季  年
                switch (TerminalType)
                {
                    case "0":
                        from = new DateTime(now.Year, now.Month, 1);
                        to = from.AddMonths(1).AddMinutes(-1);
                        break;
                    case "1":
                        var month = now.Month;
                        from = month <= 3 ? new DateTime(now.Year, 1, 1) : month <= 6 && month >= 4 ? new DateTime(now.Year, 4, 1)
                            : month <= 9 && month >= 7 ? new DateTime(now.Year, 7, 1) : new DateTime(now.Year, 10, 1);
                        to = from.AddMonths(3).AddMinutes(-1);
                        break;
                    case "2":
                        to = from.AddYears(1).AddMinutes(-1);
                        break;
                    default:
                        break;
                }
            }
            List<KeyValue> list = new List<KeyValue>();
            var edus = new EduBaseInfoService().GetAllList().Where(x => x.BZId == deptid && x.ActivityDate >= from);
            if (from != to)
            {
                edus = new EduBaseInfoService().GetAllList().Where(x => x.BZId == deptid && x.ActivityDate >= from && x.ActivityDate <= to);
            }
            decimal count1 = 0;
            foreach (EduBaseInfoEntity e in edus)
            {
                count1 += Convert.ToDecimal(e.AttendNumber * e.LearnTime);
            }

            var sql = string.Format(@"select count(*) from wg_danger where groupid = '{0}' and jobtime >= '{1}' and status = '2'", deptid, from);
            if (from != to)
            {
                sql = string.Format(@"select count(*) from wg_danger where groupid = '{0}' and jobtime >= '{1}' and jobtime <= '{2}'  and status = '2'", deptid, from, to);
            }
            var data = this.BaseRepository().FindTable(sql.ToString());
            int count2 = Convert.ToInt32(data.Rows[0][0]);

            sql = string.Format(@"select count(*) from wg_activity where activitytype = '安全日活动' and groupid = '{0}' and starttime >= '{1}' and state = 'Finish'", deptid, from);
            if (from != to)
            {
                sql = string.Format(@"select count(*) from wg_activity where activitytype = '安全日活动' and groupid = '{0}' and starttime >= '{1}' and starttime <= '{2}' and state = 'Finish'", deptid, from, to);
            }
            data = this.BaseRepository().FindTable(sql.ToString());
            int count3 = Convert.ToInt32(data.Rows[0][0]);

            list.Add(new KeyValue { key = "ZD_PXXS", value = count1.ToString() });
            list.Add(new KeyValue { key = "ZD_WXYZXL", value = count2.ToString() });
            list.Add(new KeyValue { key = "ZD_AQRHD", value = count3.ToString() });
            return list;
        }
        /// <summary>
        /// 右界面  班组考评
        /// </summary>
        /// <returns></returns>
        public IList<Group> GetKPGroups()
        {
            var bll = new EvaluateService();
            int total = 0;
            var ids = bll.GetEvaluationsFoWeb("", "", 10000, 1, out total).OrderByDescending(x => x.CreateTime).Select(x => x.EvaluateId).ToList();
            string id = null;
            if (ids != null && ids.Count > 0)
            {
                id = ids[0];

                var data = bll.GetCalcScoreNew(id, "");
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].Score2 = 0;
                    if (data[i].Score != 0)
                    {

                        decimal reScore = Math.Round((data[i].Score1 / data[i].Score * 100), 2);
                        double Percent = (double)Math.Round(reScore, 2);//得分率
                        data[i].Percent = Percent + "%";
                    }
                    else
                    {
                        data[i].Percent = "0%";
                    }
                    data[i].Percent1 = "0%";
                }
                data = data.OrderByDescending(x => x.Score1).ToList();
                return data;
            }

            return new List<Group>();
        }
        /// <summary>
        /// 主界面右侧指标 mode :1.未开班会 2.未开安全日活动 3.未开展KYT 4.培训次数未达标 5.未评价台账 6.未闭环任务 7安全学习日8人身风险预控
        /// </summary>
        /// <param name="deptcode"></param>
        /// <param name="mode">1.未开班会 2.未开安全日活动 3.未开展KYT 4.培训次数未达标 5.未评价台账 6.未闭环任务 7安全学习日8人身风险预控</param>
        /// <returns></returns>
        public int GetMonthUndoGroups(string deptcode, string mode)
        {
            int count = 0;
            var from = DateTime.Now.AddDays(-30);
            var to = DateTime.Now;
            var days = 0;
            var days1 = 0;
            var depts = GetChildren(deptcode);
            #region 7天未开班会
            if (mode == "1")
            {
                var sql = @"select * from wg_workmeeting w where DATE_SUB(CURDATE(),INTERVAL 30 DAY) <= meetingstarttime order by meetingstarttime  ";
                var list = new RepositoryFactory<WorkmeetingEntity>().BaseRepository().FindList(sql);

                var grouplist = new List<WorkmeetingEntity>();
                var obj = new WorkmeetingEntity();
                var nextobj = new WorkmeetingEntity();

                foreach (DepartmentEntity d in depts)
                {
                    grouplist = list.Where(x => x.GroupId == d.DepartmentId).ToList();
                    if (grouplist.Count < 4) //少于4次
                    {
                        count++;
                    }
                    else
                    {
                        //第一条数据与开始日期对比，最后一条数据与今天比较
                        obj = grouplist.First();
                        nextobj = grouplist.Last();
                        days = (obj.MeetingStartTime - from).Days;
                        days1 = (to - nextobj.MeetingStartTime).Days;
                        if (days > 7 || days1 > 7)
                        {
                            count++;
                        }
                        else //数据内循环比较
                        {
                            for (int i = 1; i < grouplist.Count(); i++)
                            {
                                obj = grouplist[i - 1];
                                nextobj = grouplist[i];
                                days = (nextobj.MeetingStartTime - obj.MeetingStartTime).Days;
                                if (days >= 7)
                                {
                                    count++;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region 14天未开班组活动
            if (mode == "2")
            {
                var sql = @"select * from wg_activity w where activitytype = '安全日活动' and DATE_SUB(CURDATE(),INTERVAL 30 DAY) <= starttime order by starttime ";
                var list = new RepositoryFactory<ActivityEntity>().BaseRepository().FindList(sql);

                var grouplist = new List<ActivityEntity>();
                var obj = new ActivityEntity();
                var nextobj = new ActivityEntity();
                foreach (DepartmentEntity d in depts)
                {
                    grouplist = list.Where(x => x.GroupId == d.DepartmentId).ToList();
                    if (grouplist.Count < 2) //少于两次
                    {
                        count++;
                    }
                    else
                    {
                        //第一条数据与开始日期对比，最后一条数据与今天比较
                        obj = grouplist.First();
                        nextobj = grouplist.Last();
                        days = (obj.StartTime - from).Days;
                        days1 = (to - nextobj.StartTime).Days;
                        if (days > 14 || days1 > 14)
                        {
                            count++;
                        }
                        else  //数据内循环比较
                        {
                            for (int i = 1; i < grouplist.Count(); i++)
                            {
                                obj = grouplist[i - 1];
                                nextobj = grouplist[i];
                                days = (nextobj.StartTime - obj.StartTime).Days;
                                if (days >= 14)
                                {
                                    count++;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region 14天未开危险预知训练
            if (mode == "3")
            {
                var sql = @"select * from wg_danger where DATE_SUB(CURDATE(),INTERVAL 30 DAY) <= createdate order by jobtime";
                var list = new RepositoryFactory<DangerEntity>().BaseRepository().FindList(sql);

                var grouplist = new List<DangerEntity>();
                var obj = new DangerEntity();
                var nextobj = new DangerEntity();
                foreach (DepartmentEntity d in depts)
                {
                    grouplist = list.Where(x => x.GroupId == d.DepartmentId && x.JobTime != null).ToList();
                    if (grouplist.Count < 2)
                    {
                        count++;
                    }
                    else
                    {
                        //第一条数据与开始日期对比，最后一条数据与今天比较
                        obj = grouplist.First();
                        nextobj = grouplist.Last();
                        days = (obj.JobTime.Value - from).Days;
                        days1 = (to - nextobj.JobTime.Value).Days;
                        if (days > 14 || days1 > 14)
                        {
                            count++;
                        }
                        else  //数据内循环比较
                        {
                            for (int i = 1; i < grouplist.Count(); i++)
                            {
                                obj = grouplist[i - 1];
                                nextobj = grouplist[i];
                                days = (nextobj.JobTime.Value - obj.JobTime.Value).Days;
                                if (days >= 14)
                                {
                                    count++;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region 今日未开展人身风险预控
            if (mode == "8")
            {
                var dept = new DepartmentService();
                var deptList = dept.GetSubDepartments(deptcode, null);
                var bll = new HumanDangerTrainingService();
                var now = DateTime.Now;
                var rsfrom = new DateTime(now.Year, now.Month, now.Day);
                var rsto = rsfrom.AddDays(1).AddMinutes(-1);
                var total = 0;
                var data = bll.GetUndo(deptList.Select(x => x.DepartmentId).ToArray(), string.Empty, rsfrom, rsto, 1, 10, out total);
                count = total;
            }
            #endregion
            if (mode == "4")
            {

            }
            if (mode == "5")
            {
                count += GetActs(deptcode).Count;
                count += GetEdus(deptcode).Count;
                count += GetDangers(deptcode).Count;
            }

            #region 14天未开班组活动
            if (mode == "7")
            {
                var sql = @"select * from wg_edactivity w where activitytype = '安全学习日' and DATE_SUB(CURDATE(),INTERVAL 30 DAY) <= starttime order by starttime ";
                var list = new RepositoryFactory<EdActivityEntity>().BaseRepository().FindList(sql);

                var grouplist = new List<EdActivityEntity>();
                var obj = new EdActivityEntity();
                var nextobj = new EdActivityEntity();
                foreach (DepartmentEntity d in depts)
                {
                    grouplist = list.Where(x => x.GroupId == d.DepartmentId).ToList();
                    if (grouplist.Count < 2) //少于两次
                    {
                        count++;
                    }
                    else
                    {
                        //第一条数据与开始日期对比，最后一条数据与今天比较
                        obj = grouplist.First();
                        nextobj = grouplist.Last();
                        days = (obj.StartTime - from).Days;
                        days1 = (to - nextobj.StartTime).Days;
                        if (days > 14 || days1 > 14)
                        {
                            count++;
                        }
                        else  //数据内循环比较
                        {
                            for (int i = 1; i < grouplist.Count(); i++)
                            {
                                obj = grouplist[i - 1];
                                nextobj = grouplist[i];
                                days = (nextobj.StartTime - obj.StartTime).Days;
                                if (days >= 14)
                                {
                                    count++;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            return count;
        }
        public List<ActivityEntity> GetActs(string deptcode)
        {
            var query = new RepositoryFactory<ActivityEntity>().BaseRepository();
            var sql = string.Format(@"select * from (select a.*,count(b.activityevaluateid) as total from wg_activity a
left join wg_activityevaluate b on a.activityid = b.activityid
where DATE_SUB(CURDATE(),INTERVAL 30 DAY) <= starttime and a.groupid in(select departmentid from base_department where encode like '{0}%' and nature = '班组')
group by a.activityid) a where total  =0 order by a.starttime", deptcode);
            return query.FindList(sql).ToList();
        }
        public List<EduBaseInfoEntity> GetEdus(string deptcode)
        {
            var query = new RepositoryFactory<EduBaseInfoEntity>().BaseRepository();
            string sql = string.Format(@"select * from (select a.*,count(b.activityevaluateid) as total from wg_edubaseinfo a
left join wg_activityevaluate b on a.id = b.activityid
where DATE_SUB(CURDATE(),INTERVAL 30 DAY) <= activitydate and a.bzid in (select departmentid from base_department where encode like '{0}%' and nature = '班组')
group by a.id) a where total = 0 order by a.activitydate", deptcode);
            return query.FindList(sql).ToList();
        }
        public List<DangerEntity> GetDangers(string deptcode)
        {
            var query = new RepositoryFactory<DangerEntity>().BaseRepository();
            string sql = string.Format(@"select * from (select a.*,count(b.activityevaluateid) as total from wg_danger a
left join wg_activityevaluate b on a.id = b.activityid
where DATE_SUB(CURDATE(),INTERVAL 30 DAY) <= jobtime and a.groupid in (select departmentid from base_department where encode like '{0}%' and nature = '班组')
group by a.id) a where total = 0 order by a.jobtime", deptcode);
            return query.FindList(sql).ToList();
        }
        public DataTable GetMeeting(string deptcode, string from, string to)
        {
            if (string.IsNullOrEmpty(deptcode)) deptcode = "0";
            //本季度，部门下所有班组已进行的班会次数
            var sql = string.Format(@"select a.departmentid,a.fullname,COUNT(b.meetingid) as count from base_department a left join wg_workmeeting b on a.departmentid = b.groupid and b.meetingstarttime >'{1}' and b.meetingstarttime <'{2}' where encode like '{0}%' and nature='班组' group by a.departmentid,a.fullname ", deptcode, from, to);
            var data = this.BaseRepository().FindTable(sql.ToString());
            return data;
        }
        public int GetUndoMeeting(string deptid, string from, string to)
        {


            var sql = string.Format(@" select count(*) from wg_workmeeting where groupid = '{0}' and meetingstarttime > '{1}' and meetingstarttime < '{2}'", deptid, from, to);
            var data = this.BaseRepository().FindTable(sql.ToString());
            return Convert.ToInt32(data.Rows[0][0]);
        }
        public DataTable GetMeetingNew(string type, string deptcode, string from)
        {
            if (string.IsNullOrEmpty(deptcode)) deptcode = "0";
            var sql = string.Format(@"select a.departmentid,a.fullname,COUNT(b.meetingid) as count from base_department a left join wg_workmeeting b on a.departmentid = b.groupid where encode like '{0}%' and nature='班组' group by a.departmentid,a.fullname and b.meetingstarttime >'{1}' and b.meetingtype='{2}' ", deptcode, from, type);
            var data = this.BaseRepository().FindTable(sql.ToString());
            return data;
        }

        public DataTable GetLllegals(string deptid, string from)
        {
            var code = "0";

            var dept = new DepartmentService().GetEntity(deptid);
            if (dept != null)
            {
                code = dept.EnCode;
                if (dept.Description == "安全部" || dept.DepartmentId == "0")
                {
                    code = "0";
                }
            }

            var sql = string.Format(@"select a.departmentid,a.fullname,COUNT(b.ID) as count from base_department a left join wg_lllegalregister b on a.departmentid = b.lllegalteamid and b.flowstate='待整改' and b.LllegalTime > '{1}' where encode like '{0}%' and nature='班组' group by a.departmentid,a.fullname ", code, from);
            var data = this.BaseRepository().FindTable(sql.ToString());
            return data;
        }

        public DataTable GetEducations(string deptcode, string from)
        {
            if (string.IsNullOrEmpty(deptcode)) deptcode = "0";
            var sql = string.Format(@"select a.departmentid,a.fullname,COUNT(b.id) as count from base_department a left join wg_edubaseinfo b on a.departmentid = b.bzid and b.appraisecontent is null and b.activitydate > '{1}' and flow = '1' where encode like '{0}%' and nature='班组' group by a.departmentid,a.fullname ", deptcode, from);
            var data = this.BaseRepository().FindTable(sql.ToString());
            return data;
        }

        public DataTable GetActivitys(string deptcode, string from)
        {
            if (string.IsNullOrEmpty(deptcode)) deptcode = "0";
            var sql = string.Format(@"select a.departmentid,a.fullname,COUNT(b.activityid) as count from base_department a left join wg_activity b on a.departmentid = b.groupid and b.activitytype = '安全日活动' and b.starttime > '{1}' where encode like '{0}%' and nature='班组' group by a.departmentid,a.fullname", deptcode, from);
            var data = this.BaseRepository().FindTable(sql.ToString());
            return data;
        }
        public DataTable GetDanger(string deptcode, string from)
        {
            if (string.IsNullOrEmpty(deptcode)) deptcode = "0";
            var sql = string.Format(@"select a.departmentid,a.fullname,COUNT(b.id) as count from base_department a left join wg_danger b on a.departmentid = b.groupid and b.status != 2 and b.jobtime > '{1}' where encode like '{0}%' and nature='班组' group by a.departmentid,a.fullname ", deptcode, from);
            var data = this.BaseRepository().FindTable(sql.ToString());
            return data;
        }

        public int GetAllMeeting(string deptcode, string from)
        {
            if (string.IsNullOrEmpty(deptcode)) deptcode = "0";
            var sql = string.Format(@"select count(*) from wg_workmeeting where groupid in (select departmentid from base_department where encode like '{0}%' and nature = '班组') and meetingstarttime > '{1}'", deptcode, from);
            var data = this.BaseRepository().FindTable(sql.ToString());
            return Convert.ToInt32(data.Rows[0][0]);
        }

        public int GetAllActivity(string deptcode, string from)
        {
            if (string.IsNullOrEmpty(deptcode)) deptcode = "0";
            var sql = string.Format(@"select count(*) from wg_activity where starttime > '{0}' and groupid in (select departmentid from base_department where encode like '{1}%' and nature = '班组')", from, deptcode);
            var data = this.BaseRepository().FindTable(sql.ToString());
            return Convert.ToInt32(data.Rows[0][0]);
        }

        public int GetAllEducation(string deptcode, string from)
        {
            if (string.IsNullOrEmpty(deptcode)) deptcode = "0";
            var sql = string.Format(@"select count(*) from wg_edubaseinfo where bzid in (select departmentid from base_department where encode like '{0}%' and nature = '班组') and activitydate > '{1}' and flow = '1'", deptcode, from);
            var data = this.BaseRepository().FindTable(sql.ToString());
            return Convert.ToInt32(data.Rows[0][0]);
        }


        public int GetAllLllegal(string deptid, string from)
        {
            var code = "0";

            var dept = new DepartmentService().GetEntity(deptid);
            if (dept != null)
            {
                code = dept.EnCode;
                if (dept.Description == "安全部" || dept.DepartmentId == "0")
                {
                    code = "0";
                }
            }
            var sql = string.Format(@"select count(*) from wg_lllegalregister where lllegalteamid in (select departmentid from base_department where encode like '{0}%' and nature = '班组') and LllegalTime > '{1}'", code, from);
            var data = this.BaseRepository().FindTable(sql.ToString());
            return Convert.ToInt32(data.Rows[0][0]);
        }

        public int GetAllToolborrow(string deptcode, string from)
        {
            if (string.IsNullOrEmpty(deptcode)) deptcode = "0";
            var sql = string.Format(@"select count(*) from wg_toolborrow where bzid in (select departmentid from base_department where encode like '{0}%' and nature = '班组') and borrowdate > '{1}'", deptcode, from);
            var data = this.BaseRepository().FindTable(sql.ToString());
            return Convert.ToInt32(data.Rows[0][0]);
        }

        public int GetAllDanger(string deptcode, string from)
        {
            if (string.IsNullOrEmpty(deptcode)) deptcode = "0";
            var sql = string.Format(@"select count(*) from wg_danger where deptcode like '{0}%' and jobtime > '{1}' and status = '2'", deptcode, from);
            var data = this.BaseRepository().FindTable(sql.ToString());
            return Convert.ToInt32(data.Rows[0][0]);
        }
        public int GetAllEmergencyWork(string deptname, string from)
        {
            if (string.IsNullOrEmpty(deptname)) deptname = "0";
            var sql = string.Format(@"select count(*) from wg_emergencywork where tocompiledeptid in (select departmentid from base_department where encode like '{0}%' and nature = '班组') and tocompiledate > '{1}'", deptname, from);
            var data = this.BaseRepository().FindTable(sql.ToString());
            return Convert.ToInt32(data.Rows[0][0]);
        }

        public int GetAllGlassStock(string deptcode, string from)
        {
            if (string.IsNullOrEmpty(deptcode)) deptcode = "0";
            var sql = string.Format(@"select count(*) from wg_glassstock where bzid in (select departmentid from base_department where encode like '{0}%' and nature = '班组') and createdate > '{1}'", deptcode, from);
            var data = this.BaseRepository().FindTable(sql.ToString());
            return Convert.ToInt32(data.Rows[0][0]);
        }

        public List<FileInfoEntity> GetJobs(string deptcode, string from)
        {
            var db = new RepositoryFactory().BaseRepository();
            if (string.IsNullOrEmpty(deptcode)) deptcode = "0";
            //var sql = string.Format(@"select jobid from wg_meetingjob where groupid in (select departmentid from base_department where encode like '{0}%' and nature = '班组') and starttime > '{1}' order by starttime desc", deptcode, from);
            //var data = this.BaseRepository().FindTable(sql.ToString());
            //var db = new RepositoryFactory().BaseRepository();

            //var list = db.FindList<MeetingJobEntity>(sql).ToList();
            //return list;


            var sql = string.Format(@"select fileid,filepath from base_fileinfo where recid in (select jobid from wg_meetingjob where groupid in (select departmentid from base_department where encode like '{0}%' and nature = '班组') and starttime > '{1}' order by starttime desc) and description = '照片' order by createdate desc", deptcode, from);
            var list = db.FindList<FileInfoEntity>(sql).ToList();
            return list;
        }
        #region 获取指标数据的方法合集
        /// <summary>
        /// 查询所有的班组指标
        /// </summary>
        /// <param name="keyValuePairs">编码与查询范围的集合  key（code 指标的编码）  value（terminalType "0"月 "1"季  "2"年 默认查当天的，查当天的传空字符串）</param>
        /// <param name="deptIds">取数据的部门范围</param>
        /// <returns></returns>
        public List<KeyValue> FindBZAllCount(Dictionary<string, string> keyValuePairs, List<string> deptIds)
        {
            List<KeyValue> kvs = new List<KeyValue>();
            var t = typeof(AdminPrettyService);
            ConstructorInfo constructor = t.GetConstructor(Type.EmptyTypes);
            object thisClassObj = constructor.Invoke(new object[] { });
            List<Task> taskList = new List<Task>();

            var parallel = Parallel.ForEach(keyValuePairs, x =>
            {
                lock (kvs)
                {
                    var now = DateTime.Now;
                    var from = new DateTime(now.Year, now.Month, 1);
                    var to = new DateTime(now.Year, now.Month, now.Day);
                    if (!string.IsNullOrEmpty(x.Value))
                    {
                        //TerminalType 月  季  年
                        switch (x.Value)
                        {
                            case "0":
                                from = new DateTime(now.Year, now.Month, 1);
                                to = from.AddMonths(1);
                                break;
                            case "1":
                                var month = now.Month;
                                from = month <= 3 ? new DateTime(now.Year, 1, 1) : month <= 6 && month >= 4 ? new DateTime(now.Year, 4, 1)
                                    : month <= 9 && month >= 7 ? new DateTime(now.Year, 7, 1) : new DateTime(now.Year, 10, 1);
                                to = from.AddMonths(3);
                                break;
                            case "2":
                                from = new DateTime(now.Year, 1, 1);
                                to = from.AddYears(1);
                                break;
                            default:
                                break;
                        }
                    }
                    MethodInfo thisMethod = t.GetMethod(x.Key.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    if (thisMethod != null)//找到方法才执行
                    {
                        object returnVal = thisMethod.Invoke(thisClassObj, new object[] { deptIds, from, to });
                        kvs.Add(new KeyValue() { dataType = 0, key = x.Key.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries)[0], value = returnVal == null ? null : returnVal.ToString() });
                    }
                }
            });

            #region 废弃代码

            //foreach (var kv in keyValuePairs)
            //{
            //    var taskCurrent =  Task.Run(() =>
            //    {
            //        var now = DateTime.Now;
            //        var from = new DateTime(now.Year, now.Month, 1);
            //        var to = new DateTime(now.Year, now.Month, now.Day);
            //        if (!string.IsNullOrEmpty(kv.Value))
            //        {
            //            //TerminalType 月  季  年
            //            switch (kv.Value)
            //            {
            //                case "0":
            //                    from = new DateTime(now.Year, now.Month, 1);
            //                    to = from.AddMonths(1);
            //                    break;
            //                case "1":
            //                    var month = now.Month;
            //                    from = month <= 3 ? new DateTime(now.Year, 1, 1) : month <= 6 && month >= 4 ? new DateTime(now.Year, 4, 1)
            //                        : month <= 9 && month >= 7 ? new DateTime(now.Year, 7, 1) : new DateTime(now.Year, 10, 1);
            //                    to = from.AddMonths(3);
            //                    break;
            //                case "2":
            //                    from = new DateTime(now.Year, 1, 1);
            //                    to = from.AddYears(1);
            //                    break;
            //                default:
            //                    break;
            //            }
            //        }
            //        MethodInfo thisMethod = t.GetMethod(kv.Key);
            //        if (thisMethod != null)//找到方法才执行
            //        {
            //            try
            //            {
            //                object returnVal = thisMethod.Invoke(thisClassObj, new object[] { deptIds, from, to });
            //                kvs.Add(new KeyValue() { dataType = 1, key = kv.Key, value = returnVal == null ? null : returnVal.ToString() });
            //            }
            //            catch (Exception ex)
            //            {
            //                WriteLog.AddLog($"方法{kv.Key}执行报错：{ex.Message}\r\n错误信息：{JsonConvert.SerializeObject(ex)}", "IndexSelectError");
            //                throw;
            //            }
            //        }
            //    });
            //    taskList.Add(taskCurrent);
            //}


            //if (taskList !=null && taskList.Count>0)
            //{
            //    var task = Task.Factory.ContinueWhenAll(taskList.ToArray(), completedTask =>
            //    {

            //        //WriteLog.AddLog($"{completedTask.Length}条方法执行完毕", "IndexSelectInfo");
            //    });
            //    while (!task.IsCompleted)
            //    {

            //    }
            //}

            #endregion

            return kvs;
        }

        /// <summary>
        /// 技术讲课 
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int BZ_JSJK(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $@"select count(1) jsjk from wg_edubaseinfo where edutype = '1' and flow = '1'
                                and bzid in('{string.Join("','", deptIds)}')
                                and activitydate > '{from:yyyy-MM-dd}' and activitydate <'{to:yyyy-MM-dd}'";
            int count = GetValueBySql(sql);
            return count;
        }

        /// <summary>
        /// 技术问答
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int BZ_JSWD(List<string> deptIds, DateTime from, DateTime to)
        {

            string sql = $@"select count(1) jswd from wg_edubaseinfo where (edutype = '2' or edutype = '5') and flow = '1'
                                and bzid in('{string.Join("','", deptIds)}')
                                and activitydate > '{from:yyyy-MM-dd}' and activitydate <'{to:yyyy-MM-dd}'";
            int count = GetValueBySql(sql);
            return count;
        }


        /// <summary>
        /// 事故预想
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int BZ_SGYX(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $@"select count(1) jswd from wg_edubaseinfo where edutype = '3' and flow = '1'
                                and bzid in('{string.Join("','", deptIds)}')
                                and activitydate > '{from:yyyy-MM-dd}' and activitydate <'{to:yyyy-MM-dd}'";
            int count = GetValueBySql(sql);
            return count;
        }

        /// <summary>
        /// 反事故演习
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int BZ_FSGXX(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $@"select count(1) jswd from wg_edubaseinfo where edutype = '4' and flow = '1'
                                and bzid in('{string.Join("','", deptIds)}')
                                and activitydate > '{from:yyyy-MM-dd}' and activitydate <'{to:yyyy-MM-dd}'";
            int count = GetValueBySql(sql);
            return count;
        }


        /// <summary>
        /// 班前班后会
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int BZ_BQBHH(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $@" select count(1) total from wg_workmeeting where isover = 1
                                and groupid in('{string.Join("','", deptIds)}')
                                and meetingstarttime > '{from:yyyy-MM-dd}' and meetingstarttime <'{to:yyyy-MM-dd}'";
            int count = GetValueBySql(sql);
            return count;
        }

        /// <summary>
        /// 安全日活动
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int BZ_AQRHD(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $@" select count(1) total from wg_activity where activitytype = '安全日活动'
                                and state = 'Finish'
                                and groupid in('{string.Join("','", deptIds)}')
                                and starttime > '{from:yyyy-MM-dd}' and starttime <'{to:yyyy-MM-dd}'";
            int count = GetValueBySql(sql);
            return count;
        }


        /// <summary>
        /// 安全学习日
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int BZ_AQXXR(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $@" select count(1) total from wg_edactivity where activitytype = '安全学习日'
                                and state = 'Finish'
                                and groupid in('{string.Join("','", deptIds)}')
                                and starttime > '{from:yyyy-MM-dd}' and starttime <'{to:yyyy-MM-dd}'";
            int count = GetValueBySql(sql);
            return count;
        }


        /// <summary>
        /// 危险预知训练
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int BZ_WXYZXL(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $@"select count(1) total from wg_danger where status = '2' 
                                        and groupid in('{string.Join("','", deptIds)}')
                                        and jobtime > '{from:yyyy-MM-dd}' and jobtime <'{to:yyyy-MM-dd}'";
            int count = GetValueBySql(sql);
            return count;
        }


        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int BZ_WCRW(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $@"select sum(total) total from (select w.groupid,w.meetingid,count(c.jobid) total from wg_workmeeting w 
						left join wg_meetingandjob j
            on w.meetingid = j.endmeetingid
						left join wg_meetingjob c
						on j.jobid = c.jobid
             and c.isfinished = 'finish'
						where w.meetingtype='班后会' and w.meetingstarttime>='{from:yyyy-MM-dd}' and w.meetingstarttime<'{to:yyyy-MM-dd}'
						and w.groupid  in('{string.Join("','", deptIds)}')
          group by w.meetingid) f1";
            int count = GetValueBySql(sql);
            return count;
        }


        /// <summary>
        /// 所有任务
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int BZ_SYRW(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $@"select sum(total) total from (select w.groupid,w.meetingid,count(c.jobid) total from wg_workmeeting w 
						left join wg_meetingandjob j
            on w.meetingid = j.endmeetingid
						left join wg_meetingjob c
						on j.jobid = c.jobid
						where w.meetingtype='班后会' and w.meetingstarttime>='{from:yyyy-MM-dd}' and w.meetingstarttime<'{to:yyyy-MM-dd}'
						and w.groupid  in('{string.Join("','", deptIds)}')
          group by w.meetingid) f1";
            int count = GetValueBySql(sql);
            return count;
        }

        /// <summary>
        /// 政治学习
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int BZ_ZZXX(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $@"select count(1) total from wg_activity where activitytype = '政治学习'
                                        and state = 'Finish'
                                        and groupid  in('{string.Join("','", deptIds)}')
                                        and starttime > '{from:yyyy-MM-dd}' and starttime <'{to:yyyy-MM-dd}'";
            int count = GetValueBySql(sql);
            return count;
        }

        /// <summary>
        /// 劳动保护检查
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int BZ_LDBHJC(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $@"select count(1) total from wg_activity where activitytype = '劳动保护监查'
						and state = 'Finish'
					    and groupid  in('{string.Join("','", deptIds)}')
                        and starttime > '{from:yyyy-MM-dd}' and starttime <'{to:yyyy-MM-dd}'";
            int count = GetValueBySql(sql);
            return count;
        }


        /// <summary>
        /// 班务会
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int BZ_BWH(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $@"select count(1) total from wg_activity where activitytype = '班务会'
						and state = 'Finish'
					    and groupid  in('{string.Join("','", deptIds)}')
                        and starttime > '{from:yyyy-MM-dd}' and starttime <'{to:yyyy-MM-dd}'";
            int count = GetValueBySql(sql);
            return count;
        }


        /// <summary>
        /// 民主管理会
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int BZ_MZGLH(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $@"select count(1) total from wg_activity where activitytype = '民主管理会'
						and state = 'Finish'
					    and groupid  in('{string.Join("','", deptIds)}')
                        and starttime > '{from:yyyy-MM-dd}' and starttime <'{to:yyyy-MM-dd}'";
            int count = GetValueBySql(sql);
            return count;
        }

        /// <summary>
        /// 制度学习
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int BZ_ZDXX(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $@"select count(1) total from wg_activity where activitytype = '制度学习'
						and state = 'Finish'
					    and groupid  in('{string.Join("','", deptIds)}')
                        and starttime > '{from:yyyy-MM-dd}' and starttime <'{to:yyyy-MM-dd}'";
            int count = GetValueBySql(sql);
            return count;
        }


        /// <summary>
        /// 节能记录
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int BZ_JNJL(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $@"select count(1) total from wg_activity where activitytype = '节能记录'
						and state = 'Finish'
					    and groupid  in('{string.Join("','", deptIds)}')
                        and starttime > '{from:yyyy-MM-dd}' and starttime <'{to:yyyy-MM-dd}'";
            int count = GetValueBySql(sql);
            return count;
        }

        /// <summary>
        /// 定点照片
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int BZ_DDZP(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $@"select count(1) total from wg_sevenspicture a join base_fileinfo b
                        on a.id = b.recid 
                        where a.deptid  in ('{string.Join("','", deptIds)}')
                        and planeStartDate > '{from:yyyy-MM-dd}' and planeStartDate <'{to:yyyy-MM-dd}'";
            int count = GetValueBySql(sql);
            return count;
        }


        /// <summary>
        /// 绩效上报班组
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int BZ_JXSBBZ(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $@"select count(1) total from (select DEPARTMENTID,count(id) total from wg_performanceup 
                        where DEPARTMENTID   in ('{string.Join("','", deptIds)}')
                         and usetime > '{from:yyyy-MM-dd}' and usetime <'{to:yyyy-MM-dd}'
                        group by DEPARTMENTID) n1 where n1.total > 0";
            int count = GetValueBySql(sql);
            return count;
        }



        /// <summary>
        /// QC成果
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int BZ_QCCG(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $@"select count(1) total from wg_qcactivity where 
					    deptid in ('{string.Join("','", deptIds)}')
                        and subjecttime > '{from:yyyy-MM-dd}' and subjecttime <'{to:yyyy-MM-dd}'";
            int count = GetValueBySql(sql);
            return count;
        }

        /// <summary>
        /// 管理创新成果
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int BZ_GLCXCG(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $@"select count(1) total from wg_workinnovation where 
					    deptid in ('{string.Join("','", deptIds)}')
                        and reporttime > '{from:yyyy-MM-dd}' and reporttime <'{to:yyyy-MM-dd}'";
            int count = GetValueBySql(sql);
            return count;
        }


        /// <summary>
        /// 7S创新提案
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int BZ_7SCXTA(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $@"select count(1) total from wg_sevensoffice  where 
					    deptid in  ('{string.Join("','", deptIds)}')
                        and createdate > '{from:yyyy-MM-dd}' and createdate <'{to:yyyy-MM-dd}'";
            int count = GetValueBySql(sql);
            return count;
        }

        /// <summary>
        /// 人身风险预控
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int BZ_RSFXYK(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $@"select count(1) total FROM `wg_traininguser` AS `Extent1` INNER JOIN wg_humandangertraining 
                                        AS `Extent2` ON `Extent1`.`TrainingId` = `Extent2`.`TrainingId` 
                                        where  deptid   in  ('{string.Join("','", deptIds)}')
                                        and createtime > '{from:yyyy-MM-dd}' and createtime <'{to:yyyy-MM-dd}' and  TrainingTime > '{from:yyyy-MM-dd}' and TrainingTime <'{to:yyyy-MM-dd}'  and IsDone=1 and IsMarked=1";
            int count = GetValueBySql(sql);
            return count;
        }


        /// <summary>
        /// 完成任务率
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public decimal BZ_RWWCL(List<string> deptIds, DateTime from, DateTime to)
        {
            int wc = BZ_WCRW(deptIds, from, to);
            int all = BZ_SYRW(deptIds, from, to);
            decimal perc = 0;
            if (wc != 0 && all != 0)
            {
                perc = Math.Round(Convert.ToDecimal(wc * 100) / Convert.ToDecimal(all), 2);
            }
            return perc;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public decimal ZD_PXXS(List<string> deptIds, DateTime from, DateTime to)
        {
            var sql = $@"select SUM(AttendNumber*LearnTime)  from WG_EDUBASEINFO
                        where  BZID   in  ('{string.Join("','", deptIds)}')
                        and ActivityDate>= '{from:yyyy-MM-dd}'
                        and ActivityDate < '{to:yyyy-MM-dd}'";
            object obj = BaseRepository().FindObject(sql);
            if (obj is DBNull)
            {
                return 0;
            }
            else
            {
                try
                {
                    return Convert.ToDecimal(obj);
                }
                catch (Exception ex)
                {
                    WriteLog.AddLog($"指标查询出错，查询语句：{sql}，错误信息 {ex.Message}\r\n详细信息：{JsonConvert.SerializeObject(ex)}", "IndexSelectError");
                    throw;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int ZD_WXYZXL(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $"select count(*) from wg_danger where groupid  in  ('{string.Join("','", deptIds)}') and jobtime >= '{from:yyyy-MM-dd}' and jobtime < '{to:yyyy-MM-dd}'  and status = '2'";
            int count = GetValueBySql(sql);
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deptIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int ZD_AQRHD(List<string> deptIds, DateTime from, DateTime to)
        {
            string sql = $"select count(*) from wg_activity where activitytype = '安全日活动' and groupid  in  ('{string.Join("','", deptIds)}') and starttime >= '{from:yyyy-MM-dd}' and starttime <= '{to:yyyy-MM-dd}' and state = 'Finish'";
            int count = GetValueBySql(sql);
            return count;
        }

        /// <summary>
        /// 根据Sql语句取指标
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private int GetValueBySql(string sql)
        {
            object obj = BaseRepository().FindObject(sql);
            if (obj is DBNull)
            {
                return 0;
            }
            else
            {
                try
                {
                    return Convert.ToInt32(obj);
                }
                catch (Exception ex)
                {
                    WriteLog.AddLog($"指标查询出错，查询语句：{sql}，错误信息 {ex.Message}\r\n详细信息：{JsonConvert.SerializeObject(ex)}", "IndexSelectError");
                    throw;
                }
            }
        }

        #endregion

    }
}
