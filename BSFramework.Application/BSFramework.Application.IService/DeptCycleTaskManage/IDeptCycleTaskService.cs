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
    /// 部门任务库
    /// </summary>
    public interface IDeptCycleTaskService
    {
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <param name="userid">用户</param>
        /// <returns></returns>
        List<DeptCycleTaskEntity> GetPageList(Pagination pagination, string queryJson, string userid);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        DeptCycleTaskEntity getEntity(string keyvalue);
        /// <summary>
        ///批量  新增 修改
        /// </summary>
        /// <param name="Listentity"></param>
        /// <param name="userid"></param>
        void SaveForm(List<DeptCycleTaskEntity> Listentity, string userid);
        /// <summary>
        /// 新增 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        void SaveForm(DeptCycleTaskEntity entity, string userid);
        /// <summary>
        /// 删除
        /// </summary>
        void deleteEntity(string keyvalue);

        /// <summary>
        /// 推送任务
        /// </summary>
        /// <param name="date"></param>
        /// <param name="deptid">可为空</param>
        /// <param name="userid">可为空</param>
        void GetDpetTask(DateTime date, string deptid, string userid);
    }
}
