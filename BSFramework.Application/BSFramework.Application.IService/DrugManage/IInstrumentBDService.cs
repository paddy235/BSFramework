using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.DrugManage;

namespace BSFramework.Application.IService.DrugManage
{
    public interface IInstrumentBDService
    {
        IEnumerable<InstrumentBDEntity> GetList();

        void SaveInstrument(string Id, InstrumentBDEntity entity);

        InstrumentBDEntity GetEntity(string keyValue);

        bool DelInstrument(InstrumentBDEntity entity);
    }
}
