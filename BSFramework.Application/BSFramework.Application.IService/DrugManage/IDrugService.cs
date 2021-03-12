using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.DrugManage;

namespace BSFramework.Application.IService.DrugManage
{
    public interface IDrugService
    {
        IEnumerable<DrugEntity> GetList(string deptid);

        IEnumerable<DrugEntity> GetPageList(string name, string deptid, int page, int pagesize, out int total);
        void SaveDrug(string Id, DrugEntity entity);
       
        DrugEntity GetEntity(string keyValue);

        bool DelDrug(DrugEntity entity);
        void AddDrugStockOut2(DrugOutEntity data, float used);
        void Update(List<DrugEntity> drugs);
        void Import(List<DrugEntity> list);
    }
}
