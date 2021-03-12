using BSFramework.Application.Entity.PerformanceManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.PerformanceManage
{
    public interface IPerformancesetupService
    {
        /// <summary>
        /// 获取所有配置
        /// </summary>
        /// <returns></returns>
        List<PerformancesetupEntity> AllTitle(string departmentid);
        /// <summary>
        /// 操作配置  对应修改当前月标题和数据
        /// </summary>
        void operation(List<PerformancesetupEntity> add, List<PerformancesetupEntity> del, List<PerformancesetupEntity> Listupdate, PerformancetitleEntity title, List<PerformanceEntity> Score);


    }
}
