using BSFramework.Application.Entity.CustomerManage;
using BSFramework.Application.IService.CustomerManage;
using BSFramework.Application.Service.CustomerManage;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System;

namespace BSFramework.Application.Busines.CustomerManage
{
    /// <summary>
    /// 描 述：现金余额
    /// </summary>
    public class CashBalanceBLL
    {
        private ICashBalanceService service = new CashBalanceService();

        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public IEnumerable<CashBalanceEntity> GetList(string queryJson)
        {
            return service.GetList(queryJson);
        }
        #endregion

        #region 提交数据
        #endregion
    }
}
