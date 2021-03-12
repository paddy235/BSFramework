using BSFramework.Application.Entity.PerformanceManage;
using BSFramework.Application.Entity.PerformanceManage.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.PerformanceManage
{
    /// <summary>
    /// 绩效管理数据
    /// </summary>
    public interface IPerformanceSecondService
    {

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        IQueryable<PerformanceSecondEntity> getScore(DateTime time, string departmentid);

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        PerformanceSecondEntity getScoreByuser(DateTime time, string userid);
        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="entity"></param>
        void operation(List<PerformanceSecondEntity> entity);


        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="entity"></param>
        void add(List<PerformanceSecondEntity> entity);



        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        PerformanceSecondEntity getUserScore(DateTime time, string userid);
        List<PerformanceSecondEntity> getScore(DateTime time, List<string> departIds);
        void Insert(List<PerformanceSecondEntity> addData);
        void Delete(List<PerformanceSecondEntity> delData);
        List<PerformanceModel> getYearScoreByuser(string year, string userid);
    }
}
