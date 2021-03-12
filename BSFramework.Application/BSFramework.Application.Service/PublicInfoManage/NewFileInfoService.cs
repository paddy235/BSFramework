using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.PublicInfoManage;
using BSFramework.Data;
using BSFramework.Data.Repository;
using BSFramework.Util.Extension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Linq;
using System.Collections;
using BSFramework.Application.Entity.BaseManage;
using System.IO;

namespace BSFramework.Application.Service.PublicInfoManage
{
    public class NewFileInfoService : RepositoryFactory<NewFileInfoEntity>, INewFileInfoService
    {
        public NewFileInfoEntity GetEntity(string id) 
        {
            return this.BaseRepository().FindEntity(id);
        }
        public IEnumerable<NewFileInfoEntity> GetList(string recid) 
        {
            var query = this.BaseRepository().IQueryable().Where(x => x.RecId == recid);
            return query.ToList();

        }

        public void SaveForm(string keyValue, NewFileInfoEntity entity) 
        {
            var entity1 = this.GetEntity(keyValue);
            if (entity1 == null)
            {
                if (string.IsNullOrEmpty(entity.ID))
                {
                    entity.ID = Guid.NewGuid().ToString();
                }
                this.BaseRepository().Insert(entity);
            }
        }
    }
}
