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
    public class PerformancetitleBLL
    {
        private IPerformancetitleService service = new PerformancetitleService();
        /// <summary>
        /// 获取标题
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public PerformancetitleEntity getTitle(string time, string departmentid)
        {
            return service.getTitle(time, departmentid);
        }
    }
}
