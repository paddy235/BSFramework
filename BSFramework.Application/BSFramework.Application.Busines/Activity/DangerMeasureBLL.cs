using BSFramework.Application.Entity.Activity;
using BSFramework.Application.IService.Activity;
using BSFramework.Application.Service.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.Activity
{
    public class DangerMeasureBLL
    {
        public List<DangerCategoryEntity> GetCategories(string categoryid)
        {
            IDangerMeasureService service = new DangerMeasureService();
            return service.GetCategories(categoryid);
        }

        public DangerCategoryEntity GetCategory(Guid guid)
        {
            IDangerMeasureService service = new DangerMeasureService();
            return service.GetCategory(guid);
        }

        public void Save(DangerCategoryEntity entity)
        {
            IDangerMeasureService service = new DangerMeasureService();
            service.Save(entity);
        }

        public void Delete(string categoryid)
        {
            IDangerMeasureService service = new DangerMeasureService();
            service.Delete(categoryid);
        }

        public List<DangerMeasureEntity> GetData(string categoryid, string key, int pagesize, int page, out int total, string sortfield, string direction)
        {
            IDangerMeasureService service = new DangerMeasureService();
            return service.GetData(categoryid, key, pagesize, page, out total, sortfield, direction);
        }

        public DangerMeasureEntity GetMeasureDetail(string id)
        {
            IDangerMeasureService service = new DangerMeasureService();
            return service.GetMeasureDetail(id);
        }

        public void SaveMeasure(DangerMeasureEntity model)
        {
            IDangerMeasureService service = new DangerMeasureService();
            service.SaveMeasure(model);
        }

        public void DeleteMeasure(string measureid)
        {
            IDangerMeasureService service = new DangerMeasureService();
            service.DeleteMeasure(measureid);
        }

        public bool ExistDangerReason(string categoryId, string dangerReason)
        {
            IDangerMeasureService service = new DangerMeasureService();
            return service.ExistDangerReason(categoryId, dangerReason);
        }

        public void AddMeasures(List<DangerMeasureEntity> measuredata)
        {
            IDangerMeasureService service = new DangerMeasureService();
            service.AddMeasures(measuredata);
        }

        public List<DangerMeasureEntity> GetDangerReasons(string categoryid)
        {
            IDangerMeasureService service = new DangerMeasureService();
            return service.GetDangerReasons(categoryid);
        }

        public List<DangerMeasureEntity> GetAllReasons()
        {
            IDangerMeasureService service = new DangerMeasureService();
            return service.GetAllReasons();
        }

        public List<string> GetTaskAreas(string key, int pageSize, int pageIndex, out int total)
        {
            IDangerMeasureService service = new DangerMeasureService();
            return service.GetTaskAreas(key, pageSize, pageIndex, out total);
        }

        public List<DangerMeasureEntity> GetMeasures(string humandangerid, string measureid)
        {
            IDangerMeasureService service = new DangerMeasureService();
            return service.GetMeasures(humandangerid, measureid);
        }
    }
}
