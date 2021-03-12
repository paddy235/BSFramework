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
    public class InstrumentCheckService : RepositoryFactory<InstrumentCheckEntity>, IInstrumentCheckService
    {
        public IEnumerable<InstrumentCheckEntity> GetList()
        {
            var query = this.BaseRepository().IQueryable();
            return query.OrderByDescending(x => x.CreateDate).ToList();
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public InstrumentCheckEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        public bool DelInstrument(InstrumentCheckEntity entity)
        {
            if (this.BaseRepository().Delete(entity) > 0)
            {
                return true;
            }
            else return false;
        }
        public void SaveInstrument(string Id, InstrumentCheckEntity entity)
        {
            var entity1 = this.GetEntity(Id);
            if (entity1 == null)
            {
                this.BaseRepository().Insert(entity);
            }
            else
            {
                entity1.CheckDate = entity.CheckDate;
                entity1.CheckDept = entity.CheckDept;
                entity1.CheckDeptId = entity.CheckDeptId;
                entity1.CheckPeople = entity.CheckPeople;
                entity1.CheckPeopleId = entity.CheckPeopleId;
                entity1.CheckRemind = entity.CheckRemind;
                entity1.CheckResult = entity.CheckResult;
                entity1.CheckValidate = entity.CheckValidate;

                entity1.Files = null;
                this.BaseRepository().Update(entity1);
            }

        }
    }
}
