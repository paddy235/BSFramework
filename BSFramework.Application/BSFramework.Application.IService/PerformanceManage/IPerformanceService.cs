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
    public interface IPerformanceService
    {

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        IQueryable<PerformanceEntity> getScore(string time, string departmentid);

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        PerformanceEntity getScoreByuser(string time, string userid);
        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="entity"></param>
        void operation(List<PerformanceEntity> entity);


        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="entity"></param>
        void add(List<PerformanceEntity> entity);



        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        PerformanceEntity getUserScore(string time, string userid);
        List<PerformanceEntity> getScore(string time, List<string> departIds);
        void Insert(List<PerformanceEntity> addData);
        void Delete(List<PerformanceEntity> delData);
        List<PerformanceModel> getYearScoreByuser(string year, string userid);
    }
}
