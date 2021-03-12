using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.SystemManage;

namespace BSFramework.Application.IService.SystemManage
{
    public interface IIndexManageService
    {
        List<IndexManageEntity> GetList(string deptId, int indexType, string keyWord, int? templet);
        void SaveForm(string keyValue, IndexManageEntity entity);
        IndexManageEntity GetForm(string keyValue);
        void Remove(string keyValue);
    }
}
