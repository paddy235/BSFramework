using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.IService.SystemManage;
using BSFramework.Application.Service.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.SystemManage
{
   public  class IIndexAssocationBLL
    {
        private IIndexAssocationService service = new IndexAssocationService();

        public List<IndexAssocationEntity> GetList(string deptId)
        {
            return service.GetList(deptId);

        }

        public List<IndexAssocationEntity> GetListByTitleId(params string[] titleId)
        {
            return service.GetListByTitleId(titleId);
        }

        public void Remove(List<IndexAssocationEntity> delEntity)
        {
            service.Remove(delEntity);
        }

        public void Insert(List<IndexAssocationEntity> addEntitys)
        {
            service.Insert(addEntitys);
        }
    }
}
