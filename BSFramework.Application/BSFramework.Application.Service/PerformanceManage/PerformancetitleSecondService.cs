using BSFramework.Application.Entity.PerformanceManage;
using BSFramework.Application.IService.PerformanceManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.PerformanceManage
{
    /// <summary>
    /// 绩效管理标题
    /// </summary>
    public class PerformancetitleSecondService : RepositoryFactory<PerformancetitleSecondEntity>, IPerformancetitleSecondService
    {

        /// <summary>
        /// 获取标题
        /// </summary>
        /// <returns></returns>
        public PerformancetitleSecondEntity getTitle(DateTime time,string departmentid)
        {
            return this.BaseRepository().IQueryable().FirstOrDefault(x => x.usetime == time&&x.departmentid== departmentid);
        }

        
    }
}
