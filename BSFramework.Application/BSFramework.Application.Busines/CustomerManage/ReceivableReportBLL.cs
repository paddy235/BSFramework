using BSFramework.Application.Entity.CustomerManage.ViewModel;
using BSFramework.Application.IService.CustomerManage;
using BSFramework.Application.Service.CustomerManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.CustomerManage
{
    /// <summary>
    /// 描 述：应收账款报表
    /// </summary>
    public class ReceivableReportBLL
    {
        private IReceivableReportService service = new ReceivableReportService();

        /// <summary>
        /// 获取收款列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public IEnumerable<ReceivableReportModel> GetList(string queryJson)
        {
            return service.GetList(queryJson);
        }
    }
}
