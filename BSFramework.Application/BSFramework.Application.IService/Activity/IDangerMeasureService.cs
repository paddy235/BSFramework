using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.Activity
{
    /// <summary>
    /// 描 述：人身风险预控
    /// </summary>
    public interface IDangerMeasureService
    {
        List<DangerCategoryEntity> GetCategories(string categoryid);
        void Save(DangerCategoryEntity entity);
        void Delete(string id);
        List<DangerMeasureEntity> GetData(string categoryid, string key, int pagesize, int page, out int total, string sortfield, string direction);
        DangerMeasureEntity GetMeasureDetail(string measureid);
        DangerCategoryEntity GetCategory(Guid guid);
        void SaveMeasure(DangerMeasureEntity model);
        void DeleteMeasure(string id);
        bool ExistDangerReason(string categoryId, string dangerReason);
        void AddMeasures(List<DangerMeasureEntity> measuredata);
        List<DangerMeasureEntity> GetDangerReasons(string categoryid);
        List<DangerMeasureEntity> GetAllReasons();
        List<string> GetTaskAreas(string key, int pageSize, int pageIndex, out int total);
        List<DangerMeasureEntity> GetMeasures(string humandangerid, string measureid);
    }
}
