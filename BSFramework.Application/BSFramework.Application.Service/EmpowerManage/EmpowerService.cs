using BSFramework.Application.Entity.Empower;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.IService.EmpowerManage;

namespace BSFramework.Application.Service.EmpowerManage
{
    public class EmpowerService:IEmpowerService
    {
       
        public IEnumerable<ModelEntity> GetModels() 
        {
            IRepository<ModelEntity> db = new RepositoryFactory<ModelEntity>().BaseRepository();
            var query = db.IQueryable();
            return query.ToList();
        }

        public IEnumerable<EmpowerEntity> GetEmpowers(string deptid)
        {
            IRepository<EmpowerEntity> db = new RepositoryFactory<EmpowerEntity>().BaseRepository();
           // IRepository<EmpowerEntity> db1 = new Repository<EmpowerEntity>(DbFactory.Base()).BeginTrans();
            //IRepository db2 = new RepositoryFactory().BaseRepository();
            var query = db.IQueryable().Where(x => x.DepartId == deptid);
            return query.ToList();
        }

        public IEnumerable<DepartEntity> GetDeparts()
        {
            IRepository<DepartEntity> db = new RepositoryFactory<DepartEntity>().BaseRepository();
            var query = db.IQueryable();
            return query.ToList();
        }

        public ModelEntity GetModelEntity(string id)
        {
            IRepository<ModelEntity> db = new RepositoryFactory<ModelEntity>().BaseRepository();
            return db.FindEntity(id);
        }

        public EmpowerEntity GetEmpowerEntity(string id) 
        {
            IRepository<EmpowerEntity> db = new RepositoryFactory<EmpowerEntity>().BaseRepository();
            return db.FindEntity(id);
        }

        public DepartEntity GetDepartEntity(string id) 
        {
            IRepository<DepartEntity> db = new RepositoryFactory<DepartEntity>().BaseRepository();
            return db.FindEntity(id);
        }

        public void SaveModel(string id, ModelEntity entity) 
        {
            IRepository<ModelEntity> db = new RepositoryFactory<ModelEntity>().BaseRepository();
            ModelEntity entity1 = this.GetModelEntity(id);
            if (entity1 == null)
            {
                db.Insert(entity);
            }
            else 
            {
                entity1.Code = entity.Code;
                entity1.DeleteMark = entity.DeleteMark;
                entity1.Name = entity.Name;
                entity1.NodeLevel = entity.NodeLevel;
                entity1.ParentId = entity.ParentId;
                entity1.ParentName = entity.ParentName;
                entity1.Remark = entity.Remark;
                entity1.Sort = entity.Sort;
                db.Update(entity1);
            }
        }

        public void SaveDepart(string id, DepartEntity entity)
        {
            IRepository<DepartEntity> db = new RepositoryFactory<DepartEntity>().BaseRepository();
            DepartEntity entity1 = this.GetDepartEntity(id);
            if (entity1 == null)
            {
                db.Insert(entity);
            }
            else
            {
                entity1.AndroidVersion = entity.AndroidVersion;
                entity1.EmpowerDate = entity.EmpowerDate;
                entity1.Name = entity.Name;
                entity1.TypeId = entity.TypeId;
                entity1.TypeName = entity.TypeName;
                entity1.RegisterCode = entity.RegisterCode;
                entity1.Sort = entity.Sort;
                db.Update(entity1);
            }
        }

        public void SaveEmpower(string id, EmpowerEntity entity)
        {
            IRepository<EmpowerEntity> db = new RepositoryFactory<EmpowerEntity>().BaseRepository();
            EmpowerEntity entity1 = this.GetEmpowerEntity(id);
            if (entity1 == null)
            {
                db.Insert(entity);
            }
            else
            {
                
                db.Update(entity1);
            }
        }

        public void DeleteModel(string id) 
        {

        }

        public void DeleteDepart(string id) 
        {

        }

        public void DeleteEmpower(string id) 
        {

        }
    }
}
