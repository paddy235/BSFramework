using BSFramework.Application.Entity.InnovationManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.InnovationManage
{
    public interface IAdviceService
    {

        /// <summary>
        /// 获取Advice数据
        /// </summary>
        /// <returns></returns>
        List<AdviceEntity> getAdviceList(Dictionary<string, string> keyValue, Pagination pagination);


        /// <summary>
        /// 获取管理数据
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
         List<AdviceEntity> getAdvicebyuser(string userid);
 


        /// <summary>
        /// 获取用户数据
        /// </summary>
        /// <param name="Strid"></param>
        /// <returns></returns>
         List<AdviceEntity> getAdvicebyid(string Strid);
  

        /// <summary>
        /// 数据操作
        /// </summary>
        /// <param name="add"></param>
        /// <param name="update"></param>
        /// <param name="del"></param>
        /// <param name="audit"></param>
        void Operation(AdviceEntity add, AdviceEntity update, string del, AdviceAuditEntity audit, AdviceAuditEntity auditupdate);


        /// <summary>
        /// 获取审核记录
        /// </summary>
        /// <param name="adviceid"></param>
        /// <returns></returns>
         List<AdviceAuditEntity> getAuditByid(string adviceid);

        /// <summary>
        /// 获取审核记录
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        List<AdviceAuditEntity> getAuditByuser(string userid);


        /// <summary>
        /// 获取审核记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<AdviceAuditEntity> getAuditId(string id);
        int GetSuggestions(string deptid);
        int GetTodoCount(string userId);
    }
}
