using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.IService.SystemManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.SystemManage
{
    public class IndexManageService : RepositoryFactory<IndexManageEntity>, IIndexManageService
    {
        public IndexManageEntity GetForm(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }

        public List<IndexManageEntity> GetList(string deptId, int indexType, string keyWord, int? templet)
        {
            var where = this.BaseRepository().IQueryable(x => x.DeptId == deptId && x.IndexType==indexType);
            if (!string.IsNullOrWhiteSpace(keyWord))
            {
                where = where.Where(p => p.Title.Contains(keyWord));
            }
            if (templet.HasValue)
            {
                if (templet == 1)//如果为第一套，则取Temple为1 或者该字段为空的数据
                    where = where.Where(x => x.Templet == null || x.Templet == 1);
                else
                    where = where.Where(x => x.Templet == templet);
            }
  
            
            return where.OrderBy(p=>p.Sort).ToList();
        }

        public void Remove(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }

        public void SaveForm(string keyValue, IndexManageEntity entity)
        {
            if (string.IsNullOrWhiteSpace(keyValue))
            {
                entity.Create();
                this.BaseRepository().Insert(entity);
            }
            else
            {
                entity.Modify(keyValue);
                this.BaseRepository().Update(entity);
            }
        }
    }
}
