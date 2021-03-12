using BSFramework.Application.Entity.InnovationManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.InnovationManage
{
    /// <summary>
    /// 班组管理创新成果
    /// </summary>
    public interface IWorkInnovationService
    {

        /// <summary>
        /// 获取Advice数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<WorkInnovationEntity> getList(string queryJson, Pagination pagination);


        /// <summary>
        /// 根据用户id获取记录表数据
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        List<WorkInnovationEntity> getWorkInnovationbyuser(string userid);



        /// <summary>
        /// 根据主键id获取记录表数据
        /// </summary>
        /// <param name="Strid"></param>
        /// <returns></returns>
        List<WorkInnovationEntity> getWorkInnovationbyid(string Strid);

        /// <summary>
        /// 根据关联id获取审核数据
        /// </summary>
        /// <param name="innovationid"></param>
        /// <returns></returns>
        List<WorkInnovationAuditEntity> getAuditByid(string innovationid);


        /// <summary>
        /// 根据用户id获取审核数据
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        List<WorkInnovationAuditEntity> getAuditByuser(string userid);


        /// <summary>
        /// 根据id获取审核数据
        /// </summary>
        /// <param name="auditid"></param>
        /// <returns></returns>
        List<WorkInnovationAuditEntity> getAuditId(string auditid);



        /// <summary>
        /// 
        /// </summary>
        /// <param name="main"></param>
        /// <param name="audit"></param>
        /// <param name="type">是否存在数据</param>
        void Operation(WorkInnovationEntity main, List<WorkInnovationAuditEntity> audit, bool type);
        int GetInnovation(string deptid);
        /// <summary>
        /// 得到当前用户的待审核数量
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        int GetTodoCount(string userid);
    }
}
