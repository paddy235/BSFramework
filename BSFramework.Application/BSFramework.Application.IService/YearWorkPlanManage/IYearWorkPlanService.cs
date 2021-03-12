using BSFramework.Application.Entity.YearWorkPlan;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.YearWorkPlanManage
{
    /// <summary>
    /// 
    /// </summary>
    public interface IYearWorkPlanService
    {

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        List<YearWorkPlanEntity> GetPlanList();

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        DataTable GetPageList(Pagination pagination, string queryJson);

        /// <summary>
        /// 数据操作
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        void SaveForm(string keyValue, YearWorkPlanEntity entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        void SaveFormList(List<YearWorkPlanEntity> entity);

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        YearWorkPlanEntity GetEntity(string keyValue);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue"></param>
         void RemoveForm(string keyValue);

    }
}
