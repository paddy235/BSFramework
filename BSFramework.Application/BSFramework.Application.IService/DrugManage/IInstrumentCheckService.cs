using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.DrugManage;

namespace BSFramework.Application.IService.DrugManage
{
    public interface IInstrumentCheckService
    {
        IEnumerable<InstrumentCheckEntity> GetList();

        void SaveInstrument(string Id, InstrumentCheckEntity entity);

        InstrumentCheckEntity GetEntity(string keyValue);

        bool DelInstrument(InstrumentCheckEntity entity);
    }
}
