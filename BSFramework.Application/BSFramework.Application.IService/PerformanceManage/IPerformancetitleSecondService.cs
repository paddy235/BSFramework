using BSFramework.Application.Entity.PerformanceManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.PerformanceManage
{
    /// <summary>
    /// 绩效管理标题
    /// </summary>
    public interface IPerformancetitleSecondService
    {


        /// <summary>
        /// 获取标题
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        PerformancetitleSecondEntity getTitle(DateTime time,string departmentid);
    }
}
