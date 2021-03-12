using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.DrugManage;

namespace BSFramework.Application.IService.DrugManage
{
    public interface IInstrumentService
    {
        IEnumerable<InstrumentEntity> GetList();

        void SaveInstrument(string Id, InstrumentEntity entity);

        InstrumentEntity GetEntity(string keyValue);

        bool DelInstrument(InstrumentEntity entity);
    }
}
