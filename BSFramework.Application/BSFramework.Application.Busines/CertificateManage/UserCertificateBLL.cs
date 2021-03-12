using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Entity.BaseManage;
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
    /// 用户证件
    /// </summary>
    public class UserCertificateBLL
    {
        private IUserCertificateService bll = new UserCertificateService();
        /// <summary>
        /// 添加用户证件信息
        /// </summary>
        /// <param name="entity"></param>
        public void addUserCertificate(UserCertificateEntity entity)
        {
            try
            { 
                FileInfoBLL fb = new FileInfoBLL();
                var flist = fb.GetFilebyDescription(entity.createuserid, "成员证书二维码");
                bll.addUserCertificate(entity, flist);
            }
            catch (Exception)
            {

                throw;
            }

        }
        /// <summary>
        /// 根据userid获取数据
        /// </summary>
        /// <param name="userid></param>
        /// <returns></returns>
        public List<UserCertificateEntity> getUserCertificateByuserId(string userid)
        {
            var entity = bll.getUserCertificateByuserId(userid);
            FileInfoBLL file = new PublicInfoManage.FileInfoBLL();
            foreach (var item in entity)
            {
                item.Files = file.GetFilesByRecIdNew(item.Id).Where(x => x.Description == "照片").ToList();
            }
            return entity;
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="userid></param>
        /// <returns></returns>
        public UserCertificateEntity getUserCertificateById(string userid, string id)
        {

            var entity = bll.getUserCertificateById(userid, id);
            if (entity == null)
            {
                return entity;
            }
            FileInfoBLL file = new PublicInfoManage.FileInfoBLL();

            entity.Files = file.GetFilesByRecIdNew(entity.Id).ToList();

            return entity;
        }
        /// <summary>
        /// 根据id获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserCertificateEntity getUserCertificateById(string id)
        {
            return bll.getUserCertificateById(id);
        }

        /// <summary>
        /// 修改证件信息
        /// </summary>
        public void modifyUserCertificate(UserCertificateEntity entity)
        {
            bll.modifyUserCertificate(entity);

        }
        /// <summary>
        /// 删除证件信息
        /// </summary>
        public void DelUserCertificate(string id)
        {
            bll.DelUserCertificate(id);

        }
    }
}
