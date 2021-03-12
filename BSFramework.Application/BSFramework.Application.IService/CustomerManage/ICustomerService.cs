using BSFramework.Application.Entity.CustomerManage;
using BSFramework.Util.WebControl;
using System.Collections.Generic;

namespace BSFramework.Application.IService.CustomerManage
{
    /// <summary>
    /// �� �����ͻ���Ϣ
    /// </summary>
    public interface ICustomerService
    {
        #region ��ȡ����
        /// <summary>
        /// ��ȡ�б�
        /// </summary>
        /// <returns></returns>
        IEnumerable<CustomerEntity> GetList();
        /// <summary>
        /// ��ȡ�б�
        /// </summary>
        /// <param name="pagination">��ҳ</param>
        /// <param name="queryJson">��ѯ����</param>
        /// <returns>���ط�ҳ�б�</returns>
        IEnumerable<CustomerEntity> GetPageList(Pagination pagination, string queryJson);
        /// <summary>
        /// ��ȡʵ��
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns></returns>
        CustomerEntity GetEntity(string keyValue);
        #endregion

        #region ��֤����
        /// <summary>
        /// �ͻ����Ʋ����ظ�
        /// </summary>
        /// <param name="fullName">����</param>
        /// <param name="keyValue">����</param>
        /// <returns></returns>
        bool ExistFullName(string fullName, string keyValue);
        #endregion

        #region �ύ����
        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="keyValue">����</param>
        void RemoveForm(string keyValue);
        /// <summary>
        /// ����������������޸ģ�
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <param name="entity">ʵ�����</param>
        /// <returns></returns>
        void SaveForm(string keyValue, CustomerEntity entity);

        /// <summary>
        /// �������(����/�޸�)
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        /// <param name="moduleId">ģ��</param>
        void SaveForm(string keyValue, CustomerEntity entity, string moduleId);
        #endregion
    }
}