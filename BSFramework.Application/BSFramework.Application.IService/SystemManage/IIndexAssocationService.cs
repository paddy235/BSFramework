using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.SystemManage;

namespace BSFramework.Application.IService.SystemManage
{
    public interface IIndexAssocationService
    {
        List<IndexAssocationEntity> GetList(string deptId);
        List<IndexAssocationEntity> GetListByTitleId(params string[] titleId);
        void Remove(List<IndexAssocationEntity> delEntity);
        void Insert(List<IndexAssocationEntity> addEntitys);
    }
}
