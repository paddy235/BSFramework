using BSFramework.Application.Entity.DeviceInspection;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.DeviceInspection
{
    public interface IInspectionRecordService
    {
        IEnumerable<InspectionRecordEntity> GetPageList(Pagination pagination, string queryJson);
        InspectionRecordEntity GetEntity(string keyValue);
        List<DeviceInspectionItemJobEntity> GetRecordItems(string recordId, string deviceId);
        void SaveRecord(InspectionRecordEntity recordEntity, List<ItemResultEntity> results, List<FileInfoEntity> files, List<FileInfoEntity> delFiles, bool isUpdate);
        List<InspectionRecordEntity> GetPageList(int pageIndex, int pageSize, string keyWord, string userId, string jobId, string issubmit, string time,string deptcode, int totalCount);
    }
}
