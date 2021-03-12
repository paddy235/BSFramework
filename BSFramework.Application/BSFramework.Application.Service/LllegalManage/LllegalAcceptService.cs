using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.LllegalManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.LllegalManage;
using BSFramework.Data.Repository;

namespace BSFramework.Application.Service.LllegalManage
{
    public class LllegalAcceptService : RepositoryFactory<LllegalAcceptEntity>, IlllegalAcceptService
    {
        public void SaveFrom(LllegalAcceptEntity entity)
        {
            try
            {
                if (GetEntityByLllegalId(entity.LllegalId) != null)
                {
                    var obj = GetEntityByLllegalId(entity.LllegalId);
                    obj.AcceptMind = entity.AcceptMind;
                    obj.AcceptPeople = entity.AcceptPeople;
                    obj.AcceptPeopleId = entity.AcceptPeopleId;
                    obj.AcceptResult = entity.AcceptResult;
                    obj.AcceptTime = entity.AcceptTime;
                    obj.Files = null;
                    this.BaseRepository().Update(obj);
                }
                else
                {
                    entity.Id = Guid.NewGuid().ToString();
                    this.BaseRepository().Insert(entity);
                }

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        public LllegalAcceptEntity GetEntity(string key)
        {
            LllegalAcceptEntity refromEntity = this.BaseRepository().FindEntity(key);
            if (refromEntity != null)
            {
                refromEntity.Files = new Repository<FileInfoEntity>(DbFactory.Base()).IQueryable().Where(x => x.RecId == key).ToList();
            }
            return refromEntity;
        }

        public LllegalAcceptEntity GetEntityByLllegalId(string id)
        {
            var query = this.BaseRepository().IQueryable().Where(x => x.LllegalId == id);
            if (query.Count() > 0)
            {
                var entity = query.ToList().SingleOrDefault();
                entity.Files = new Repository<FileInfoEntity>(DbFactory.Base()).IQueryable().Where(x => x.RecId == entity.Id).ToList();
                return entity;
            }
            else
            {
                return null;
            }
        }
    }
}
