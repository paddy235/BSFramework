using BSFramework.Application.Entity.AppVersionManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.AppVersionManage
{
    public interface IAppVersionService
    {
        IEnumerable<AppVersionEntity> GetList();

        void SaveForm(string keyValue, AppVersionEntity entity);
    }
}
