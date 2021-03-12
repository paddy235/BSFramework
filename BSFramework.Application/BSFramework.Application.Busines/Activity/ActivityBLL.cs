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
using BSFramework.Util;
using BSFramework.Application.Service.EducationManage;
using BSFramework.Application.IService.EducationManage;
using BSFramework.Application.Entity.PublicInfoManage.ViewMode;

namespace BSFramework.Busines.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public class ActivityBLL
    {
        private ActivityIService service;
        private IEduBaseInfoService _eduBaseInfoService;

        public ActivityBLL()
        {
            service = new ActivityService();
            _eduBaseInfoService = new EduBaseInfoService();
        }

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

        public void EditActivity(ActivityEntity activity)
        {
            service.EditActivity(activity);
        }

        public void EndActivity(string id)
        {
            var activity = service.GetDetail(id);
            service.EndActivity(activity);
            if (activity.SubActivities.Count(x => x.ActivitySubject == "��������") > 0)
            {
                var subject = activity.SubActivities.Find(x => x.ActivitySubject == "��������");
                _eduBaseInfoService.End(subject.SubActivityId);
            }
            if (activity.SubActivities.Count(x => x.ActivitySubject == "�¹�Ԥ��") > 0)
            {
                var subject = activity.SubActivities.Find(x => x.ActivitySubject == "�¹�Ԥ��");
                _eduBaseInfoService.End(subject.SubActivityId);
            }
            if (activity.SubActivities.Count(x => x.ActivitySubject == "���¹���ϰ") > 0)
            {
                var subject = activity.SubActivities.Find(x => x.ActivitySubject == "���¹���ϰ");
                _eduBaseInfoService.End(subject.SubActivityId);
            }
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
        public ActivityEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }
        /// <summary>
        /// ��ȡʵ��
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns></returns>
        public List<ActivityEntity> GetActivityList(string deptid, string StrTime)
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

        public void SaveActivityPerson(ActivityPersonEntity entity)
        {
            service.SaveActivityPerson(entity);
        }
        public int GetActivityList(string deptid, string from, string to)
        {
            return service.GetActivityList(deptid, from, to);
        }
        public IEnumerable<ActivityEntity> GetList(string deptid, DateTime? from, DateTime? to, string name, int page, int pagesize, string category, out int total)
        {
            return service.GetList(deptid, from, to, name, page, pagesize, category, out total);
        }

        public void Over(ActivityEntity model)
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
        public void SaveForm(string keyValue, ActivityEntity entity)
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

        public void SaveFormSafetyday(string keyValue, ActivityEntity entity, List<SafetydayEntity> safetyday, string userId)
        {
            service.SaveFormSafetyday(keyValue, entity, safetyday, userId);
        }

        public ActivityEntity GetActivities(string category, string deptid)
        {
            return service.GetActivities(category, deptid);
        }

        public List<ActivityEntity> GetList(int status, string bzDepart, DateTime? fromtime, DateTime? to)
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
        /// <summary>
        /// �б��ҳ
        /// </summary>
        /// <param name="pagination">��ҳ</param>
        /// <param name="queryJson">��ѯ����</param>
        /// <returns></returns>
        public DataTable GetPagesList(Pagination pagination, string queryJson, string type, string select, string deptid, string category, string isEvaluate, string userid)
        {
            return service.GetPagesList(pagination, queryJson, type, select, deptid, category, isEvaluate, userid);

        }
        /// <summary>
        /// �б��ҳ
        /// </summary>
        /// <param name="pagination">��ҳ</param>
        /// <param name="queryJson">��ѯ����</param>
        /// <returns></returns>
        public DataTable GetActivityPageList(Pagination pagination, string queryJson, string select, string deptid, string category, string isEvaluate, string userid)
        {
            return service.GetActivityPageList(pagination, queryJson, select, deptid, category, isEvaluate, userid);

        }
        public dynamic GetData(string deptId)
        {
            return null;
        }

        public void Start(ActivityEntity model)
        {
            model.State = "Study";
            service.Start(model);
        }

        public void Ready(ActivityEntity model)
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

        public string PostActivity(ActivityEntity activity)
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
        public void modfiyEntity(ActivityEntity entity)
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


        public List<ActivityEntity> GetActivities2(string userid, DateTime from, DateTime to, string category, string deptid, bool isall, int pageSize, int pageIndex, out int total, string notcategory = null)
        {
            return service.GetActivities2(userid, from, to, category, deptid, isall, pageSize, pageIndex, out total, notcategory);
        }

        public ActivityEntity GetDetail(string p)
        {
            return service.GetDetail(p);
        }

        public void Prepare(ActivityEntity activityEntity)
        {
            activityEntity.State = "Ready";
            service.Edit(activityEntity);
        }

        public void Edit(ActivityEntity activityEntity)
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


        public void SaveEvaluate(List<ActivityEvaluateEntity> entity)
        {
            try
            {
                service.SaveEvaluate(entity);
            }
            catch (Exception)
            {

                throw;
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
        public void Update(string keyValue, ActivityEntity entity)
        {
            service.Update(keyValue, entity);
        }
        /// <summary>
        /// ��̨����
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        /// <param name="measures"></param>
        public void ManagerUpdate(string keyValue, ActivityEntity entity)
        {
            service.ManagerUpdate(keyValue, entity);
        }

        public List<ActivitySupplyEntity> GetSupplyById()
        {
            return service.GetSupplyById();
        }
        public List<SupplyPeopleEntity> GetPeopleById()
        {
            return service.GetPeopleById();
        }
        public void SaveActivitySupply(string keyValue, ActivitySupplyEntity entity)
        {
            service.SaveActivitySupply(keyValue, entity);
        }
        public void SaveSupplyPeople(string keyValue, SupplyPeopleEntity entity)
        {
            service.SaveSupplyPeople(keyValue, entity);
        }

        public ActivitySupplyEntity GetActivitySupplyEntity(string keyValue)
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
        /// <summary>
        /// ��ȡ���ݵ�����ģ��
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public DataTable getIsModuleData(string strSql)
        {
            return service.getIsModuleData(strSql);
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

        #region ͳ��

        public DataTable GetGroupCount(string deptcode, DateTime f, DateTime t, string category)
        {
            return service.GetGroupCount(deptcode, f, t, category);

        }

        /// <summary>
        /// ��ȡ���������͵Ŀ�չ����
        /// </summary>
        /// <param name="from">��ʼʱ��</param>
        /// <param name="to">����ʱ��</param>
        /// <returns></returns>
        public List<KeyValue> ActivityTypeCount(DateTime from, DateTime to, List<string> deptlist)
        {
            return service.ActivityTypeCount(from, to, deptlist);
        }

        #endregion


        #region web �ҳ������
        /// <summary>
        /// �������û�ȡҳ��չʾ
        /// </summary>
        public List<ActivityCategoryEntity> GetActivityMenu(string userId, string deptId, string module)
        {

            //��ȡ˫������
            var columnMenu = new TerminalDataSetBLL().GetMenuConfigList(0);
            List<ActivityCategoryEntity> result = new List<ActivityCategoryEntity>();
            if (columnMenu.Count > 0)
            {
                //var code = module == "����" ? "activity" : "education";
                var frist = columnMenu.FirstOrDefault(x => x.ModuleCode == module);
                if (frist != null)
                {
                    //���õĶ����˵�
                    var second = columnMenu.Where(x => x.ParentId == frist.ModuleId).OrderBy(x => x.Sort);
                    //�����������͵Ĳ�ѯʵ��
                    Dictionary<string, string[]> categoryList = new Dictionary<string, string[]>();

                    #region windows code���ձ�

                    // ������ѵ����  1.��������   2.�����ʴ�  3.�¹�Ԥ�� 4.���¹�Ԥ�� 5.�¼����ʴ�  6.���¹�Ԥ��  7.���ʽ��� 8���ʽ��⣨����ʽ��
                    //������ѵ	education
                    //��ȫ������ѵ	education_edutrain
                    //��������	education_teach
                    //�¹�Ԥ��	education_expect
                    //���¹���ϰ	education_drill
                    //�����ʴ�	education_qaa
                    //���ʽ���  education_qa
                    //��ȫѧϰ��  education_safeday

                    //����	activity
                    //��ȫ�ջ	activity_safeday
                    //����ѧϰ	activity_study
                    //���������	activity_manage
                    //�����	activity_meet
                    //�ϼ���������	activity_superior



                    #endregion
                    #region ��������
                    foreach (var item in second)
                    {
                        if (item.ModuleCode == "education_edutrain")
                        {
                            continue;
                        }
                        switch (item.ModuleCode)
                        {
                            //������ѵ�ķ���
                            //�������� 1 
                            case "education_teach":
                                categoryList.Add(item.ModuleCode, new string[] { "1", item.ModuleName });
                                break;
                            //�����ʴ� 2 
                            case "education_qaa":
                                categoryList.Add(item.ModuleCode, new string[] { "2", item.ModuleName });
                                break;
                            //�¹�Ԥ�� 3 
                            case "education_expect":
                                categoryList.Add(item.ModuleCode, new string[] { "3", item.ModuleName });
                                break;
                            //���¹���ϰ 4 
                            case "education_drill":
                                categoryList.Add(item.ModuleCode, new string[] { "4", item.ModuleName });
                                break;
                            //���ʽ��� 7
                            case "education_qa":
                                categoryList.Add(item.ModuleCode, new string[] { "7", item.ModuleName });
                                break;
                            //��ȫѧϰ�� 
                            case "education_safeday":
                                categoryList.Add("EA", new string[] { item.ModuleName });
                                break;
                            //�����
                            default:
                                categoryList.Add(item.ModuleCode, new string[] { item.ModuleName });
                                break;

                        }
                    }
                    #endregion

                    result = service.GetMenuIndex(module, userId, deptId, categoryList);



                }
            }

            return result;
        }

        public List<ActivityStatiticsEntity> Statistics(string[] depts, string[] categories, DateTime from, DateTime to)
        {
            return service.Statistics(depts, categories, from, to);
        }

        #endregion
    }
}
