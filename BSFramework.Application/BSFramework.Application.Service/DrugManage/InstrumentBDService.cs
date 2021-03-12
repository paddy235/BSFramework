using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.DrugManage;
using BSFramework.Application.IService.DrugManage;
using BSFramework.Data.Repository;

namespace BSFramework.Application.Service.DrugManage
{
    public class InstrumentBDService : RepositoryFactory<InstrumentBDEntity>, IInstrumentBDService
    {
        public IEnumerable<InstrumentBDEntity> GetList()
        {
            var query = this.BaseRepository().IQueryable();
            return query.OrderByDescending(x => x.CreateDate).ToList();
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public InstrumentBDEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        public bool DelInstrument(InstrumentBDEntity entity)
        {
            if (this.BaseRepository().Delete(entity) > 0)
            {
                return true;
            }
            else return false;
        }
        public void SaveInstrument(string Id, InstrumentBDEntity entity)
        {
            var entity1 = this.GetEntity(Id);
            if (entity1 == null)
            {
                this.BaseRepository().Insert(entity);
            }
            else
            {
                entity1.BDDate = entity.BDDate;
                entity1.BDDept = entity.BDDept;
                entity1.BDDeptId = entity.BDDeptId;
                entity1.BDPeople = entity.BDPeople;
                entity1.BDPeopleId = entity.BDPeopleId;
                entity1.BDRemind = entity.BDRemind;
                entity1.BDResult = entity.BDResult;
                entity1.BeforeBDState = entity.BeforeBDState;

                entity1.Files = null;
                this.BaseRepository().Update(entity1);
            }

        }
    }
}
