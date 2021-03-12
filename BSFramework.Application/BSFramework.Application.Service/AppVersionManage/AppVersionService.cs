using BSFramework.Application.Entity.AppVersionManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.IService.AppVersionManage;
using System.Data;

namespace BSFramework.Application.Service.AppVersionManage
{
    public class AppVersionService : RepositoryFactory<AppVersionEntity>, IAppVersionService
    {
        public IEnumerable<AppVersionEntity> GetList()
        {
            var query = this.BaseRepository().IQueryable();
            return query.OrderByDescending(x => x.CreateDate).ToList();
        }

        public void SaveForm(string keyValue, AppVersionEntity entity)
        {

                this.BaseRepository().Insert(entity);
        }
    }
}
