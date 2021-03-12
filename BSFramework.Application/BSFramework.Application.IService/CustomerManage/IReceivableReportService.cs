using BSFramework.Application.Entity.CustomerManage;
using BSFramework.Application.Entity.CustomerManage.ViewModel;
using BSFramework.Util.WebControl;
using System.Collections.Generic;

namespace BSFramework.Application.IService.CustomerManage
{
    /// <summary>
    /// 描 述：应收账款报表
    /// </summary>
    public interface IReceivableReportService
    {
        /// <summary>
        /// 获取收款列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        IEnumerable<ReceivableReportModel> GetList(string queryJson);
    }
}