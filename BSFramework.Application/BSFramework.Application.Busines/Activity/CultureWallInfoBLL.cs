using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.ClutureWallManage;
using BSFramework.Application.IService.Activity;
using BSFramework.Application.Service.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.Activity
{
    public class CultureWallInfoBLL
    {
        ICultureWallInfoService service = new CultureWallInfoService();
        public void SaveOrUpdate(CultureWallInfoEntity data)
        {
            service.SaveOrUpdate(data);
        }

        public CultureWallInfoEntity GetEntity(string bzId)
        {
            return service.GetEntity(bzId);
        }
    }
}
