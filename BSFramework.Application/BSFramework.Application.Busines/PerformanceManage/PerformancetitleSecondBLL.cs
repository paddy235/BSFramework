using BSFramework.Application.Entity.PerformanceManage;
using BSFramework.Application.IService.PerformanceManage;
using BSFramework.Application.Service.PerformanceManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.PerformanceManage
{
    /// <summary>
    /// 绩效管理标题
    /// </summary>
    public class PerformancetitleSecondBLL
    {
        private IPerformancetitleSecondService service = new PerformancetitleSecondService();
        /// <summary>
        /// 获取标题
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public PerformancetitleSecondEntity getTitle(DateTime time, string departmentid)
        {
            return service.getTitle(time, departmentid);
        }
    }
}
