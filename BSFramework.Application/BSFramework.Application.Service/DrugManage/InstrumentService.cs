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
    public class InstrumentService : RepositoryFactory<InstrumentEntity>, IInstrumentService
    {
        public IEnumerable<InstrumentEntity> GetList()
        {
            var query = this.BaseRepository().IQueryable();
            return query.OrderByDescending(x => x.CreateDate).ToList();
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public InstrumentEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        public bool DelInstrument(InstrumentEntity entity)
        {
            if (this.BaseRepository().Delete(entity) > 0)
            {
                return true;
            }
            else return false;
        }
        public void SaveInstrument(string Id, InstrumentEntity entity)
        {
            var entity1 = this.GetEntity(Id);
            if (entity1 == null)
            {
                this.BaseRepository().Insert(entity);
            }
            else
            {
                entity1.Amount = entity.Amount;
                entity1.BuyDate = entity.BuyDate;
                entity1.CheckDept = entity.CheckDept;
                entity1.CheckDeptId = entity.CheckDeptId;
                entity1.CheckResult = entity.CheckResult;
                entity1.Cycle = entity.Cycle;
                entity1.DutyPeople = entity.DutyPeople;
                entity1.DutyPeopleId = entity.DutyPeopleId;
                entity1.Factory = entity.Factory;
                entity1.GlassWareId = entity.GlassWareId;
                entity1.Name = entity.Name;
                entity1.Number = entity.Number;
                entity1.Path = entity.Path;
                entity1.Remind = entity.Remind;
                entity1.Spec = entity.Spec;
                entity1.Validate = entity.Validate;
                entity1.BDCycle = entity.BDCycle;
                entity1.BDRemind = entity.BDRemind;
                entity1.BDValidate = entity.BDValidate;
                entity1.BDResult = entity.BDResult;
                entity1.Files = null;
                this.BaseRepository().Update(entity1);
            }

        }
    }
}
