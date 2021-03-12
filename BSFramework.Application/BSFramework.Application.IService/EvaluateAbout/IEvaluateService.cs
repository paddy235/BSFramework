using BSFramework.Application.Entity.BaseManage;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using BSFramework.Entity.EvaluateAbout;
using System.Data;
using BSFramework.Application.Entity.EvaluateAbout;

namespace BSFramework.IService.EvaluateAbout
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public interface IEvaluateService
    {
        DataTable GetGroupsIndex(string evaluateid, string itemcontentid, string type);

        DataTable GetGroups(string evaluateid, string itemcontentid, string type);
        IList<EvaluateCategoryEntity> GetAllCategories();
        void Add(EvaluateCategoryEntity model);
        void Edit(EvaluateCategoryEntity model);
        void DeleteCategory(string id);
        void AddItem(EvaluateCategoryItemEntity model);
        void EditItem(EvaluateCategoryItemEntity model);
        IList<EvaluateCategoryItemEntity> GetCategoryItems(string key, string categoryid, int pagesize, int page, out int total);
        EvaluateCategoryItemEntity GetCategoryItem(string id);
        void DeleteItem(string id);
        IList<EvaluateCategoryItemEntity> GetAllCategoryItemsIndex(string deptname);
        IList<EvaluateEntity> GetEvaluations(string name, string userid, int pagesize, int page, out int total);
        void EnsureEvaluate(string name, DateTime date);
        EvaluateEntity GetEvaluate(string id);
        IList<EvaluateCategoryItemEntity> GetAllCategoryItems(string deptname);
        EvaluateEntity GetEvaluateionDetail(string id, string deptname, string deptid);
        IList<GroupIndex> GetCalcScoreIndex(string id, string categoryid, string deptcode);
        void AddItem(List<EvaluateCategoryItemEntity> models);
        object GetData1(string evaluateid);
        object GetData2(string evaluateid);
        void Evaluate(string id, List<EvaluateItemEntity> entities);
        void AddEvaluate(EvaluateEntity model);
        EvaluateItemEntity GetEvaluateItem(string id);
        void EditEvaluateItem(EvaluateItemEntity model);
        IList<EvaluateGroupEntity> GetCalcScore(string id, string categoryid);
        IList<EvaluateCategoryEntity> GetCategories();
        IList<EvaluateDeptEntity> GetAllDeptsById(string eid);
        void EditEvaluate(EvaluateEntity model);
        IList<EvaluateCalcEntity> GetCalcScore2(string year, string groupid);
        EvaluateCalcEntity GetGroupScore(string evaluateId, string groupid);
        EvaluateCalcEntity GetAvgScore(string evaluateid);
        IList<EvaluateCategoryEntity> GetBigCategories();
        IList<EvaluateScoreItemEntity> GetCalcScoreItme(string id, string CategoryId, string groupid);
        void Submit(string evaluateid, string deptid);
        void Submit(string id);
        void Publish(string id);
        IList<Group> GetCalcScoreNew(string id, string categoryid);
        IList<EvaluateItemEntity> GetItemsByItemId(string itemid);
        List<EvaluateItemEntity> GetMainData(string deptid);
        List<EvaluateGroupEntity> GetEvaluationResult();
        EvaluateEntity GetLastEvaluate();
        List<EvaluateItemEntity> GetEvaluateContent(string[] evaluateids, string[] deptids);
        EvaluateEntity GetEvaluateBySeason(string season);
        int GetAllDeptCountById(string keyValue);
        IList<EvaluateEntity> GetEvaluationsFoWeb(string name, string userid, int pagesize, int page, out int total);
        void DelEvaluateById(string keyValue);
        object GetCalcScore(string id, string categoryid, int indexType);
        IList<Group> GetCalcScoreNew(string id, string categoryid, int indexType);
        List<EvaluateMarksRecordsEntity> GetMarksRecords(string evaluateItemId);
        EvaluateMarksRecordsEntity GetMarksRecordEntity(string id);
        void AddMarksRecord(EvaluateMarksRecordsEntity entity);
        void UpdateMarksRecord(EvaluateMarksRecordsEntity entity);
        void RemoveMarksRecord(string id);
        List<EvaluateScoreDetail> EvaluateScoreDetail(string evaluateId, string categoryId, string bZID);
        List<EvaluateRecord> GetIndexGradeInfo();
    }
}
