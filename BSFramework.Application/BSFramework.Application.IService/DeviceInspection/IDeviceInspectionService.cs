using BSFramework.Application.Entity.DeviceInspection;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.DeviceInspection
{
    /// <summary>
    /// 设备巡回检查
    /// </summary>
    public interface IDeviceInspectionService
    {
        void SaveForm(string keyValue, DeviceInspectionEntity entity, List<DeviceInspectionItemEntity> items);
        bool ExistInspectionName(string keyValue, string inspectionName);
        IEnumerable<DeviceInspectionEntity> GetPageList(Pagination pagination, string queryJson);
        DeviceInspectionEntity GetEntity(string keyValue);
        List<DeviceInspectionItemEntity> GetDeviceInspectionItems(string deviceId);
        void RemoveInspection(string keyValue);
        List<DeviceInspectionEntity> GetPageList(int pageIndex, int pageSize, string departmentCode, string keyword, ref int totalCount);
        void Import(List<DeviceInspectionEntity> inspectionEntities, List<DeviceInspectionItemEntity> itemEntities);
        List<DeviceInspectionEntity> GetAllInspetionList();
    }
}
