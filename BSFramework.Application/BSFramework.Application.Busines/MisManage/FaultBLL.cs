using BSFramework.Application.Entity.MisManage;
using BSFramework.Application.IService.MisManage;
using BSFramework.Application.Service.MisManage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.MisManage
{
    public class FaultBLL
    {
        public List<string> GetUnits()
        {
            IFaultService service = new FaultService();
            return service.GetUnits();
        }

        public List<string> GetSpecialties()
        {
            IFaultService service = new FaultService();
            return service.GetSpecialties();
        }

        public List<string> GetCategories()
        {
            IFaultService service = new FaultService();
            return service.GetCategories();
        }

        public List<string> GetStatus()
        {
            IFaultService service = new FaultService();
            return service.GetStatus();
        }

        public List<FaultEntity> GetFaults(string[] deptname, string[] units, string specialty, string[] categories, string status, int pagesize, int pageindex, out int total)
        {
            IFaultService service = new FaultService();
            return service.GetFaults(deptname,units, specialty, categories, status, pagesize, pageindex, out total);
        }
        public List<string> GetFaultsDept()
        {
            IFaultService service = new FaultService();
            return service.GetFaultsDept();
        }
        public List<FaultEntity> GetFaultsByClass(string[] deptname, string[] units, string specialty, int pagesize, int pageindex, out int total)
        {
            IFaultService service = new FaultService();
            return service.GetFaultsByClass(deptname, units, specialty, pagesize, pageindex, out total);
        }
        public List<FaultEntity> GetStatistical(string deptname, DateTime Start, DateTime End,string TeamType)
        {
            IFaultService service = new FaultService();
            return service.GetStatistical(deptname, Start,End,TeamType);
        }
        public FaultEntity GetDetail(decimal faultid)
        {
            IFaultService service = new FaultService();
            return service.GetDetail(faultid);
        }

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
        public DataTable GetJJRZ(int pageIndex, int pageSize, string bZMC, string fL, string kEYWORD, string sTARTDATE, string eNDDATE, ref int totalCount)
        {
            IFaultService service = new FaultService();
            return service.GetJJRZ(pageIndex, pageSize, bZMC, fL, kEYWORD, sTARTDATE, eNDDATE, ref totalCount);
        }
        /// <summary>
        /// 查询值班记录分类
        /// </summary>
        /// <returns></returns>
        public DataTable GetFL()
        {
            IFaultService service = new FaultService();
            return service.GetFL();
        }
    }
}
