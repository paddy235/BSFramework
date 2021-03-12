using BSFramework.Application.Entity.PerformanceManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.PerformanceManage
{
    public interface  IPerformanceupSecondService
    {
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        PerformanceupSecondEntity getList(string titleid);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        List<PerformanceupSecondEntity> getListByMonth(string month);

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        List<PerformanceupSecondEntity> getListByYear(string year);
        /// <summary>
        /// 修改数据
        /// </summary>
        /// <returns></returns>
        void operation(string titleid);

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <returns></returns>
        void add(List<PerformanceupSecondEntity> list);
    }
}
