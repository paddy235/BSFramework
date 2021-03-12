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
    public class PerformancetitleService : RepositoryFactory<PerformancetitleEntity>, IPerformancetitleService
    {

        /// <summary>
        /// 获取标题
        /// </summary>
        /// <returns></returns>
        public PerformancetitleEntity getTitle(string time,string departmentid)
        {
            return this.BaseRepository().IQueryable().FirstOrDefault(x => x.usetime == time&&x.departmentid== departmentid);
        }

        
    }
}
