using BSFramework.Application.Entity.CertificateManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.CertificateManage
{
    public interface ICertificateService
    {
        #region 获取数据
        /// <summary>
        /// 获取基础节点信息
        /// </summary>
        /// <returns></returns>
        IList<CertificateEntity> getCertificateFirst(); 


        /// <summary>
        /// 获取节点信息
        /// </summary>
        /// <returns></returns>
        IList<CertificateEntity> GetCertificate();
        /// <summary>
        /// 获取节点下的支点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IList<CertificateEntity> getCertificateById(string id);
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id">父节点id</param>
        /// <returns></returns>
        CertificateEntity selectLike(string name, string id);

        /// <summary>
        /// 模糊查询索引框
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id">父节点id</param>
        /// <returns></returns>
        IList<CertificateEntity> selectLikeList(string name, string id);
        #endregion
        #region 提交数据
        /// <summary>
        /// 新增类型
        /// </summary>
        /// <param name="entity"></param>

        bool addCertificate(CertificateEntity entity);

        /// <summary>
        ///删除菜单
        /// </summary>
        /// <param name="id"></param>
        void delCertificate(string id);
        #endregion
    }
}
