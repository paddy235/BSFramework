using BSFramework.Application.Entity.DeptCycleTaskManage;
using BSFramework.Application.IService.DeptCycleTaskManage;
using BSFramework.Application.Service.DeptCycleTaskManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.DeptCycleTaskManage
{
    public class DeptCycleTaskBLL
    {
        private IDeptCycleTaskService dpetCycle = new DeptCycleTaskService();
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <param name="userid">用户</param>
        /// <returns></returns>
        public List<DeptCycleTaskEntity> GetPageList(Pagination pagination, string queryJson, string userid)
        {
            return dpetCycle.GetPageList(pagination, queryJson, userid);
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public DeptCycleTaskEntity getEntity(string keyvalue)
        {
            return dpetCycle.getEntity(keyvalue);
        }

        /// <summary>
        ///批量  新增 修改
        /// </summary>
        /// <param name="Listentity"></param>
        /// <param name="userid"></param>
        public void SaveForm(List<DeptCycleTaskEntity> Listentity, string userid) {
            dpetCycle.SaveForm(Listentity, userid);
        }
        /// <summary>
        /// 新增 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        public void SaveForm(DeptCycleTaskEntity entity, string userid)
        {
            dpetCycle.SaveForm(entity, userid);
        }
        /// <summary>
        /// 删除
        /// </summary>
        public void deleteEntity(string keyvalue) {
            dpetCycle.deleteEntity(keyvalue);
        }

        /// <summary>
        /// 推送任务
        /// </summary>
        /// <param name="date"></param>
        /// <param name="deptid">可为空</param>
        /// <param name="userid">可为空</param>
        public void GetDpetTask(DateTime date, string deptid, string userid) {
            dpetCycle.GetDpetTask(date,deptid,userid);
        }
    }
}
