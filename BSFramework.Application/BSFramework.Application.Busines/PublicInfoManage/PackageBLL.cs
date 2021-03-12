using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.PublicInfoManage;
using BSFramework.Application.Service.PublicInfoManage;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System;

namespace BSFramework.Application.Busines.PublicInfoManage
{
    /// <summary>
    /// �� ����app�汾
    /// </summary>
    public class PackageBLL
    {
        private PackageIService service = new PackageService();

        #region ��ȡ����
        /// <summary>
        /// ��ȡ�б�
        /// </summary>
        /// <param name="queryJson">��ѯ����</param>
        /// <returns>�����б�</returns>
        public IEnumerable<PackageEntity> GetList(string queryJson)
        {
            return service.GetList(queryJson);
        }
        /// <summary>
        /// ��ȡʵ��
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns></returns>
        public PackageEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }

        /// <summary>
        /// ��ȡʵ��
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns></returns>
        public PackageEntity GetEntity(System.Linq.Expressions.Expression<Func<PackageEntity,bool>> expression)
        {
            return service.GetEntity(expression);
        }

        /// <summary>
        /// ����Ӧ�����ͻ�ȡ���°汾�ź�����·��
        /// </summary>
        /// <param name="packType"></param>
        /// <returns></returns>
        public PackageEntity GetEntity(int packType)
        {
            return service.GetEntity(packType);
        }

        /// <summary>
        /// ��ȡ��ҳ���б�
        /// </summary>
        public IEnumerable<PackageEntity> GetPageList(Pagination pagination, string queryJson)
        {
            return service.GetPageList(pagination,queryJson);
        }
        #endregion

        #region �ύ����
        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="keyValue">����</param>
        public void RemoveForm(string keyValue)
        {
            try
            {
                service.RemoveForm(keyValue);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// ��������������޸ģ�
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <param name="entity">ʵ�����</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, PackageEntity entity)
        {
            try
            {
                service.SaveForm(keyValue, entity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
