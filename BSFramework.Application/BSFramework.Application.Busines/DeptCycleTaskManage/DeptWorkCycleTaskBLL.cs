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
    
  public  class DeptWorkCycleTaskBLL
    {
        private IDeptWorkCycleTaskServcie deptCycleServcie = new DeptWorkCycleTaskService();
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <param name="userid">用户</param>
        /// <returns></returns>
        public List<DeptWorkCycleTaskEntity> GetPageList(Pagination pagination, string queryJson, string userid)
        {
            return deptCycleServcie.GetPageList(pagination, queryJson, userid);
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public DeptWorkCycleTaskEntity getEntity(string keyvalue)
        {
            return deptCycleServcie.getEntity(keyvalue);
        }

        /// <summary>
        ///批量  新增 修改
        /// </summary>
        /// <param name="Listentity"></param>
        /// <param name="userid"></param>
        public void SaveForm(List<DeptWorkCycleTaskEntity> Listentity, string userid)
        {
            deptCycleServcie.SaveForm(Listentity, userid);
        }
        /// <summary>
        /// 新增 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        public void SaveForm(DeptWorkCycleTaskEntity entity, string userid)
        {
            deptCycleServcie.SaveForm(entity, userid);
        }
        /// <summary>
        /// 删除
        /// </summary>
        public void deleteEntity(string keyvalue)
        {
            deptCycleServcie.deleteEntity(keyvalue);
        }
    }
}
