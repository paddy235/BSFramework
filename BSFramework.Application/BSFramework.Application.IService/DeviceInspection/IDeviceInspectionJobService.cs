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
    public interface IDeviceInspectionJobService
    {
        void SaveForm(string keyValue, DeviceInspectionJobEntity entity, List<DeviceInspectionItemJobEntity> items);
        bool ExistInspectionName(string keyValue, string inspectionName);
        IEnumerable<DeviceInspectionJobEntity> GetPageList(Pagination pagination, string queryJson);
        DeviceInspectionJobEntity GetEntity(string keyValue);

        List<DeviceInspectionJobEntity> GetEntityByMeetOrJob(string MeetId, string JobId);
        List<DeviceInspectionItemJobEntity> GetDeviceInspectionItems(string deviceId);
        void RemoveInspection(string keyValue);
        List<DeviceInspectionJobEntity> GetPageList(int pageIndex, int pageSize, string departmentCode, string keyword, string state, string userid, ref int totalCount);
        /// <summary>
        ///修改代办状态
        /// </summary>
        /// <param name="id"></param>
        void SaveInspectionState(string id);
        void Import(List<DeviceInspectionJobEntity> inspectionEntities, List<DeviceInspectionItemJobEntity> itemEntities);
        List<DeviceInspectionJobEntity> GetAllInspetionList();
    }
}
