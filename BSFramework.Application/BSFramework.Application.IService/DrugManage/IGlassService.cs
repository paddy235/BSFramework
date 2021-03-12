using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.DrugManage;

namespace BSFramework.Application.IService.DrugManage
{
    public interface IGlassService
    {
        IEnumerable<GlassEntity> GetList();

        IEnumerable<GlassEntity> GetPageList(string name, string deptid, int page, int pagesize, out int total);
        void SaveGlass(string Id, GlassEntity entity);

        GlassEntity GetEntity(string keyValue);

        bool DelGlass(GlassEntity entity);
    }
}
