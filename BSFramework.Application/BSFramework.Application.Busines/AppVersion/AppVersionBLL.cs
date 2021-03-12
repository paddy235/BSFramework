
using BSFramework.Application.Entity.AppVersionManage;
using BSFramework.Application.IService.AppVersionManage;
using BSFramework.Application.Service.AppVersionManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.AppVersion
{
    public class AppVersionBLL
    {
        private IAppVersionService service = new AppVersionService();

        public IEnumerable<AppVersionEntity> GetList()
        {
            return service.GetList();
        }
        public void SaveForm(string keyValue, AppVersionEntity entity)
        {
                service.SaveForm(keyValue, entity);

        }
    }
}
