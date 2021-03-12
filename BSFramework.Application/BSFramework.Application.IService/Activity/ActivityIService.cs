using BSFramework.Application.Entity.Activity;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System;
using System.Data;
using BSFramework.Application.Entity.PublicInfoManage.ViewMode;

namespace BSFramework.IService.WorkMeeting
{
    /// <summary>
    /// �� �������鰲ȫ�ջ
    /// </summary>
    public interface ActivityIService
    {
        #region ��ȡ����
        /// <summary>
        /// ��ȡ�б�
        /// </summary>
        /// <param name="queryJson">��ѯ����</param>
        /// <returns>�����б�</returns>
        List<ActivityCategoryEntity> GetIndex(string userid, string deptid, string name);
        List<ActivityEntity> GetList(int status, string bzDepart, DateTime? fromtime, DateTime? to);
        /// <summary>
        /// ��ȡ���/
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        ActivityCategoryEntity GetCategory(string keyvalue);
        /// <summary>
        /// ��ȡ�������
        /// </summary>
        /// <param name="deptid"></param>
        /// <returns></returns>
        List<ActivityCategoryEntity> GetCategoryList();
        List<ActivitySupplyEntity> GetSupplyById();
        List<SupplyPeopleEntity> GetPeopleById();
        void EndActivity(ActivityEntity activity);

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        int GetIndexEntity(string keyValue);
        /// <summary>
        /// ��ȡʵ��
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns></returns>
        ActivityEntity GetEntity(string keyValue);
        void EditActivity(ActivityEntity activity);
        ActivitySupplyEntity GetActivitySupplyEntity(string keyValue);
        List<ActivityEntity> GetActivityList(string deptid, string StrTime);

        List<ActivityEvaluateEntity> GetEntityList();
        /// <summary>
        /// ��ȡʵ��
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns></returns>
        ActivityCategoryEntity GetCategoryEntity(string keyValue);
        /// <summary>
        /// ̨��
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        int GetIndex(string deptId, DateTime? from, DateTime? to, string name);
        ActivityCategoryEntity AddCategory(ActivityCategoryEntity model);
        int Count(string[] depts, string category, DateTime start, DateTime end);

        /// <summary>
        /// ����ƽ̨�޸�����
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        void UpdateActivityCategoryType(ActivityCategoryEntity category);
        /// <summary>
        /// ����ƽ̨ɾ������
        /// </summary>
        /// <param name="categoryid"></param>
        /// <returns></returns>
        void DeleteCategoryType(string categoryid);
        /// <summary>
        /// ����ƽ̨��������
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        void AddCategoryType(ActivityCategoryEntity model);
        bool IsEvaluate(string id, string userid);

        /// <summary>
        /// ��ȡ�����б�
        /// </summary>
        /// <param name="page">����ҳ����</param>
        /// <param name="pagesize">ÿҳ��¼��</param>
        /// <param name="total">��¼����</param>
        /// <param name="status">״̬��0��δ��չ��1���ѽ�����2�����У�</param>
        /// <param name="subject">�����</param>
        /// <returns></returns>
        System.Collections.IList GetList(int page, int pagesize, out int total, int status, string subject, string bzDepart);

        /// <summary>
        /// �б��ҳ
        /// </summary>
        /// <param name="pagination">��ҳ</param>
        /// <param name="queryJson">��ѯ����</param>
        /// <returns></returns>
        DataTable GetPageList(Pagination pagination, string queryJson);

        /// <summary>
        /// �б��ҳ
        /// </summary>
        /// <param name="pagination">��ҳ</param>
        /// <param name="queryJson">��ѯ����</param>
        /// <returns></returns>
        DataTable GetPagesList(Pagination pagination, string queryJson, string type, string select, string deptid, string category, string isEvaluate, string userid);

        /// <summary>
        /// �б��ҳ
        /// </summary>
        /// <param name="pagination">��ҳ</param>
        /// <param name="queryJson">��ѯ����</param>
        /// <returns></returns>
        DataTable GetActivityPageList(Pagination pagination, string queryJson, string select, string deptid, string category, string isEvaluate, string userid);
        #endregion

        #region �ύ����
        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="keyValue">����</param>
        void RemoveForm(string keyValue);
        void SaveActivitySupply(string keyValue, ActivitySupplyEntity entity);
        void SaveSupplyPeople(string keyValue, SupplyPeopleEntity entity);

        /// <summary>
        ///�޸�
        /// </summary>
        /// <param name="entity"></param>
        void modfiyEntity(ActivityEntity entity);
        void Del(string keyValue);
        /// <summary>
        /// ��������������޸ģ�
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <param name="entity">ʵ�����</param>
        /// <returns></returns>
        void SaveForm(string keyValue, ActivityEntity entity);

        void SaveFormSafetyday(string keyValue, ActivityEntity entity, List<SafetydayEntity> safetyday, string userId);
        /// <summary>
        /// ��������������޸ģ�
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <param name="entity">ʵ�����</param>
        /// <returns></returns>
        void SaveFormCategory(string keyValue, ActivityCategoryEntity entity);
        List<ActivityCategoryEntity> GetActivityCategories(string deptid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        IEnumerable<ActivityEntity> GetList(string deptid, DateTime? from, DateTime? to, string name, int page, int pagesize, string category, out int total);
        IEnumerable<ActivityEntity> GetListApp(string deptid, DateTime? from, DateTime? to, string name, int page, int pagesize, string category, out int total);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityId"></param>
        void Over(ActivityEntity model);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        ActivityEntity GetActivities(string category, string deptid);
        void Start(ActivityEntity entity);
        void Ready(ActivityEntity model);
        void Study(ActivityEntity model);

        ActivityEntity GetDetail(string id);
        void DeleteCategory(string categoryid);
        ActivityCategoryEntity UpdateActivityCategory(ActivityCategoryEntity category);
        string PostActivity(ActivityCategoryEntity activity);
        List<ActivityEvaluateEntity> GetActivityEvaluateEntity(List<string> list);
        #endregion
        /// <summary>
        /// ��ȡ����
        /// </summary>
        ///<param name="pagination">��ҳ������</param>
        /// <param name="queryJson">startTime��ʼʱ��|endTime����ʱ��|deptId����id|userId�û�id|State״̬|Category����|haveEvaluate�Ƿ��ѯ����|Depts����id����</param>
        /// <returns></returns>
        List<ActivityEntity> GetAcJobCount(Pagination pagination, string queryJson);
        List<ActivityEntity> GetActivities2(string userid, DateTime from, DateTime to, string category, string deptid, bool isall, int pageSize, int pageIndex, out int total, string notcategory = null);
        void Edit(ActivityEntity activityEntity);

        void SaveEvaluate(string keyValue, ActivityEvaluateEntity entity);
        void SaveEvaluate(List<ActivityEvaluateEntity> entity);
        List<ActivityEvaluateEntity> GetActivityEvaluateEntity(string keyValue, int pagesize, int page, out int total);
        List<StatisticsNumModel> GetActivityStatisticsEntity(string keyValue, int pagesize, int page, string date, out int total);

        void Update(string keyValue, ActivityEntity entity);

        void ManagerUpdate(string keyValue, ActivityEntity entity);
        int GetActivityList(string deptid, string from, string to);
        void SaveActivityPerson(ActivityPersonEntity entity);
        string[] GetLastest(string deptid);
        List<ActivityEvaluateEntity> GetActivityEvaluateEntity(string Activityid);

        DataTable GetEvaluateSetData(Pagination pagination, string queryJson);

        void SaveEvaluateSet(string keyvalue, EvaluateStepsEntity entity);
        void deleteEvaluateSet(string keyvalue);
        EvaluateStepsEntity getEvaluateSet(string keyvalue);
        /// <summary>
        /// ��ȡ���ݵ�����ģ��
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        DataTable getIsModuleData(string strSql);

        List<EvaluateStepsEntity> getEvaluateSetBymodule(string module);
        //����
        List<EvaluateTodoEntity> WorkToDo(string userId, string module);
        List<EvaluateTodoEntity> AcWorkToDo(string userId, string activityid);
        //���ɴ���
        void setToDo(string module, string activityid, string deptid);
        void NextTodo(string module, string activityid);

        DataTable GetGroupCount(string deptcode, DateTime f, DateTime t, string category);


        #region web �ҳ������

        /// <summary>
        /// ��ȡ���ǰ״̬
        /// </summary>
        /// <param name="mianModule">ѡ���һ��ģ��</param>
        /// <param name="userid"></param>
        /// <param name="deptid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        List<ActivityCategoryEntity> GetMenuIndex(string mianModule, string userid, string deptid, Dictionary<string, string[]> type);
        #endregion
        List<KeyValue> ActivityTypeCount(DateTime from, DateTime to, List<string> deptlist);
        List<ActivityStatiticsEntity> Statistics(string[] depts, string[] categories, DateTime from, DateTime to);
    }
}
