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
    /// 是否提交
    /// </summary>
    public class PerformanceupBLL
    {
        private IPerformanceupService service = new PerformanceupService();


        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public PerformanceupEntity getList(string titleid)
        {
            return service.getList(titleid);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public List<PerformanceupEntity> getListByMonth(string month)
        {
            return service.getListByMonth(month);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public List<PerformanceupEntity> getListByYear(string year)
        {
            return service.getListByYear(year);
        }
        /// <summary>
        /// 修改数据
        /// </summary>
        /// <returns></returns>
        public void operation(string titleid)
        {
            try
            {
                service.operation(titleid);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <returns></returns>
        public void add(List<PerformanceupEntity> list)
        {
            try
            {
                service.add(list);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
