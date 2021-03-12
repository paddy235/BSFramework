using BSFramework.Entity.WorkMeeting;
using BSFramework.IService.WorkMeeting;
using BSFramework.Service.WorkMeeting;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System;
using BSFramework.Application.Entity.BaseManage;
using System.Linq;
using BSFramework.Application.Busines.Activity;
using BSFramework.Entity.EvaluateAbout;
using BSFramework.IService.EvaluateAbout;
using BSFramework.Service.EvaluateAbout;
using BSFramework.Application.Code;
using System.Data;
using BSFramework.Application.Entity.EvaluateAbout;

namespace BSFramework.Busines.EvaluateAbout
{
    /// <summary>
    /// �� ��������
    /// </summary>
    public class EvaluateBLL
    {
        private IEvaluateService service = new EvaluateService();

        public IList<EvaluateCategoryEntity> GetAllCategories()
        {
            return service.GetAllCategories();
        }

        public void Add(EvaluateCategoryEntity model)
        {
            model.CategoryId = Guid.NewGuid().ToString();
            service.Add(model);
        }

        public void AddItem(List<EvaluateCategoryItemEntity> models)
        {
            service.AddItem(models);
        }
        public IList<EvaluateCategoryItemEntity> GetAllCategoryItemsIndex(string deptname)
        {
            return service.GetAllCategoryItemsIndex(deptname);
        }
        public IList<EvaluateCategoryItemEntity> GetAllCategoryItems(string deptname)
        {
            return service.GetAllCategoryItems(deptname);
        }

        public object GetData1(string evaluateid)
        {
            return service.GetData1(evaluateid);
        }

        public object GetData2(string evaluateid)
        {
            return service.GetData2(evaluateid);
        }

        public void AddEvaluate(EvaluateEntity model)
        {
            model.EvaluateId = Guid.NewGuid().ToString();
            model.PublishDate = DateTime.Now;
            model.CreateTime = DateTime.Now;
            service.AddEvaluate(model);
        }

        public void EditEvaluate(EvaluateEntity model)
        {
            service.EditEvaluate(model);
        }

        public void EnsureEvaluate()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var season = "һ";
            if (month >= 1 && month <= 3)
                season = "һ";
            else if (month >= 4 && month <= 6)
                season = "��";
            else if (month >= 7 && month <= 9)
                season = "��";
            else if (month >= 10 && month <= 12)
                season = "��";

            var name = string.Format("{0}���{1}����", year, season);
            service.EnsureEvaluate(name, DateTime.Now);
        }

        public EvaluateEntity GetEvaluate(string id)
        {
            return service.GetEvaluate(id);
        }

        public IList<EvaluateEntity> GetEvaluations(string name, string userid, int pagesize, int page, out int total)
        {
            return service.GetEvaluations(name, userid, pagesize, page, out total);
        }
        public IList<EvaluateEntity> GetEvaluationsFoWeb(string name, string userid, int pagesize, int page, out int total)
        {
            return service.GetEvaluationsFoWeb(name, userid, pagesize, page, out total);
        }
        public EvaluateEntity GetEvaluateionDetail(string id, string deptname,string deptid=null)
        {
            return service.GetEvaluateionDetail(id, deptname, deptid);
        }

        public void Edit(EvaluateCategoryEntity model)
        {
            service.Edit(model);
        }

        public void DeleteCategory(string id)
        {
            service.DeleteCategory(id);
        }

        public void DeleteItem(string id)
        {
            service.DeleteItem(id);
        }

        public void AddItem(EvaluateCategoryItemEntity model)
        {
            model.ItemId = Guid.NewGuid().ToString();
            model.CreateUserId = OperatorProvider.Provider.Current().UserId;
            model.CreateTime = DateTime.Now;
            service.AddItem(model);
        }

        public void Evaluate(string id, List<EvaluateItemEntity> entities)
        {
            service.Evaluate(id, entities);
        }

        public void EditItem(EvaluateCategoryItemEntity model)
        {
            service.EditItem(model);
        }

        public IList<EvaluateCategoryItemEntity> GetCategoryItems(string key, string categoryid, int pagesize, int page, out int total)
        {
            return service.GetCategoryItems(key, categoryid, pagesize, page, out total);
        }

        public EvaluateItemEntity GetEvaluateItem(string id)
        {
            return service.GetEvaluateItem(id);
        }

        public EvaluateCategoryItemEntity GetCategoryItem(string id)
        {
            return service.GetCategoryItem(id);
        }

        public void EditEvaluateItem(EvaluateItemEntity model)
        {
            service.EditEvaluateItem(model);
        }

        public IList<EvaluateGroupEntity> GetCalcScore(string id, string categoryid)
        {
            return service.GetCalcScore(id, categoryid);
        }

        public IList<EvaluateCategoryEntity> GetCategories()
        {
            return service.GetCategories();
        }

        public IList<EvaluateCalcEntity> GetCalcScore2(string year, string groupid)
        {
            return service.GetCalcScore2(year, groupid);
        }
        public IList<EvaluateScoreItemEntity> GetCalcScoreItme(string id, string CategoryId, string groupid)
        {
            return service.GetCalcScoreItme(id, CategoryId, groupid);
        }
        public IList<EvaluateCalcEntity> GetCalcScore3(string evaluateId, string groupid, string othergroupid)
        {
            var items = service.GetCalcScore(evaluateId, null);
            var avg = service.GetAvgScore(evaluateId);
            var current = service.GetGroupScore(evaluateId, groupid);
            var other = service.GetGroupScore(evaluateId, othergroupid);
            var seq = 1;
            if (items.Count > 0)
            {
                var lastScore = items[0].Pct;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Pct != lastScore)
                    {
                        lastScore = items[i].Pct;
                        seq++;
                    }
                    if (items[i].GroupName == current.Season)
                        break;
                }
            }
            current.Seq = seq;

            seq = 1;
            if (items.Count > 0)
            {
                var lastScore = items[0].Pct;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Pct != lastScore)
                    {
                        lastScore = items[i].Pct;
                        seq++;
                    }
                    if (items[i].GroupName == other.Season)
                        break;
                }
            }
            other.Seq = seq;

            return new List<EvaluateCalcEntity>() { avg, current, other };
        }

        public EvaluateCalcEntity GetCalcScore4(string evaluateId, string groupid)
        {
            var items = service.GetCalcScore(evaluateId, null);
            var data = service.GetGroupScore(evaluateId, groupid);
            var seq = 1;
            if (items.Count > 0)
            {
                var lastScore = items[0].Pct;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Pct != lastScore)
                    {
                        lastScore = items[i].Pct;
                        seq++;
                    }
                    if (items[i].GroupName == data.Season)
                        break;
                }
            }
            data.Seq = seq;
            return data;
        }

        public List<EvaluateGroupEntity> GetEvaluationResult()
        {
            return service.GetEvaluationResult();
        }

        public List<EvaluateItemEntity> GetMainData(string deptid)
        {
            return service.GetMainData(deptid);
        }

        public EvaluateCalcEntity GetAvgScore(string evaluateid)
        {
            return service.GetAvgScore(evaluateid);
        }
        public IList<EvaluateDeptEntity> GetAllDeptsById(string eid)
        {
            return service.GetAllDeptsById(eid);
        }
        public IList<EvaluateCategoryEntity> GetBigCategories()
        {
            return service.GetBigCategories();
        }

        public void Submit(string evaluateid, string deptid)
        {
            service.Submit(evaluateid, deptid);
        }

        public void SubmitAll(string id)
        {
            service.Submit(id);
        }
        public void Publish(string id)
        {
            service.Publish(id);
        }
        public IList<GroupIndex> GetCalcScoreIndex(string id, string categoryid, string dpetcode)
        {
            return service.GetCalcScoreIndex(id, categoryid, dpetcode);

        }
        public IList<Group> GetCalcScoreNew(string id, string categoryid)
        {
            return service.GetCalcScoreNew(id, categoryid);

        }
        public IList<EvaluateItemEntity> GetItemsByItemId(string itemid)
        {
            return service.GetItemsByItemId(itemid);
        }
        public DataTable GetGroups(string evaluateid, string itemcontentid, string type)
        {
            return service.GetGroups(evaluateid, itemcontentid, type);
        }
        public DataTable GetGroupsIndex(string evaluateid, string itemcontentid, string type)
        {
            return service.GetGroupsIndex(evaluateid, itemcontentid, type);
        }
        public EvaluateEntity GetLastEvaluate()
        {
            return service.GetLastEvaluate();
        }

        public List<EvaluateItemEntity> GetEvaluateContent(string[] evaluateids, string[] deptids)
        {
            return service.GetEvaluateContent(evaluateids, deptids);
        }

        public EvaluateEntity GetEvaluate1(EvaluateEntity current)
        {
            var year = current.EvaluateSeason.Substring(0, current.EvaluateSeason.IndexOf('��'));
            var lastyear = (int.Parse(year) - 1).ToString();
            var season = current.EvaluateSeason.Replace(year, lastyear);
            return service.GetEvaluateBySeason(season);
        }

        public EvaluateEntity GetEvaluate2(EvaluateEntity current)
        {
            var s1 = string.Empty;
            var s2 = string.Empty;
            var season = "xxxx";
            if (current.EvaluateSeason.Contains("����"))
            {
                s1 = current.EvaluateSeason.Substring(0, current.EvaluateSeason.IndexOf("��"));
                s2 = current.EvaluateSeason.Substring(current.EvaluateSeason.IndexOf("��") + 1, 1);
                if (s2 == "1")
                {
                    s2 = "4";
                    s1 = (int.Parse(s1) - 1).ToString();
                }
                else
                {
                    s2 = (int.Parse(s2) - 1).ToString();
                }
                season = string.Format("{0}���{1}����", s1, s2);
            }
            else if (current.EvaluateSeason.Contains("�¶�"))
            {
                s1 = current.EvaluateSeason.Substring(0, current.EvaluateSeason.IndexOf("��"));
                s2 = current.EvaluateSeason.Substring(current.EvaluateSeason.IndexOf("��") + 1, current.EvaluateSeason.IndexOf("��") - current.EvaluateSeason.IndexOf("��") - 1);
                if (s2 == "1")
                {
                    s2 = "12";
                    s1 = (int.Parse(s1) - 1).ToString();
                }
                else
                {
                    s2 = (int.Parse(s2) - 1).ToString();
                }
                season = string.Format("{0}��{1}��", s1, s2);
            }
            else
            {
                s1 = current.EvaluateSeason.Substring(0, current.EvaluateSeason.IndexOf("��"));
                var n = int.Parse(s1);
                --n;
                season = string.Format("{0}��", n);
            }

            return service.GetEvaluateBySeason(season);
        }

        public int GetAllDeptCountById(string keyValue)
        {
            return service.GetAllDeptCountById(keyValue);
        }

        public void DelEvaluateById(string keyValue)
        {
            service.DelEvaluateById(keyValue);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="categoryid"></param>
        /// <param name="indexType">������0 ������1</param>
        /// <returns></returns>
        public object GetCalcScore(string id, string categoryid, int indexType)
        {
            return service.GetCalcScore(id, categoryid, indexType);
        }

        public IList<Group> GetCalcScoreNew(string id, string categoryid, int indexType)
        {
            if (indexType == 0)
            {
                return service.GetCalcScoreNew(id, categoryid);
            }
            else
            {
                return service.GetCalcScoreNew(id, categoryid, indexType);
            }

        }
        /// <summary>
        /// �����Ӽ��ּ�¼
        /// </summary>
        /// <param name="evaluateItemId"></param>
        /// <returns></returns>
        public List<EvaluateMarksRecordsEntity> GetMarksRecords(string evaluateItemId)
        {
            return service.GetMarksRecords(evaluateItemId);
        }
        /// <summary>
        /// ��ȡ�Ӽ�������
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EvaluateMarksRecordsEntity GetMarksRecordEntity(string id)
        {
            return service.GetMarksRecordEntity(id);
        }
        /// <summary>
        /// ����������¼
        /// </summary>
        /// <param name="entity"></param>
        public void AddMarksRecord(EvaluateMarksRecordsEntity entity)
        {
            service.AddMarksRecord(entity);
        }
        /// <summary>
        /// �޸Ŀ�����¼
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateMarksRecord(EvaluateMarksRecordsEntity entity)
        {
            service.UpdateMarksRecord(entity);
        }
        /// <summary>
        /// ɾ��������¼
        /// </summary>
        /// <param name="id"></param>
        public void RemoveMarksRecord(string id)
        {
            service.RemoveMarksRecord(id);
        }

        public List<EvaluateScoreDetail> EvaluateScoreDetail(string evaluateId, string categoryId, string bZID)
        {
          return service.EvaluateScoreDetail(evaluateId, categoryId, bZID);
        }

        /// <summary>
        ///ƽ̨��ҳ- ��ȡ���µ�����ʵʱ�۷���Ϣ
        /// </summary>
        /// <returns></returns>
        public List<EvaluateRecord> GetIndexGradeInfo()
        {
            return service.GetIndexGradeInfo();
        }
    }
}
