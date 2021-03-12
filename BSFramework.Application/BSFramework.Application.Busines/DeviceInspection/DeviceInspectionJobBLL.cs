using BSFramework.Application.Entity.DeviceInspection;
using BSFramework.Application.IService.DeviceInspection;
using BSFramework.Application.Service.DeviceInspection;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.DeviceInspection
{
    /// <summary>
    /// 设备巡回检查BLL
    /// </summary>
   public class DeviceInspectionJobBLL
    {
        private readonly IDeviceInspectionJobService _service;
        public DeviceInspectionJobBLL()
        {
            _service = new DeviceInspectionJobService();
        }
        /// <summary>
        /// 保存设备巡回检查信息
        /// </summary>
        /// <param name="keyValue">设备巡回检查表主键</param>
        /// <param name="entity">设备巡回检查实体</param>
        /// <param name="items">检查项Json格式数据</param>
        /// <returns></returns>
        public void SaveForm(string keyValue,DeviceInspectionJobEntity entity, List<DeviceInspectionItemJobEntity> items)
        {
             _service.SaveForm(keyValue, entity, items);
        }
        /// <summary>
        /// 设备巡回检查表 检查表名称唯一性检查
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="InspectionName">检查表名称的值</param>
        /// <returns></returns>
        public bool ExistInspectionName(string keyValue, string inspectionName)
        {
            return _service.ExistInspectionName(keyValue, inspectionName);
        }

        /// <summary>
        /// 根据设备巡回检查表的主键查询检查项列表 (非分页，查所有)
        /// </summary>
        /// <param name="deviceId">设备巡回检查表的主键</param>
        /// <returns></returns>
        public List<DeviceInspectionItemJobEntity> GetDeviceInspectionItems(string deviceId)
        {
            return _service.GetDeviceInspectionItems(deviceId);
        }

        /// <summary>
        /// 查询设备巡回检查表单个实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public DeviceInspectionJobEntity GetEntity(string keyValue)
        {
            return _service.GetEntity(keyValue);
        }

        /// <summary>
        /// 分页查询设备巡回检查表
        /// </summary>
        /// <param name="pagination">分页信息</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<DeviceInspectionJobEntity> GetPageList(Pagination pagination, string queryJson)
        {
            return _service.GetPageList(pagination, queryJson);
        }

        /// <summary>
        /// 删除设备巡回检查表 包括底下的巡回检查项
        /// </summary>
        /// <param name="keyValue">设备巡回检查表主键</param>
        public void RemoveInspection(string keyValue)
        {
            _service.RemoveInspection(keyValue);
        }

        /// <summary>
        /// 分页查询设备巡回检查表的数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页个数</param>
        /// <param name="departmentCode">部门编码</param>
        /// <param name="keyword">关键词</param>
        /// <param name="totalCount">总数量</param>
        /// <returns></returns>
        public List<DeviceInspectionJobEntity> GetPageList(int pageIndex, int pageSize, string departmentCode, string keyword, string state, string userid, ref int totalCount)
        {
            return _service.GetPageList(pageIndex, pageSize, departmentCode, keyword, state, userid, ref totalCount);
        }
        /// <summary>
        ///修改代办状态
        /// </summary>
        /// <param name="id"></param>
        public void SaveInspectionState(string id) {

             _service.SaveInspectionState(id);

        }


        public List<DeviceInspectionJobEntity> GetEntityByMeetOrJob(string MeetId, string JobId) {
           return  _service.GetEntityByMeetOrJob(MeetId,JobId);
        }
        /// <summary>
        /// 新增检查表与检查项
        /// </summary>
        /// <param name="inspectionEntities">检查表数据</param>
        /// <param name="itemEntities">检查项数据</param>
        public void Import(List<DeviceInspectionJobEntity> inspectionEntities, List<DeviceInspectionItemJobEntity> itemEntities)
        {
             _service.Import(inspectionEntities, itemEntities);
        }

        /// <summary>
        /// 查询所有的检查表
        /// </summary>
        /// <returns></returns>
        public List<DeviceInspectionJobEntity> GetAllInspetionList()
        {
            return _service.GetAllInspetionList();
        }
    }
}
