using BSFramework.Application.Entity.DeviceInspection;
using BSFramework.Application.Entity.PublicInfoManage;
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
    /// 设备巡回检查记录操作类
    /// </summary>
    public class InspectionRecordBLL
    {
        private readonly IInspectionRecordService _service;
        public InspectionRecordBLL()
        {
            _service = new InspectionRecordService();
        }
        /// <summary>
        /// 分页查询设备巡回检查表
        /// </summary>
        /// <param name="pagination">分页信息</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<InspectionRecordEntity> GetPageList(Pagination pagination, string queryJson)
        {
            return _service.GetPageList(pagination, queryJson);
        }

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public InspectionRecordEntity GetEntity(string keyValue)
        {
            return _service.GetEntity(keyValue);
        }

        /// <summary>
        /// 获取检查记录中各检查项的检查结果
        /// </summary>
        /// <param name="recordId">检查记录的主键Id</param>
        /// <param name="deviceId">设备巡回检查表的Id</param>
        /// <returns></returns>
        public List<DeviceInspectionItemJobEntity> GetRecordItems(string recordId, string deviceId)
        {
            return _service.GetRecordItems(recordId, deviceId);
        }

        /// <summary>
        /// 新增设备巡回检查记录  包括检查结果与附件信息
        /// </summary>
        /// <param name="recordEntity">检查记录的实体</param>
        /// <param name="results">各检查项的检查结果</param>
        /// <param name="files">要新增的附件的实体</param>
        /// <param name="delFiles">要删除的附件的集合（FilePath属性未服务器的物理路径，用于System.IO的删除用，请先处理）</param>
        /// <param name="isUpdate">是否是修改，否按照新增处理</param>
        public void SaveRecord(InspectionRecordEntity recordEntity, List<ItemResultEntity> results, List<FileInfoEntity> files, List<FileInfoEntity> delFiles, bool isUpdate)
        {
            _service.SaveRecord(recordEntity, results, files, delFiles, isUpdate);
        }

        /// <summary>
        /// 分页查询检查记录
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页个数</param>
        /// <param name="keyWord">设备系统关键字</param>
        /// <param name="userId">检查人</param>
        /// <param name="jobId">任务的id</param>
        /// <param name="totalCount">总条数</param>
        /// <returns></returns>
        public List<InspectionRecordEntity> GetPageList(int pageIndex, int pageSize, string keyWord, string userId, string jobId, string issubmit, string time,string deptcode, ref int totalCount)
        {
            return _service.GetPageList(pageIndex, pageSize, keyWord, userId, jobId, issubmit, time, deptcode, totalCount);
        }
    }
}
