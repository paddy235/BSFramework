using BSFramework.Application.Entity.CustomerManage;
using BSFramework.Application.IService.CustomerManage;
using BSFramework.Application.Service.CustomerManage;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System;

namespace BSFramework.Application.Busines.CustomerManage
{
    /// <summary>
    /// �� �����ֽ����
    /// </summary>
    public class CashBalanceBLL
    {
        private ICashBalanceService service = new CashBalanceService();

        #region ��ȡ����
        /// <summary>
        /// ��ȡ�б�
        /// </summary>
        /// <param name="queryJson">��ѯ����</param>
        /// <returns>�����б�</returns>
        public IEnumerable<CashBalanceEntity> GetList(string queryJson)
        {
            return service.GetList(queryJson);
        }
        #endregion

        #region �ύ����
        #endregion
    }
}
