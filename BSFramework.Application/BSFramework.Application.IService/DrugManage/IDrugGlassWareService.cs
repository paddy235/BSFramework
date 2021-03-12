using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.DrugManage;
using BSFramework.Util.WebControl;

namespace BSFramework.Application.IService.DrugManage
{
    public interface IDrugGlassWareService
    {
        IEnumerable<DrugGlassWareEntity> GetPageList(Pagination pagination, string queryJson, string type);
    }
}
