using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.DrugManage;

namespace BSFramework.Application.IService.DrugManage
{
    public interface IDrugStockService
    {
        void SaveDrugStock(string Id, DrugStockEntity entity);
        DrugStockEntity GetEntity(string keyValue);

        IEnumerable<DrugStockEntity> GetStockList(string[] deptid, DateTime? from, DateTime? to, string name, int page, int pagesize, out int total);
    }
}
