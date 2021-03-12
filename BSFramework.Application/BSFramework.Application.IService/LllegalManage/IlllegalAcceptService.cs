using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.LllegalManage;
namespace BSFramework.Application.IService.LllegalManage
{
    public interface IlllegalAcceptService
    {
        void SaveFrom(LllegalAcceptEntity entity);
        LllegalAcceptEntity GetEntity(string key);

        LllegalAcceptEntity GetEntityByLllegalId(string id);
    }
}
