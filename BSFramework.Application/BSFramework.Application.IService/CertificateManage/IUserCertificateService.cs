using BSFramework.Application.Entity.CertificateManage;
using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.CertificateManage
{
    public interface IUserCertificateService
    {
        #region 获取数据
        /// <summary>
        /// 根据id获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserCertificateEntity getUserCertificateById(string id); 
        /// <summary>
        /// 根据userid获取数据
        /// </summary>
        /// <param name="userid></param>
        /// <returns></returns>
        List<UserCertificateEntity> getUserCertificateByuserId(string userid);
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="userid></param>
        /// <returns></returns>
        UserCertificateEntity getUserCertificateById(string userid, string id);
        #endregion
        #region 提交数据
        /// <summary>
        /// 添加用户证件信息
        /// </summary>
        /// <param name="entity"></param>
        void addUserCertificate(UserCertificateEntity entity, FileInfoEntity file);
        /// <summary>
        /// 修改证件信息
        /// </summary>
        void modifyUserCertificate(UserCertificateEntity entity);
        /// <summary>
        /// 删除证件信息
        /// </summary>
        void DelUserCertificate(string id);
        #endregion
    }
}
