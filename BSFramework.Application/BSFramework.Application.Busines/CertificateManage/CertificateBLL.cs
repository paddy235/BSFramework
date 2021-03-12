using BSFramework.Application.Entity.CertificateManage;
using BSFramework.Application.IService.CertificateManage;
using BSFramework.Application.Service.CertificateManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.CertificateManage
{
    /// <summary>
    /// 证件管理
    /// </summary>
    public class CertificateBLL
    {
        private ICertificateService bll = new CertificateService();
        /// <summary>
        /// 获取基础节点信息
        /// </summary>
        /// <returns></returns>
        public IList<CertificateEntity> getCertificateFirst()
        {
            return bll.getCertificateFirst();
        } 

        /// <summary>
        /// 获取节点信息
        /// </summary>
        /// <returns></returns>
        public IList<CertificateEntity> GetCertificate()
        {
            return bll.GetCertificate();
        }
        /// <summary>
        /// 获取节点下的支点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IList<CertificateEntity> getCertificateById(string id)
        {
            return bll.getCertificateById(id);
        }

        /// <summary>
        /// 新增类型
        /// </summary>
        /// <param name="entity"></param>
        public bool addCertificate(CertificateEntity entity)
        {
            return bll.addCertificate(entity);

        }
        /// <summary>
        ///删除菜单
        /// </summary>
        /// <param name="id"></param>
        public void delCertificate(string id)
        {
            bll.delCertificate(id);
        }
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id">父节点id</param>
        /// <returns></returns>
        public CertificateEntity selectLike(string name, string id)
        {

            return bll.selectLike(name, id);
        }

        /// <summary>
        /// 模糊查询索引框
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id">父节点id</param>
        /// <returns></returns>
        public IList<CertificateEntity> selectLikeList(string name, string id)
        {
            return bll.selectLikeList(name, id);
        }
    }
}
