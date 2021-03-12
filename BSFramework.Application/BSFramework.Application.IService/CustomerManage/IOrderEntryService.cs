using BSFramework.Application.Entity.CustomerManage;
using BSFramework.Util.WebControl;
using System.Collections.Generic;

namespace BSFramework.Application.IService.CustomerManage
{
    /// <summary>
    /// 描 述：订单明细
    /// </summary>
    public interface IOrderEntryService
    {
        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="orderId">订单主键</param>
        /// <returns>返回列表</returns>
        IEnumerable<OrderEntryEntity> GetList(string orderId);
        #endregion
    }
}