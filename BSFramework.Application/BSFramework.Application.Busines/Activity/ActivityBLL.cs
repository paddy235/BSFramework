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
    /// 描 述：班组安全日活动
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

        #region 获取数据

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
            if (activity.SubActivities.Count(x => x.ActivitySubject == "技术讲课") > 0)
            {
                var subject = activity.SubActivities.Find(x => x.ActivitySubject == "技术讲课");
                _eduBaseInfoService.End(subject.SubActivityId);
            }
            if (activity.SubActivities.Count(x => x.ActivitySubject == "事故预想") > 0)
            {
                var subject = activity.SubActivities.Find(x => x.ActivitySubject == "事故预想");
                _eduBaseInfoService.End(subject.SubActivityId);
            }
            if (activity.SubActivities.Count(x => x.ActivitySubject == "反事故演习") > 0)
            {
                var subject = activity.SubActivities.Find(x => x.ActivitySubject == "反事故演习");
                _eduBaseInfoService.End(subject.SubActivityId);
            }
        }

        /// <summary>
        /// 获取所有类别
        /// </summary>
        /// <param name="deptid"></param>
        /// <returns></returns>
        public List<ActivityCategoryEntity> GetCategoryList()
        {
            return service.GetCategoryList();
        }


        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public ActivityEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
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
        /// 获取班组活动列表
        /// </summary>
        /// <param name="page">请求页索引</param>
        /// <param name="pagesize">每页记录数</param>
        /// <param name="total">记录总数</param>
        /// <param name="status">状态（0：未开展，1：已结束，2：所有）</param>
        /// <param name="subject">活动主题</param>
        /// <returns></returns>
        public System.Collections.IList GetList(int page, int pagesize, out int total, int status, string subject, string bzDepart)
        {
            return service.GetList(page, pagesize, out total, status, subject, bzDepart);
        }
        #endregion

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
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
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
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
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public DataTable GetPageList(Pagination pagination, string queryJson)
        {
            return service.GetPageList(pagination, queryJson);
        }
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public DataTable GetPagesList(Pagination pagination, string queryJson, string type, string select, string deptid, string category, string isEvaluate, string userid)
        {
            return service.GetPagesList(pagination, queryJson, type, select, deptid, category, isEvaluate, userid);

        }
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
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
        /// 管理平台修改类型
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public void UpdateActivityCategoryType(ActivityCategoryEntity category)
        {
            service.UpdateActivityCategoryType(category);
        }
        /// <summary>
        /// 管理平台删除类型
        /// </summary>
        /// <param name="categoryid"></param>
        /// <returns></returns>
        public void DeleteCategoryType(string categoryid)
        {
            service.DeleteCategoryType(categoryid);
        }
        /// <summary>
        /// 管理平台新增类型
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
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
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
        ///修改
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
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
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
                if (user != null)  //移动端评价，user=null
                {
                    entity.DeptName = user.DeptName;
                }
                service.SaveEvaluate(keyValue, entity);
                NextTodo(category, keyValue);
                var messagebll = new MessageBLL();
                switch (category)
                {
                    case "班前班后会":
                        messagebll.SendMessage("评价班前班后会", entity.ActivityEvaluateId);
                        break;
                    case "危险预知训练":
                        messagebll.SendMessage("评价危险预知训练", entity.ActivityEvaluateId);
                        break;
                    case "班组活动":
                        messagebll.SendMessage("评价班组活动", entity.ActivityEvaluateId);
                        break;
                    case "人身风险预控":
                        messagebll.SendMessage("人身风险预控评价", entity.ActivityEvaluateId);
                        //var dangerbll = new DangerBLL();
                        //var danger = dangerbll.GetTrainingDetail(keyValue);
                        //users = userbll.GetDeptUsers(danger.GroupId);
                        //messagebll.SendMessage("评价班前班后会", null, string.Join(",", users.Select(x => x.UserId)), "工作评价", meeting.MeetingStartTime.ToString("yyyy-MM-dd"), keyValue);
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
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
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
        /// 后台管理
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

        #region 评价设置
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
        /// 获取数据的所属模块
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
        //待办
        public List<EvaluateTodoEntity> WorkToDo(string userId, string module)
        {
            return service.WorkToDo(userId, module);
        }
        public List<EvaluateTodoEntity> AcWorkToDo(string userId, string activityid)
        {
            return service.AcWorkToDo(userId, activityid);
        }
        //生成待办
        public void setToDo(string module, string activityid, string deptid)
        {
            service.setToDo(module, activityid, deptid);
        }
        //下级待办
        public void NextTodo(string module, string activityid)
        {
            service.NextTodo(module, activityid);
        }
        #endregion

        #region 统计

        public DataTable GetGroupCount(string deptcode, DateTime f, DateTime t, string category)
        {
            return service.GetGroupCount(deptcode, f, t, category);

        }

        /// <summary>
        /// 获取班组活动各类型的开展次数
        /// </summary>
        /// <param name="from">开始时间</param>
        /// <param name="to">结束时间</param>
        /// <returns></returns>
        public List<KeyValue> ActivityTypeCount(DateTime from, DateTime to, List<string> deptlist)
        {
            return service.ActivityTypeCount(from, to, deptlist);
        }

        #endregion


        #region web 活动页面配置
        /// <summary>
        /// 根据配置获取页面展示
        /// </summary>
        public List<ActivityCategoryEntity> GetActivityMenu(string userId, string deptId, string module)
        {

            //获取双控数据
            var columnMenu = new TerminalDataSetBLL().GetMenuConfigList(0);
            List<ActivityCategoryEntity> result = new List<ActivityCategoryEntity>();
            if (columnMenu.Count > 0)
            {
                //var code = module == "班组活动" ? "activity" : "education";
                var frist = columnMenu.FirstOrDefault(x => x.ModuleCode == module);
                if (frist != null)
                {
                    //配置的二级菜单
                    var second = columnMenu.Where(x => x.ParentId == frist.ModuleId).OrderBy(x => x.Sort);
                    //创建基础类型的查询实体
                    Dictionary<string, string[]> categoryList = new Dictionary<string, string[]>();

                    #region windows code对照表

                    // 教育培训类型  1.技术讲课   2.技术问答  3.事故预想 4.反事故预想 5.新技术问答  6.新事故预想  7.拷问讲解 8考问讲解（集中式）
                    //教育培训	education
                    //安全技术培训	education_edutrain
                    //技术讲课	education_teach
                    //事故预想	education_expect
                    //反事故演习	education_drill
                    //技术问答	education_qaa
                    //考问讲解  education_qa
                    //安全学习日  education_safeday

                    //班组活动	activity
                    //安全日活动	activity_safeday
                    //政治学习	activity_study
                    //民主管理会	activity_manage
                    //班务会	activity_meet
                    //上级精神宣贯	activity_superior



                    #endregion
                    #region 对照类型
                    foreach (var item in second)
                    {
                        if (item.ModuleCode == "education_edutrain")
                        {
                            continue;
                        }
                        switch (item.ModuleCode)
                        {
                            //教育培训的分类
                            //技术讲课 1 
                            case "education_teach":
                                categoryList.Add(item.ModuleCode, new string[] { "1", item.ModuleName });
                                break;
                            //技术问答 2 
                            case "education_qaa":
                                categoryList.Add(item.ModuleCode, new string[] { "2", item.ModuleName });
                                break;
                            //事故预想 3 
                            case "education_expect":
                                categoryList.Add(item.ModuleCode, new string[] { "3", item.ModuleName });
                                break;
                            //反事故演习 4 
                            case "education_drill":
                                categoryList.Add(item.ModuleCode, new string[] { "4", item.ModuleName });
                                break;
                            //拷问讲解 7
                            case "education_qa":
                                categoryList.Add(item.ModuleCode, new string[] { "7", item.ModuleName });
                                break;
                            //安全学习日 
                            case "education_safeday":
                                categoryList.Add("EA", new string[] { item.ModuleName });
                                break;
                            //其他活动
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
