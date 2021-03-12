using BSFramework.Application.Entity.SweepManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.SweepManage
{
    /// <summary>
    /// 保洁管理
    /// </summary>
    public interface ISweepService
    {

        #region 获取数据

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        List<SweepEntity> getSweepData();
     

        /// <summary>
        /// id获取数据
        /// </summary>
        /// <returns></returns>
        SweepEntity getSweepDataById(string Id);
    
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        List<SweepEntity> getSweepAndItemData();


        /// <summary>
        /// id获取数据
        /// </summary>
        /// <returns></returns>
        SweepEntity getSweepAndItemDataById(string Id);
  
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        List<SweepEntity> GetPageSweepList(Pagination pagination, string queryJson);


        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        List<SweepEntity> GetPageSweepAndItemList(Pagination pagination, string queryJson);


        #endregion
        #region 数据操作
        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="entity"></param>
        void operateSweepAndItem(List<SweepEntity> entity);
   

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyvalue"></param>
        void removeSweepAndItem(string keyvalue);
     
        #endregion
    }
}
