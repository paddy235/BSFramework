using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.IService.SystemManage;
using BSFramework.Data.Repository;
using BSFramework.Util.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.SystemManage
{
    public class IndexAssocationService : RepositoryFactory<IndexAssocationEntity>, IIndexAssocationService
    {
        public List<IndexAssocationEntity> GetList(string deptId)
        {
            return this.BaseRepository().IQueryable(x => x.DeptId == deptId).ToList();
        }

        public List<IndexAssocationEntity> GetListByTitleId(params string[] titleId)
        {
                Expression<Func<IndexAssocationEntity, bool>> expression = x => x.Id == null;
            if (titleId != null && titleId.Count() > 0)
            {
                foreach (var item in titleId)
                {
                    expression = expression.Or(x => x.TitleId == item);
                }
            }
            return this.BaseRepository().IQueryable().Where(expression).ToList();
        }

        public void Insert(List<IndexAssocationEntity> addEntitys)
        {
            this.BaseRepository().Insert(addEntitys);
        }

        public void Remove(List<IndexAssocationEntity> delEntity)
        {
            this.BaseRepository().Delete(delEntity);
        }
    }
}
