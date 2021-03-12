using BSFramework.Application.Entity.SweepManage;
using BSFramework.Application.IService.SweepManage;
using BSFramework.Application.Service.SweepManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.SweepManage
{
    /// <summary>
    /// 保洁管理
    /// </summary>
    public class SweepBLL
    {
        private ISweepService service = new SweepService();
        #region 获取数据

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public List<SweepEntity> getSweepData()
        {
            return service.getSweepData();
        }

        /// <summary>
        /// id获取数据
        /// </summary>
        /// <returns></returns>
        public SweepEntity getSweepDataById(string Id)
        {
            return service.getSweepDataById(Id);
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public List<SweepEntity> getSweepAndItemData()
        {
            return service.getSweepAndItemData();
        }

        /// <summary>
        /// id获取数据
        /// </summary>
        /// <returns></returns>
        public SweepEntity getSweepAndItemDataById(string Id)
        {
            return service.getSweepAndItemDataById(Id);
        }
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public List<SweepEntity> GetPageSweepList(Pagination pagination, string queryJson)
        {
            return service.GetPageSweepList(pagination,queryJson);
        }

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public List<SweepEntity> GetPageSweepAndItemList(Pagination pagination, string queryJson)
        {
            return service.GetPageSweepAndItemList(pagination, queryJson);
        }

        #endregion
        #region 数据操作
        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="entity"></param>
        public void operateSweepAndItem(List<SweepEntity> entity)
        {
            service.operateSweepAndItem(entity);
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyvalue"></param>
        public void removeSweepAndItem(string keyvalue)
        {
            service.removeSweepAndItem(keyvalue);
        }
        #endregion
    }
}
