using BSFramework.Application.Entity.CustomerManage;
using BSFramework.Util.WebControl;
using System.Collections.Generic;

namespace BSFramework.Application.IService.CustomerManage
{
    /// <summary>
    /// �� ��������֧��
    /// </summary>
    public interface IExpensesService
    {
        #region ��ȡ����
        /// <summary>
        /// ��ȡ�б�
        /// </summary>
        /// <param name="pagination">��ҳ</param>
        /// <param name="queryJson">��ѯ����</param>
        /// <returns>���ط�ҳ�б�</returns>
        IEnumerable<ExpensesEntity> GetPageList(Pagination pagination, string queryJson);
        /// <summary>
        /// ��ȡ�б�
        /// </summary>
        /// <param name="queryJson">��ѯ����</param>
        /// <returns>�����б�</returns>
        IEnumerable<ExpensesEntity> GetList(string queryJson);
        /// <summary>
        /// ��ȡʵ��
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns></returns>
        ExpensesEntity GetEntity(string keyValue);
        #endregion

        #region �ύ����
        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="keyValue">����</param>
        void RemoveForm(string keyValue);
        /// <summary>
        /// ���������������
        /// </summary>
        /// <param name="entity">ʵ�����</param>
        /// <returns></returns>
        void SaveForm(ExpensesEntity entity);
        #endregion
    }
}