using BSFramework.Application.Entity.DeptCycleTaskManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.DeptCycleTaskManage
{
    /// <summary>
    /// 部门任务
    /// </summary>
    public interface IDeptWorkCycleTaskServcie
    {
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <param name="userid">用户</param>
        /// <returns></returns>
        List<DeptWorkCycleTaskEntity> GetPageList(Pagination pagination, string queryJson, string userid);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        DeptWorkCycleTaskEntity getEntity(string keyvalue);
        /// <summary>
        ///批量  新增 修改
        /// </summary>
        /// <param name="Listentity"></param>
        /// <param name="userid"></param>
        void SaveForm(List<DeptWorkCycleTaskEntity> Listentity, string userid);
        /// <summary>
        /// 新增 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        void SaveForm(DeptWorkCycleTaskEntity entity, string userid);
        /// <summary>
        /// 删除
        /// </summary>
        void deleteEntity(string keyvalue);
    }
}
