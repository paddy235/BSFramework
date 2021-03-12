
using BSFramework.Application.Entity.CertificateManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.CertificateManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.CertificateManage
{
    public class UserCertificateService : RepositoryFactory<UserCertificateEntity>, IUserCertificateService
    {

        /// <summary>
        /// 根据id获取数据
        /// </summary>
        /// <param name="id"></param> 
        /// <returns></returns>
        public UserCertificateEntity getUserCertificateById(string id)
        {
            return this.BaseRepository().FindEntity(id);
        }

        /// <summary>
        /// 根据userid获取数据
        /// </summary>
        /// <param name="userid></param>
        /// <returns></returns>
        public List<UserCertificateEntity> getUserCertificateByuserId(string userid)
        {


            return this.BaseRepository().IQueryable().Where(x => x.userid == userid).ToList();
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="userid></param>
        /// <returns></returns>
        public UserCertificateEntity getUserCertificateById(string userid, string id)
        {
            return this.BaseRepository().IQueryable().FirstOrDefault(x => x.userid == userid && x.Id == id);
        }
        /// <summary>
        /// 添加用户证件信息
        /// </summary>
        /// <param name="entity"></param>
        public void addUserCertificate(UserCertificateEntity entity, FileInfoEntity file)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var oldfiles = (from q in db.IQueryable<FileInfoEntity>()
                                where q.RecId == entity.Id & q.Description != "成员证书二维码"
                                select q).ToList();
                if (entity.Files != null)
                {

                    var deletefiles = oldfiles.Except(entity.Files).ToList();
                    db.Delete(deletefiles);

                    var newfiles = entity.Files.Except(oldfiles).ToList();
                    db.Insert(newfiles);
                }
                entity.Files = null;
                if (file != null)
                {
                    file.RecId = entity.Id;
                    file.State = null;
                    db.Update(file);
                }
                db.Insert(entity);
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 修改证件信息
        /// </summary>
        public void modifyUserCertificate(UserCertificateEntity entity)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            var oldfiles = (from q in db.IQueryable<FileInfoEntity>()
                            where q.RecId == entity.Id & q.Description != "成员证书二维码"
                            select q).ToList();
            try
            {
                var deletefiles = oldfiles.Except(entity.Files).ToList();
                db.Delete(deletefiles);

                var newfiles = entity.Files.Except(oldfiles).ToList();
                db.Insert(newfiles);
                entity.Files = null;
                db.Update(entity);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

        }
        /// <summary>
        /// 删除证件信息
        /// </summary>
        public void DelUserCertificate(string id)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            var getOne = db.FindEntity<UserCertificateEntity>(id);
            try
            {
                var oldfiles = (from q in db.IQueryable<FileInfoEntity>()
                                where q.RecId == getOne.Id & q.CreateUserId == getOne.createuserid
                                select q).ToList();
                db.Delete(oldfiles);

                db.Delete(getOne);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

        }
    }
}
