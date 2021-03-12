using BSFramework.Application.Entity.CertificateManage;
using BSFramework.Application.IService.CertificateManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.CertificateManage
{
    public class CertificateService : RepositoryFactory<CertificateEntity>, ICertificateService
    {
        /// <summary>
        /// 获取基础节点信息
        /// </summary>
        /// <returns></returns>
        public IList<CertificateEntity> getCertificateFirst() 
        {
            return this.BaseRepository().IQueryable().Where(x => x.ParentId == null && x.IsEffective).ToList();
        }


        /// <summary>
        /// 获取节点信息
        /// </summary>
        /// <returns></returns>
        public IList<CertificateEntity> GetCertificate()
        {
            return this.BaseRepository().IQueryable().ToList();
        }

        /// <summary>
        /// 获取节点下的支点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IList<CertificateEntity> getCertificateById(string id)
        {
            var db = new RepositoryFactory().BaseRepository();
            var Certificategroups = from q1 in db.IQueryable<CertificateEntity>()
                                    join q2 in db.IQueryable<CertificateEntity>() on q1.ParentId equals q2.CertificateId
                                    where q2.CertificateId == id && q1.IsEffective
                                    select q1;
            return Certificategroups.ToList();
        }
        /// <summary>
        /// 新增类型
        /// </summary>
        /// <param name="entity"></param>
        public bool addCertificate(CertificateEntity entity)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                if (string.IsNullOrEmpty(entity.CertificateId))
                {
                    entity.Create();
                    db.Insert(entity);
                }
                else
                {
                    entity.Modify(entity.CertificateId);
                    db.Update(entity);
                }

                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();

                return false;
            }

            return true;

        }

        /// <summary>
        ///删除菜单
        /// </summary>
        /// <param name="id"></param>
        public void delCertificate(string id)
        {
            var db = this.BaseRepository().BeginTrans();
            try
            {
                var entity = db.IQueryable().FirstOrDefault(x => x.CertificateId == id);
                if (entity != null)
                {
                    var list = db.IQueryable().Where(x => x.ParentId == entity.CertificateId);
                    foreach (var item in list)
                    {
                        db.Delete(item);
                    }
                    db.Delete(entity);
                }

                    db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id">父节点id</param>
        /// <returns></returns>
        public CertificateEntity selectLike(string name, string id)
        {
            var db = new RepositoryFactory().BaseRepository();
            var Certificategroups = from q1 in db.IQueryable<CertificateEntity>()
                                    join q2 in db.IQueryable<CertificateEntity>() on q1.ParentId equals q2.CertificateId
                                    where q2.CertificateId == id && q1.IsEffective
                                    select q1;
            return Certificategroups.FirstOrDefault(x => x.CertificateName == name);
        }

        /// <summary>
        /// 模糊查询索引框
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id">父节点id</param>
        /// <returns></returns>
        public IList<CertificateEntity> selectLikeList(string name, string id)
        {
            var db = new RepositoryFactory().BaseRepository();
            var parent = db.IQueryable<CertificateEntity>().Where(x => x.ParentId==null);
            var Certificategroups = from q1 in db.IQueryable<CertificateEntity>()
                                     join q2 in parent on q1.ParentId equals q2.CertificateId
                                     where q1.IsEffective
                                     select q1;
            if (string.IsNullOrEmpty(id))
            {
                var  one = parent.FirstOrDefault(x => x.CertificateName=="职业资格证");
                if (one!=null)
                {
                    Certificategroups = Certificategroups.Where(x => x.ParentId == one.CertificateId);
                }
            }
            if (!string.IsNullOrEmpty(name))
            {
                Certificategroups = Certificategroups.Where(x => x.CertificateName.Contains(name));
            }

            if (!string.IsNullOrEmpty(id))
            {
                Certificategroups = Certificategroups.Where(x => x.ParentId==id);
            }

            return Certificategroups.ToList();
        }

    }
}
