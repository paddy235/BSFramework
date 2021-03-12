using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.DrugManage;
using System.Data;
using BSFramework.Util.WebControl;

namespace BSFramework.Application.IService.DrugManage
{
    public interface IDrugInventoryService
    {
        IEnumerable<DrugInventoryEntity> GetList();

        IEnumerable<DrugGlassWareEntity> GetDrugGlassWareList();
        
        IEnumerable<DrugInventoryEntity> GetPageList(string name, string deptid, int page, int pagesize, out int total);

        List<DrugInventoryEntity> GetDrugPageList(string usercode, Pagination pagination, string queryJson);
        void SaveDrugInventory(string Id, DrugInventoryEntity entity);
        void SaveDrugGlassWare(string Id, DrugGlassWareEntity entity);
        DrugInventoryEntity GetEntity(string keyValue);

        DrugGlassWareEntity GetDrugGlassWareEntity(string keyValue);
        bool DelDrugInventory(DrugInventoryEntity entity);

        bool DeleteGlassWare(DrugGlassWareEntity entity); 
    }
}
