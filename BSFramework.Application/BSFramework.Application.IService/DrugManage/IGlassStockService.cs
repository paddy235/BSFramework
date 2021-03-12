using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.DrugManage;

namespace BSFramework.Application.IService.DrugManage
{
    public interface IGlassStockService
    {
        IEnumerable<GlassStockEntity> GetList(string deptid);

        IEnumerable<GlassStockEntity> GetPageList(DateTime? from, DateTime? to, string name, string deptid, string glasid, int page, int pagesize, out int total);
        void SaveGlassStock(string Id, GlassStockEntity entity);

        GlassStockEntity GetEntity(string keyValue);
    }
}
