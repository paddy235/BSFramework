using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.ClutureWallManage;

namespace BSFramework.Application.IService.Activity
{
    public interface ICultureWallInfoService
    {
        void SaveOrUpdate(CultureWallInfoEntity data);

        CultureWallInfoEntity GetEntity(string bzId);
    }
}
