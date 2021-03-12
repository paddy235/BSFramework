using BSFramework.Application.Entity.EmergencyManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.EmergencyManage
{
    public interface IEmergencyService
    {

        IList<EmergencyReportEntity> GetAllList();
        void SaveEmergencyEntity(EmergencyEntity entity);
        IList<EmergencyCardTypeEntity> GetIndex(string name, string deptCode, string typeid);
        IList<EmergencyEntity> GetCarkItems(string key, string typeid, int pagesize, int page, string deptCpde, out int total);
        void EditCardType(EmergencyCardTypeEntity model);
        void AddCardType(EmergencyCardTypeEntity model);
        IList<EmergencyCardTypeEntity> GetAllCardType(string deptCpde);
        IEnumerable<EmergencyEntity> GetList(string typename, string typeId, string name, string cardId, int pageIndex, int pageSize, bool ispage);

        IEnumerable<EmergencyEntity> GetPageList(string from, string to, string name, string bzid, int page, int pagesize, out int total);
        EmergencyEntity GetEntity(string keyValue);
        EmergencyReportEntity GetReportEntity(string keyValue);
        EmergencyWorkEntity GetWorkEntity(string keyValue);
        void DeleteCardType(string id);
        void DeleteItem(string id);
        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue"></param>
        void DelEmergency(string keyValue);
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        void SaveForm(string keyValue, EmergencyEntity entity);

        void SaveFormWork(string keyValue, EmergencyWorkEntity workEntity);
        IList<EmergencyWorkEntity> GetEvaluations(string deptid, string name, int pagesize, int page, string ToCompileDeptIdSearch, string EmergencyTypeSearch, out int total);
        IList<EmergencyReportEntity> GetEvaluationsManoeuvre(string deptid, string name, int pagesize, int page, string ToCompileDeptIdSearch, string EmergencyTypeSearch, string meetingstarttime, string meetingendtime, out int total);
        #endregion

        #region 管理平台
        /// <summary>
        /// 导入应急预案保存
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entitys"></param>
        void SaveImportList(List<EmergencyWorkEntity> entity, List<EmergencyStepsEntity> entitys);

        /// <summary>
        /// 删除应急预案
        /// </summary>
        /// <param name="emergencyId"></param>
        void deleteEmergency(string emergencyId);
        /// <summary>
        /// 获取应急演练步骤人员
        /// </summary>
        /// <returns></returns>
        IList<EmergencyReportStepsEntity> GetEmergencyReportStepsList(string EmergencyId, string EmergencyReportId);
        /// <summary>
        /// 获取应急演练
        /// </summary>
        /// <returns></returns>
        IList<EmergencyReportEntity> GetEmergencyReportList(string createUser, string EmergencyReportId);
        /// <summary>
        /// 获取应急预案
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="EmergencyType"></param>
        /// <returns></returns>
        IList<EmergencyWorkEntity> GetEmergencyWorkList(string deptId, string EmergencyType, string EmergencyId, string createUser);

        /// <summary>
        /// 获取应急预案步骤
        /// </summary>
        IList<EmergencyStepsEntity> GetEmergencyStepsList(string EmergencyId, string createUser);
        #endregion

        #region 终端页面
        /// <summary>
        /// 开始演练新增记录
        /// </summary>
        /// <param name="entity"></param>
        void InsertEmergencyReport(EmergencyReportEntity entity, List<EmergencyPersonEntity> entitys);
        /// <summary>
        /// 应急预案列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        IEnumerable<EmergencyReportEntity> EmergencyReportGetPageList(string DeptId, string name, DateTime? from, DateTime? to, int page, int pagesize, out int total);

        /// <summary>
        /// 终端页面应急预案
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="deptName"></param>
        /// <param name="EmergencyType"></param>
        /// <returns></returns>
        IEnumerable<EmergencyWorkEntity> getEmergencyWorkList(DateTime? from, DateTime? to, string deptName, string EmergencyType);
        /// <summary>
        /// 步骤修改数据
        /// </summary>
        void updateEmergencyReport(string EmergencyReportId, string Purpose, string RehearsesceNario, string MainPoints, bool radio, string score, string effectreport, string planreport);
        /// <summary>
        /// 添加演练步骤
        /// </summary>
        /// <param name="entity"></param>
        void saveReportSteps(List<EmergencyReportStepsEntity> entity);

        void updateEmergencyReportEvaluate(string EmergencyReportId, EmergencyReportEntity workEntity);
        #endregion
    }
}
