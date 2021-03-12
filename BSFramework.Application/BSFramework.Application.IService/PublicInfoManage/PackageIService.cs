using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BSFramework.Application.IService.PublicInfoManage
{
    /// <summary>
    /// �� ����app�汾
    /// </summary>
    public interface PackageIService
    {
        #region ��ȡ����
        /// <summary>
        /// ��ȡ�б�
        /// </summary>
        /// <param name="queryJson">��ѯ����</param>
        /// <returns>�����б�</returns>
        IEnumerable<PackageEntity> GetList(string queryJson);
        /// <summary>
        /// ��ȡʵ��
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns></returns>
        PackageEntity GetEntity(string keyValue);

        /// <summary>
        /// ����Ӧ�����ͻ�ȡ���°汾�ź�����·��
        /// </summary>
        /// <param name="packType"></param>
        /// <returns></returns>
        PackageEntity GetEntity(int packType);
        IEnumerable<PackageEntity> GetPageList(Pagination pagination, string queryJson);
        #endregion

        #region �ύ����
        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="keyValue">����</param>
        void RemoveForm(string keyValue);
        PackageEntity GetEntity(Expression<Func<PackageEntity, bool>> expression);

        /// <summary>
        /// ��������������޸ģ�
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <param name="entity">ʵ�����</param>
        /// <returns></returns>
        void SaveForm(string keyValue, PackageEntity entity);
        #endregion
    }
}
