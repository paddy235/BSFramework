using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Application.IService.WorkMeeting;
using BSFramework.Application.Service.SafetyScore;
using BSFramework.Data.Repository;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.WorkMeeting
{
    public class MeetingRecordService : RepositoryFactory<MeetingRecordEntity>, IMeetingRecordService
    {
        /// <summary>
        /// 根据班会的Id，查询班会记录 
        /// （如果有多个默认查第一个）
        /// </summary>
        /// <param name="meetingId">班会的Id</param>
        /// <returns></returns>
        public MeetingRecordEntity GetEntityByMeetingId(string meetingId)
        {
           return  BaseRepository().FindEntity(p => p.MeetingId == meetingId);
        }


        /// <summary>
        /// 早安中铝统计
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="deptCode">部门编码</param>
        /// <returns>list<{ DeptName = "部门名称",     MeetingCount ="班会次数",   RecordCount = "早安中铝学习次数",   Point = "学习率"  }></returns>
        public object GetStatistics(string startTime, string endTime, string deptCode)
        {
            var db = new RepositoryFactory().BaseRepository();
            var meetingQuery = from q1 in db.IQueryable<DepartmentEntity>()
                               join q2 in db.IQueryable<WorkmeetingEntity>() on q1.DepartmentId equals q2.GroupId into t
                               from t1 in t.DefaultIfEmpty()
                               where t1.MeetingType == "班前会"
                               select new { q1, t1 };
            var recordQuery = db.IQueryable<MeetingQuestionEntity>();


            //string MeetingCountSql = @"select COUNT(*) AS COUNT , DEPARTMENTID,FULLNAME from base_department A LEFT JOIN wg_workmeeting B ON A.DEPARTMENTID=B.GROUPID WHERE B.meetingtype='班前会'";
            //string recordCountSql = "	 SELECT COUNT(*) AS COUNT,CREATEUSERDEPTID,CREATEUSERDEPTNAME,CREATEUSERDEPTCODE FROM wg_meetingquestion   where 1=1";
            if (!string.IsNullOrWhiteSpace(startTime))
            {
                DateTime starttime;
                if (DateTime.TryParse(startTime, out starttime))
                {
                    //MeetingCountSql += $" AND  meetingstarttime>='{starttime.ToString("yyyy-MM-dd")}'";
                    meetingQuery = meetingQuery.Where(x => x.t1.MeetingStartTime >= starttime);
                    //recordCountSql += $" AND CREATEDATE >='{starttime.ToString("yyyy-MM-dd")}'";
                    recordQuery = recordQuery.Where(x => x.CreateDate >= starttime);
                }
            }
            if (!string.IsNullOrWhiteSpace(endTime))
            {
                DateTime endtime;
                if (DateTime.TryParse(endTime, out endtime))
                {
                    endtime = endtime.AddDays(1);
                    //MeetingCountSql += $" AND  meetingendtime<'{endtime.AddDays(1).ToString("yyyy-MM-dd")}'";
                    meetingQuery = meetingQuery.Where(x => x.t1.MeetingEndTime < endtime);
                    //recordCountSql += $" AND CREATEDATE <'{endtime.AddDays(1).ToString("yyyy-MM-dd")}'";
                    recordQuery = recordQuery.Where(x => x.CreateDate < endtime);
                }
            }
            IQueryable<DepartmentEntity> deptQuery = db.IQueryable<DepartmentEntity>(p => p.Nature == "班组");
            if (!string.IsNullOrWhiteSpace(deptCode))
            {
                //MeetingCountSql += $" AND  A.encode like '{deptCode}%' ";
                meetingQuery = meetingQuery.Where(x => x.q1.EnCode.StartsWith(deptCode));
                //recordCountSql += $" AND CREATEUSERDEPTCODE LIKE '{deptCode}%'";
                recordQuery = recordQuery.Where(x => x.CreateUserDeptCode.StartsWith(deptCode));
                deptQuery = deptQuery.Where(p => p.EnCode.StartsWith(deptCode));
            }
            //MeetingCountSql += " GROUP BY DEPARTMENTID,FULLNAME";
            //recordCountSql += " GROUP BY CREATEUSERDEPTID";
            var meetingQueryData = meetingQuery.GroupBy(x => new { x.q1.DepartmentId, x.q1.FullName }).Select(x => new { COUNT = x.Count(), x.Key.DepartmentId, x.Key.FullName }).ToList();
            var recordQueryData = recordQuery.GroupBy(x => new { x.CreateUserDeptId, x.CreateUserDeptCode, x.CreateUserDeptName }).Select(x => new { COUNT = x.Count(), x.Key.CreateUserDeptName, x.Key.CreateUserDeptId, x.Key.CreateUserDeptCode }).ToList();

            //DataTable dtMeeting = db.FindTable(MeetingCountSql);
            //DataTable dtRecord = db.FindTable(recordCountSql);
            List<DepartmentEntity> deptList = deptQuery.ToList();
            if (deptList != null && deptList.Count > 0)
            {
                List<object> list = new List<object>();
                deptList.ForEach(p => {
                    decimal meetingCount = 0, recordCount = 0, point = 0; ;
                    var drsMeeting = meetingQueryData.Where(x => x.DepartmentId == p.DepartmentId).ToList();
                    var drsRecord = recordQueryData.Where(x => x.CreateUserDeptId == p.DepartmentId).ToList();
                    if (drsMeeting.Count > 0) meetingCount = Convert.ToDecimal(drsMeeting.FirstOrDefault().COUNT);
                    if (drsRecord.Count > 0) recordCount = Convert.ToDecimal(drsRecord.FirstOrDefault().COUNT);
                    if (meetingCount > 0 && recordCount > 0)
                    {
                        point = Math.Round(recordCount / meetingCount * 100, 2);
                    }
                    list.Add(new
                    {
                        DeptName = p.FullName,
                        MeetingCount = meetingCount,
                        RecordCount = recordCount,
                        Point = point
                    });
                });
                return list;
            }
            return null;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="data"></param>
        public void Insert(MeetingRecordEntity data)
        {
            BaseRepository().Insert(data);
            Task.Run(() =>
            {
                    //添加添加安全积分
                    SafetyScoreService scoreService = new SafetyScoreService();
                    List<string> userIds = data.ShareUserIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (userIds !=null && userIds.Count>0)
                    {
                        userIds.ForEach(x => {
                            try
                            {
                                scoreService.AddScore(x, 5);
                            }
                            catch (Exception)
                            {

                            }
                        });
                    }
            });
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="data"></param>
        public void Update(MeetingRecordEntity data)
        {
            BaseRepository().Update(data);
        }
    }
}
