using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.MisManage;

namespace BSFramework.Application.IService.MisManage
{
    public interface IFaultService
    {
        List<string> GetUnits();
        List<string> GetSpecialties();
        List<string> GetCategories();
        List<string> GetStatus();
        List<FaultEntity> GetFaults(string[] deptname, string[] units, string specialty, string[] categories, string status, int pagesize, int pageindex, out int total);
        List<FaultEntity> GetFaultsByClass(string[] deptname, string[] units, string specialty, int pagesize, int pageindex, out int total);
        List<string> GetFaultsDept();
        List<FaultEntity> GetStatistical(string deptname, DateTime Start, DateTime End, string TeamType);


        FaultEntity GetDetail(decimal faultid);
        /// <summary>
        /// 分页查询班长值班交接日志
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每个个数</param>
        /// <param name="bZMC">班组，班号</param>
        /// <param name="fL">分类</param>
        /// <param name="kEYWORD">关键字</param>
        /// <param name="sTARTDATE">开始时间</param>
        /// <param name="eNDDATE">结束时间</param>
        /// <param name="totalCount">返回的总条数</param>
        /// <returns></returns>
        DataTable GetJJRZ(int pageIndex, int pageSize, string bZMC, string fL, string kEYWORD, string sTARTDATE, string eNDDATE, ref int totalCount);
        DataTable GetFL();
    }
}
