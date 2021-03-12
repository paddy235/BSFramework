using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.WebApp;
using BSFramework.Application.IService.WebApp;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.WebApp
{
    public class MedicalService : RepositoryFactory<MedicalEntity>, IMedicalService
    {
        /// <summary>
        /// 获取信息
        /// </summary>
        /// <returns></returns>
        public IList<MedicalEntity> getMedicalInfo(string userid)
        {
            return this.BaseRepository().IQueryable().Where(x => x.createuserid == userid).ToList();
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <returns></returns>
        public MedicalEntity getMedicalDetail(string userid, string id)
        {
            return this.BaseRepository().IQueryable().FirstOrDefault(x =>x.MedicalId == id);
        }
        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="entity"></param>
        public void addMedical(MedicalEntity entity, FileInfoEntity file)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var oldfiles = (from q in db.IQueryable<FileInfoEntity>()
                                where q.RecId == entity.MedicalId & q.Description != "职业健康二维码"
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
                    file.RecId = entity.MedicalId;
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
        /// 修改信息
        /// </summary>
        public void modifyMedical(MedicalEntity entity)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            var oldfiles = (from q in db.IQueryable<FileInfoEntity>()
                            where q.RecId == entity.MedicalId & q.Description != "职业健康二维码"
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
        ///删除体检信息
        /// </summary>
        /// <param name="id"></param>
        public void delMedical(string id)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            var getOne = db.FindEntity<MedicalEntity>(id);
            try
            {
                var oldfiles = (from q in db.IQueryable<FileInfoEntity>()
                                where q.RecId == getOne.MedicalId
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
