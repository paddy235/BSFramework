using BSFramework.Application.Entity.CustomerManage;
using BSFramework.Util.WebControl;
using System.Collections.Generic;

namespace BSFramework.Application.IService.CustomerManage
{
    /// <summary>
    /// 描 述：应收账款
    /// </summary>
    public interface IReceivableService
    {
        #region 获取数据
        /// <summary>
        /// 获取收款单列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表</returns>
        IEnumerable<OrderEntity> GetPaymentPageList(Pagination pagination, string queryJson);
        /// <summary>
        /// 获取收款记录列表
        /// </summary>
        /// <param name="orderId">订单主键</param>
        /// <returns></returns>
        IEnumerable<ReceivableEntity> GetPaymentRecord(string orderId);
        #endregion

        #region 提交数据
        /// <summary>
        /// 保存表单（新增）
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        void SaveForm(ReceivableEntity entity);
        #endregion
    }
}