using BSFramework.Application.Entity.PerformanceManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.PerformanceManage
{
    public interface  IPerformanceupService
    {
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        PerformanceupEntity getList(string titleid);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        List<PerformanceupEntity> getListByMonth(string month);

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        List<PerformanceupEntity> getListByYear(string year);
        /// <summary>
        /// 修改数据
        /// </summary>
        /// <returns></returns>
        void operation(string titleid);

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <returns></returns>
        void add(List<PerformanceupEntity> list);
    }
}
