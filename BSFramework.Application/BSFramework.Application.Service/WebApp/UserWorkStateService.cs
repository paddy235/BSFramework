using BSFramework.Application.Entity.WebApp;
using BSFramework.Application.IService.WebApp;
using BSFramework.Application.Service.WebApp;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.WebApp
{
  public  class UserWorkStateService : RepositoryFactory<UserWorkStateEntity>, IUserWorkStateService
    {

        /// <summary>
        ///人员状态
        /// </summary>
        public void addState(UserWorkStateEntity entity)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                 
                if (string.IsNullOrEmpty(entity.Id))
                {
                    entity.Create();
                    db.Insert(entity);
                }
                else {
                    db.Update(entity);
                }
               
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

        }

        /// <summary>
        ///获取人员状态
        /// </summary>
        public UserWorkStateEntity selectState(string userid)
        {
            return this.BaseRepository().IQueryable().FirstOrDefault(x=>x.userId==userid);

        }
    }
}
