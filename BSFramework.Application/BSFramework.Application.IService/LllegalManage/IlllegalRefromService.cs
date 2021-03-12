using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.LllegalManage;

namespace BSFramework.Application.IService.LllegalManage
{
    public interface IlllegalRefromService
    {
        void SaveFrom(LllegalRefromEntity entity);
        LllegalRefromEntity GetEntity(string key);
        LllegalRefromEntity GetEntityByLllegalId(string id);
    }
}
