using BSFramework.Entity.WorkMeeting;
using BSFramework.IService.WorkMeeting;
using BSFramework.Service.WorkMeeting;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System;
using BSFramework.Application.Entity.Activity;
using System.Data;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Busines.BaseManage;
using System.Linq;
using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Service.EducationManage;
using BSFramework.Application.IService.EducationManage;
using BSFramework.Application.Entity.EducationManage;
using BSFramework.Util.Extension;
using System.Text;
using BSFramework.Util;

namespace BSFramework.Application.Busines.EducationManage
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class EdActivityBLL
    {
        private EdActivityIService service = new EdActivityService();

        #region ��ȡ����

        public List<ActivityCategoryEntity> GetIndex(string userid, string deptId, string name)
        {
            return service.GetIndex(userid, deptId, name);
        }
        public ActivityCategoryEntity GetCategory(string keyvalue)
        {
            return service.GetCategory(keyvalue);
        }
        public int GetIndex(string deptId, DateTime? now1, DateTime? now2, string name)
        {
            return service.GetIndex(deptId, now1, now2, name);
        }
        /// <summary>
        /// ��ȡ�������
        /// </summary>
        /// <param name="deptid"></param>
        /// <returns></returns>
        public List<ActivityCategoryEntity> GetCategoryList()
        {
            return service.GetCategoryList();
        }


        /// <summary>
        /// ��ȡʵ��
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns></returns>
        public EdActivityEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }
        /// <summary>
        /// ��ȡʵ��
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns></returns>
        public List<EdActivityEntity> GetActivityList(string deptid, string StrTime)
        {
            return service.GetActivityList(deptid, StrTime);
        }

        public List<ActivityEvaluateEntity> GetEntityList()
        {
            return service.GetEntityList();
        }

        public ActivityCategoryEntity AddCategory(ActivityCategoryEntity model)
        {
            return service.AddCategory(model);
        }

        public void IsEvaluate(string id)
        {
            throw new NotImplementedException();
        }

        public bool IsEvaluate(string id, string userid)
        {
            return service.IsEvaluate(id, userid);
        }

        /// <summary>
        /// ��ȡ�����б�
        /// </summary>
        /// <param name="page">����ҳ����</param>
        /// <param name="pagesize">ÿҳ��¼��</param>
        /// <param name="total">��¼����</param>
        /// <param name="status">״̬��0��δ��չ��1���ѽ�����2�����У�</param>
        /// <param name="subject">�����</param>
        /// <returns></returns>
        public System.Collections.IList GetList(int page, int pagesize, out int total, int status, string subject, string bzDepart)
        {
            return service.GetList(page, pagesize, out total, status, subject, bzDepart);
        }
        #endregion

        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="keyValue">����</param>
        public void RemoveForm(string keyValue)
        {
            try
            {
                service.RemoveForm(keyValue);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveActivityPerson(EdActivityPersonEntity entity)
        {
            service.SaveActivityPerson(entity);
        }
        public int GetActivityList(string deptid, string from, string to)
        {
            return service.GetActivityList(deptid, from, to);
        }
        public IEnumerable<EdActivityEntity> GetList(string deptid, DateTime? from, DateTime? to, string name, int page, int pagesize, string category, out int total)
        {
            return service.GetList(deptid, from, to, name, page, pagesize, category, out total);
        }

        public void Over(EdActivityEntity model)
        {
            model.State = "Finish";
            service.Over(model);
        }

        /// <summary>
        /// ��������������޸ģ�
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <param name="entity">ʵ�����</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, EdActivityEntity entity)
        {
            try
            {
                service.SaveForm(keyValue, entity);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<ActivityCategoryEntity> GetActivityCategories(string deptid)
        {
            return service.GetActivityCategories(deptid);
        }

        public void SaveFormSafetyday(string keyValue, EdActivityEntity entity, List<SafetydayEntity> safetyday, string userId)
        {
            try
            {
                service.SaveFormSafetyday(keyValue, entity, safetyday, userId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public EdActivityEntity GetActivities(string category, string deptid)
        {
            return service.GetActivities(category, deptid);
        }

        public List<EdActivityEntity> GetList(int status, string bzDepart, DateTime? fromtime, DateTime? to)
        {
            return service.GetList(status, bzDepart, fromtime, to);
        }
        /// <summary>
        /// �б��ҳ
        /// </summary>
        /// <param name="pagination">��ҳ</param>
        /// <param name="queryJson">��ѯ����</param>
        /// <returns></returns>
        public DataTable GetPageList(Pagination pagination, string queryJson)
        {
            return service.GetPageList(pagination, queryJson);
        }
        public DataTable GetPagesList(Pagination pagination, string queryJson, string type, string select, string deptid, string category, string isEvaluate, string userid) {

            return service.GetPagesList(pagination, queryJson,type,select,deptid,category, isEvaluate, userid);
        }

        public dynamic GetData(string deptId)
        {
            return null;
        }

        public void Start(EdActivityEntity model)
        {
            model.State = "Study";
            service.Start(model);
        }

        public void Ready(EdActivityEntity model)
        {
            model.State = "Study";
            service.Ready(model);
        }

        public void DeleteCategory(string categoryid)
        {
            service.DeleteCategory(categoryid);
        }

        public List<ActivityEvaluateEntity> GetActivityEvaluateEntity(List<string> list)
        {
            return service.GetActivityEvaluateEntity(list);
        }

        /// <summary>
        /// ����ƽ̨�޸�����
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public void UpdateActivityCategoryType(ActivityCategoryEntity category)
        {
            service.UpdateActivityCategoryType(category);
        }
        /// <summary>
        /// ����ƽ̨ɾ������
        /// </summary>
        /// <param name="categoryid"></param>
        /// <returns></returns>
        public void DeleteCategoryType(string categoryid)
        {
            service.DeleteCategoryType(categoryid);
        }
        /// <summary>
        /// ����ƽ̨��������
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void AddCategoryType(ActivityCategoryEntity model)
        {
            service.AddCategoryType(model);
        }
        public ActivityCategoryEntity UpdateActivityCategory(ActivityCategoryEntity category)
        {
            return service.UpdateActivityCategory(category);
        }

        public string PostActivity(EdActivityEntity activity)
        {
            activity.State = "Study";
            service.Start(activity);
            return activity.ActivityId;
        }

        public ActivityCategoryEntity GetCategoryEntity(string keyValue)
        {
            return service.GetCategoryEntity(keyValue);
        }
        public int GetIndexEntity(string keyValue)
        {
            return service.GetIndexEntity(keyValue);
        }
        /// <summary>
        /// ��������������޸ģ�
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <param name="entity">ʵ�����</param>
        /// <returns></returns>
        public void SaveFormCategory(string keyValue, ActivityCategoryEntity entity)
        {
            try
            {
                service.SaveFormCategory(keyValue, entity);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        ///�޸�
        /// </summary>
        /// <param name="entity"></param>
        public void modfiyEntity(EdActivityEntity entity)
        {
            try
            {
                service.modfiyEntity(entity);
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public string[] GetLastest(string deptid)
        {
            return service.GetLastest(deptid);
        }

        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="keyValue">����</param>
        public void Del(string keyValue)
        {
            try
            {
                service.Del(keyValue);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<EdActivityEntity> GetActivities2(string userid, DateTime from, DateTime to, string category, string deptid, bool isall)
        {
            return service.GetActivities2(userid, from, to, category, deptid, isall);
        }

        public EdActivityEntity GetDetail(string p)
        {
            return service.GetDetail(p);
        }

        public void Prepare(EdActivityEntity activityEntity)
        {
            activityEntity.State = "Ready";
            service.Edit(activityEntity);
        }

        public void Edit(EdActivityEntity activityEntity)
        {
            service.Edit(activityEntity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        public void SaveEvaluate(string keyValue, string category, ActivityEvaluateEntity entity)
        {
            var user = OperatorProvider.Provider.Current();
            try
            {
                if (user != null)  //�ƶ������ۣ�user=null
                {
                    entity.DeptName = user.DeptName;
                }
                service.SaveEvaluate(keyValue, entity);
                NextTodo(category, keyValue);
                var messagebll = new MessageBLL();
                switch (category)
                {
                    case "��ǰ����":
                        messagebll.SendMessage("���۰�ǰ����", entity.ActivityEvaluateId);
                        break;
                    case "Σ��Ԥ֪ѵ��":
                        messagebll.SendMessage("����Σ��Ԥ֪ѵ��", entity.ActivityEvaluateId);
                        break;
                    case "����":
                        messagebll.SendMessage("���۰���", entity.ActivityEvaluateId);
                        break;
                    case "�������Ԥ��":
                        messagebll.SendMessage("�������Ԥ������", entity.ActivityEvaluateId);
                        //var dangerbll = new DangerBLL();
                        //var danger = dangerbll.GetTrainingDetail(keyValue);
                        //users = userbll.GetDeptUsers(danger.GroupId);
                        //messagebll.SendMessage("���۰�ǰ����", null, string.Join(",", users.Select(x => x.UserId)), "��������", meeting.MeetingStartTime.ToString("yyyy-MM-dd"), keyValue);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// ��ȡʵ��
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns></returns>
        public List<ActivityEvaluateEntity> GetActivityEvaluateEntity(string keyValue, int pagesize, int page, out int total)
        {
            return service.GetActivityEvaluateEntity(keyValue, pagesize, page, out total);
        }

        public List<StatisticsNumModel> GetActivityStatisticsEntity(string keyValue, int pagesize, int page, string date, out int total)
        {
            return service.GetActivityStatisticsEntity(keyValue, pagesize, page, date, out total);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        /// <param name="measures"></param>
        public void Update(string keyValue, EdActivityEntity entity)
        {
            service.Update(keyValue, entity);
        }
        /// <summary>
        /// ��̨����
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        /// <param name="measures"></param>
        public void ManagerUpdate(string keyValue, EdActivityEntity entity)
        {
            service.ManagerUpdate(keyValue, entity);
        }

        public List<EdActivitySupplyEntity> GetSupplyById()
        {
            return service.GetSupplyById();
        }
        public List<SupplyPeopleEntity> GetPeopleById()
        {
            return service.GetPeopleById();
        }
        public void SaveActivitySupply(string keyValue, EdActivitySupplyEntity entity)
        {
            service.SaveActivitySupply(keyValue, entity);
        }
        public void SaveSupplyPeople(string keyValue, SupplyPeopleEntity entity)
        {
            service.SaveSupplyPeople(keyValue, entity);
        }

        public EdActivitySupplyEntity GetActivitySupplyEntity(string keyValue)
        {
            return service.GetActivitySupplyEntity(keyValue);
        }

        public List<ActivityEvaluateEntity> GetActivityEvaluateEntity(string Activityid)
        {
            return service.GetActivityEvaluateEntity(Activityid);
        }

        #region ��������
        public DataTable GetEvaluateSetData(Pagination pagination, string queryJson)
        {

            return service.GetEvaluateSetData(pagination, queryJson);
        }
        public void SaveEvaluateSet(string keyvalue, EvaluateStepsEntity entity)
        {
            service.SaveEvaluateSet(keyvalue, entity);
        }
        public void deleteEvaluateSet(string keyvalue)
        {
            service.deleteEvaluateSet(keyvalue);
        }

        public EvaluateStepsEntity getEvaluateSet(string keyvalue)
        {
            return service.getEvaluateSet(keyvalue);
        }
        public List<EvaluateStepsEntity> getEvaluateSetBymodule(string module)
        {
            return service.getEvaluateSetBymodule(module);
        }
        //����
        public List<EvaluateTodoEntity> WorkToDo(string userId, string module)
        {
            return service.WorkToDo(userId, module);
        }
        public List<EvaluateTodoEntity> AcWorkToDo(string userId, string activityid)
        {
            return service.AcWorkToDo(userId, activityid);
        }
        //���ɴ���
        public void setToDo(string module, string activityid, string deptid)
        {
            service.setToDo(module, activityid, deptid);
        }
        //�¼�����
        public void NextTodo(string module, string activityid)
        {
            service.NextTodo(module, activityid);
        }
        #endregion

        /// <summary>
        /// ��ҳ��̬
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public object GetIndexDay(string userid)
        {
            var user = new UserBLL().GetEntity(userid);
            var bztype = Config.GetValue("TrainingType");
            if (string.IsNullOrEmpty(bztype))
            {
                bztype = "Σ��Ԥ֪ѵ��";
            }
            var nowTime = DateTime.Now;
            int TimeNum = BSFramework.Util.Time.GetWeekNumberOfDay(nowTime);
            var Start = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 0, 0, 0);
            var End = Start.AddDays(1).AddMinutes(-1);
            DepartmentBLL deptbll = new DepartmentBLL();
            var deptId = string.Empty;
            var dept = deptbll.GetEntity(user.DepartmentId);
            deptId = user.UserId == "System" || dept.IsSpecial ? deptbll.GetRootDepartment().DepartmentId : user.DepartmentId;
            var DeptData = deptbll.GetSubDepartments(deptId, "����").Select(x => new { deptId = x.DepartmentId, deptName = x.FullName, Encode = x.EnCode }).ToList();
            var deptStr = "\"" + string.Join("\",\"", DeptData.Select(x => x.deptId)) + "\"";
            StringBuilder tableStr = new StringBuilder();
            tableStr.Append(" select * from ( ");
            tableStr.Append("select * from ( select a.groupid as deptid, b.fullname as deptname,a.`subject` as title ,a.activitytype as worktype,a.starttime as worktime from  wg_edactivity a LEFT JOIN  base_department b ");
            tableStr.Append("on a.groupid=b.DEPARTMENTID where a.activitytype=\"��ȫѧϰ��\"   and a.groupid in ({0}) order by a.starttime desc  limit 1) a");
            tableStr.Append(" union all ");
            tableStr.Append(" select * from ( select a.groupid as deptid,b.fullname as deptname,\"\" as title,a.meetingtype as worktype,a.meetingstarttime as worktime from wg_workmeeting a LEFT JOIN  base_department b ");
            tableStr.Append(" on a.groupid=b.DEPARTMENTID  where   a.groupid in ({0})   order by a.meetingstarttime desc  limit 1 ) a ");
            tableStr.Append(" union all ");
            tableStr.Append(" select * from ( select  a.groupid as deptid,a.groupname as deptname ,a.jobname as title ,\"" + bztype + "\" as worktype,a.jobtime as worktime from wg_danger a ");
            tableStr.Append(" where   a.groupid in ({0})   order by a.jobtime desc limit 1 ) a ");
            tableStr.Append(" union all ");
            tableStr.Append(" select * from ( select a.bzid as deptid, a.bzname as deptname,a.theme as title ,a.edutype as worktype,a.activitydate as worktime  from  wg_edubaseinfo a ");
            tableStr.Append(" where  a.bzid in ({0})  order by a.activitydate desc limit 1 ) a");
            tableStr.Append(" )a ORDER BY a.worktime desc");
            var List = service.GetDataTable(string.Format(tableStr.ToString(), deptStr, Start.ToString(), End.ToString()));
            var result = new List<object>();
            foreach (DataRow item in List.Rows)
            {
                var deptname = item[1].ToString();
                var title = item[2].ToString();
                var worktype = item[3].ToString();
                var worktime = item[4].ToDate();
                /// ������ѵ����  1.��������   2.�����ʴ�  3.�¹�Ԥ�� 4.���¹�Ԥ�� 5.�¼����ʴ�  6.���¹�Ԥ��  7.���ʽ���

                switch (worktype)
                {
                    case "1":
                        worktype = "��������";
                        break;
                    case "2":
                        worktype = "�����ʴ�";
                        break;
                    case "3":
                        worktype = "�¹�Ԥ��";
                        break;
                    case "4":
                        worktype = "���¹���ϰ";
                        break;
                    case "5":
                        worktype = "�����ʴ�";
                        break;
                    case "6":
                        worktype = "�¹�Ԥ��";
                        break;
                    case "7":
                        worktype = "���ʽ���";
                        break;
                    default:
                        break;
                }
                result.Add(new { deptname = deptname, title = title, worktype = worktype, worktime = worktime.ToString("MM/dd HH:mm") });
            }

            return result;
        }


        /// <summary>
        /// ��ȫѧϰ��ͳ�ƽӿ� ÿ�� ÿ��
        /// </summary>
        /// <param name="Type">0Ϊÿ�ܣ�1Ϊÿ��</param>
        /// <returns></returns>
        public object GetAqxxrTotal(string Type, string userid)
        {
            var user = new UserBLL().GetEntity(userid);
            var nowTime = DateTime.Now;
            int TimeNum = BSFramework.Util.Time.GetWeekNumberOfDay(nowTime);
            var Start = new DateTime(nowTime.Year, nowTime.Month, 1, 0, 0, 0);
            var End = Start.AddMonths(1).AddMinutes(-1);
            if (Type == "0")
            {
                var StartDay = nowTime.Day - (TimeNum - 1);
                var endDay = StartDay + 7;
                Start = new DateTime(nowTime.Year, nowTime.Month, StartDay, 0, 0, 0);
                End = Start.AddDays(7).AddMinutes(-1);
            }
            DepartmentBLL deptbll = new DepartmentBLL();
            var deptId = string.Empty;
            var dept = deptbll.GetEntity(user.DepartmentId);
            deptId = user.UserId == "System" || dept.IsSpecial ? deptbll.GetRootDepartment().DepartmentId : user.DepartmentId;
            var DeptData = deptbll.GetSubDepartments(deptId, "����").Select(x => new { deptId = x.DepartmentId, deptName = x.FullName, Encode = x.EnCode }).ToList();
            var deptStr = "\"" + string.Join("\",\"", DeptData.Select(x => x.deptId)) + "\"";
            var List = service.GetListBySql(string.Format(" and GroupId in ({0}) and starttime  between \"{1}\" and \"{2}\" and activitytype=\"{3}\" and state=\"Finish\" ", deptStr, Start.ToString(), End.ToString(), "��ȫѧϰ��")).ToList();
            var GroupList = List.GroupBy(x => x.GroupId);
            var result = new List<AqxxrModel>();
            var resultNull = new List<AqxxrModel>();
            int total = 0;
            foreach (var item in DeptData)
            {
                var deptList = GroupList.FirstOrDefault(x => x.Key == item.deptId);

                if (deptList != null && deptList.Count() > 0)
                {
                    if (Type == "0")
                    {
                        string week = string.Empty;
                        int minute = 0;
                        List<int> weekDays = new List<int>();
                        foreach (EdActivityEntity DeptItem in deptList)
                        {
                            TimeSpan tsDiffer = DeptItem.EndTime - DeptItem.StartTime;
                            var nowMinute = tsDiffer.TotalMinutes.ToInt();
                            if (nowMinute > 240)
                            {
                                minute += 240;
                            }
                            else
                            {
                                minute += nowMinute;

                            }
                            var weekday = Util.Time.GetWeekNumberOfDay(DeptItem.StartTime);
                            if (!weekDays.Contains(weekday))
                            {
                                switch (weekday)
                                {
                                    case 1:
                                        week += "��һ��";
                                        break;
                                    case 2:
                                        week += "�ܶ���";
                                        break;
                                    case 3:
                                        week += "������";
                                        break;
                                    case 4:
                                        week += "���ġ�";
                                        break;
                                    case 5:
                                        week += "���塢";
                                        break;
                                    case 6:
                                        week += "������";
                                        break;
                                    case 7:
                                        week += "���ա�";
                                        break;
                                }
                                weekDays.Add(weekday);
                            }

                        }
                        week = week.Substring(0, (weekDays.Count * 3) - 1);
                        var hour = (int)Math.Floor((decimal)minute / 60);
                        minute = minute % 60;
                        var time = string.Empty;
                        if (hour != 0)
                        {
                            time += hour + "ʱ";
                        }
                        time += minute + "��";
                        result.Add(new AqxxrModel() { dept = item.deptName, week = week, time = time, total = minute });
                    }
                    else
                    {

                        total = deptList.Count();

                        result.Add(new AqxxrModel() { dept = item.deptName, total = total });

                    }

                }
                else
                {
                    if (Type == "0")
                    {
                        resultNull.Add(new AqxxrModel() { dept = item.deptName, week = "δѧϰ", time = "0��" });

                    }
                    else
                    {
                        resultNull.Add(new AqxxrModel() { dept = item.deptName, total = 0 });

                    }
                }



            }
            result = result.OrderByDescending(x => x.total).ToList();
            result.AddRange(resultNull);

            return result;
        }
        public class AqxxrModel
        {
            public string dept { get; set; }
            public string week { get; set; }
            public string time { get; set; }
            public int total { get; set; }


        }
        /// <summary>
        /// ��ȡĳ���鵱ǰ�°�ȫѧϰ�ջ�Ĵ���
        /// </summary>
        /// <param name="deptId">����ID</param>
        /// <returns></returns>
        public int GetMonthCount(string deptId)
        {
            return service.GetMonthCount(deptId);
        }
    }
}
