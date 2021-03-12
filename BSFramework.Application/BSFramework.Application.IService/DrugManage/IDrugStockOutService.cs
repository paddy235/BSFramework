using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.DrugManage;

namespace BSFramework.Application.IService.DrugManage
{
    public interface IDrugStockOutService
    {
        void SaveDrugStockOut(string Id, DrugStockOutEntity entity);
        DrugStockOutEntity GetEntity(string keyValue);
        IEnumerable<DrugStockOutEntity> GetStockOutList(string deptid, string name, string level);
        bool DelDrugStcokOut(DrugStockOutEntity entity);
    }
}
