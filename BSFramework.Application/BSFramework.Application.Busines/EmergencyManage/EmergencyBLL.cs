using BSFramework.Application.Entity.EmergencyManage;
using BSFramework.Application.Entity.ToolManage;
using BSFramework.Application.IService.EmergencyManage;
using BSFramework.Application.IService.ToolManage;
using BSFramework.Application.Service.EmergencyManage;
using BSFramework.Application.Service.ToolManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.EmergencyManage
{
    public class EmergencyBLL
    {
        public IList<EmergencyReportEntity> GetAllList()
        {
            return service.GetAllList();
        }
        private IEmergencyService service = new EmergencyService();
        public IEnumerable<EmergencyEntity> GetList(string typename, string typeId, string name, string cardId, int pageIndex, int pageSize, bool ispage)
        {
            return service.GetList(typename, typeId, name, cardId, pageIndex, pageSize, ispage);
        }
        public void DeleteCardType(string id)
        {
            service.DeleteCardType(id);
        }
        public void DeleteItem(string id)
        {
            service.DeleteItem(id);
        }
        public void EditCardType(EmergencyCardTypeEntity model)
        {
            service.EditCardType(model);
        }
        public void AddCardType(EmergencyCardTypeEntity model)
        {
            model.TypeId = Guid.NewGuid().ToString();
            service.AddCardType(model);
        }
        public IList<EmergencyCardTypeEntity> GetIndex(string name, string deptCode, string typeid)
        {
            return service.GetIndex(name, deptCode, typeid);

        }
        public IList<EmergencyEntity> GetCarkItems(string key, string typeid, int pagesize, int page, string deptCode, out int total)
        {
            return service.GetCarkItems(key, typeid, pagesize, page, deptCode, out total);
        }

        public IList<EmergencyCardTypeEntity> GetAllCardType(string deptCode)
        {
            return service.GetAllCardType(deptCode);
        }

        public IEnumerable<EmergencyEntity> GetPageList(string from, string to, string name, string deptid, int page, int pagesize, out int total)
        {
            return service.GetPageList(from, to, name, deptid, page, pagesize, out total);
        }
        public EmergencyEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }
        public EmergencyReportEntity GetReportEntity(string keyValue)
        {
            return service.GetReportEntity(keyValue);
        }

        public EmergencyWorkEntity GetWorkEntity(string keyValue)
        {
            return service.GetWorkEntity(keyValue);
        }

        public void RemoveForm(string keyValue)
        {
            try
            {
                service.RemoveForm(keyValue);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, EmergencyEntity entity)
        {
            try
            {
                service.SaveForm(keyValue, entity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SaveEmergencyEntity(EmergencyEntity entity)
        {
            try
            {
                service.SaveEmergencyEntity(entity);
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveFormWork(string keyValue, EmergencyWorkEntity workEntity)
        {
            try
            {
                service.SaveFormWork(keyValue, workEntity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 应急预案
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IList<EmergencyWorkEntity> GetEvaluations(string deptid, string name, int pagesize, int page, string ToCompileDeptIdSearch, string EmergencyTypeSearch, out int total)
        {
            return service.GetEvaluations(deptid, name, pagesize, page, ToCompileDeptIdSearch, EmergencyTypeSearch, out total);
        }
        /// <summary>
        /// 应急演练
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pagesize"></param>
        /// <param name="page"></param>
        /// <param name="ToCompileDeptIdSearch"></param>
        /// <param name="EmergencyTypeSearch"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IList<EmergencyReportEntity> GetEvaluationsManoeuvre(string deptid, string name, int pagesize, int page, string ToCompileDeptIdSearch, string EmergencyTypeSearch, string meetingstarttime, string meetingendtime, out int total)
        {
            return service.GetEvaluationsManoeuvre(deptid, name, pagesize, page, ToCompileDeptIdSearch, EmergencyTypeSearch, meetingstarttime, meetingendtime, out total);
        }

        #region 管理平台
        /// <summary>
        /// 导入应急预案保存
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entitys"></param>
        public void SaveImportList(List<EmergencyWorkEntity> entity, List<EmergencyStepsEntity> entitys)
        {
            try
            {
                foreach (var item in entity)
                {
                    item.Create();
                }
                foreach (var item in entitys)
                {
                    item.Create();
                }
                service.SaveImportList(entity, entitys);
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 删除应急预案
        /// </summary>
        /// <param name="emergencyId"></param>
        public void deleteEmergency(string emergencyId)
        {
            try
            {
                service.deleteEmergency(emergencyId);

            }
            catch (Exception)
            {

                throw;
            }

        }
        public void DelEmergency(string keyValue)
        {
            try
            {
                service.DelEmergency(keyValue);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 获取应急演练步骤人员
        /// </summary>
        /// <returns></returns>
        public IList<EmergencyReportStepsEntity> GetEmergencyReportStepsList(string EmergencyId, string EmergencyReportId)
        {
            return service.GetEmergencyReportStepsList(EmergencyId, EmergencyReportId);

        }
        /// <summary>
        /// 获取应急演练
        /// </summary>
        /// <returns></returns>
        public IList<EmergencyReportEntity> GetEmergencyReportList(string createUser, string EmergencyReportId)
        {
            return service.GetEmergencyReportList(createUser, EmergencyReportId);
        }
        /// <summary>
        /// 获取应急预案
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="EmergencyType"></param>
        /// <returns></returns>
        public IList<EmergencyWorkEntity> GetEmergencyWorkList(string deptId, string EmergencyType, string EmergencyId, string createUser)
        {
            return service.GetEmergencyWorkList(deptId, EmergencyType, EmergencyId, createUser);

        }
        /// <summary>
        /// 获取应急预案步骤
        /// </summary>
        public IList<EmergencyStepsEntity> GetEmergencyStepsList(string EmergencyId, string createUser)
        {
            return service.GetEmergencyStepsList(EmergencyId, createUser);
        }
        #endregion

        #region 终端页面

        /// <summary>
        /// 应急预案列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<EmergencyReportEntity> EmergencyReportGetPageList(string DeptId, string name, DateTime? from, DateTime? to, int page, int pagesize, out int total)
        {
            return service.EmergencyReportGetPageList(DeptId, name, from, to, page, pagesize, out total);
        }
        /// <summary>
        /// 开始演练新增记录
        /// </summary>
        /// <param name="entity"></param>
        public void InsertEmergencyReport(EmergencyReportEntity entity, List<EmergencyPersonEntity> entitys)
        {
            try
            {
                service.InsertEmergencyReport(entity, entitys);
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// 终端页面应急预案
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="deptName"></param>
        /// <param name="EmergencyType"></param>
        /// <returns></returns>
        public IEnumerable<EmergencyWorkEntity> getEmergencyWorkList(DateTime? from, DateTime? to, string deptName, string EmergencyType)
        {

            return service.getEmergencyWorkList(from, to, deptName, EmergencyType);

        }
        /// <summary>
        /// 步骤修改数据
        /// </summary>
        public void updateEmergencyReport(string EmergencyReportId, string Purpose, string RehearsesceNario, string MainPoints, bool radio, string score, string effectreport, string planreport)
        {

            try
            {
                service.updateEmergencyReport(EmergencyReportId, Purpose, RehearsesceNario, MainPoints, radio, score, effectreport, planreport);
            }
            catch (Exception)
            {

                throw;
            }

        }
        /// <summary>
        /// 添加演练步骤
        /// </summary>
        /// <param name="entity"></param>
        public void saveReportSteps(List<EmergencyReportStepsEntity> entity)
        {
            try
            {
                service.saveReportSteps(entity);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void updateEmergencyReportEvaluate(string EmergencyReportId, EmergencyReportEntity workEntity)
        {

            try
            {
                service.updateEmergencyReportEvaluate(EmergencyReportId, workEntity);
            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion

    }
}
