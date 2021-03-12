using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Util.WebControl;

namespace BSFramework.Application.IService.SystemManage
{
    public interface IWorkStandardService
    {
        List<WorkStandardEntity> GetPageList(Pagination pagination, string queryJson);
        WorkStandardEntity GetEntity(string keyValue);
        void SaveForm(string keyValue, WorkStandardEntity entity);
        void RemoveForm(string keyValue);
        List<WorkStandardEntity> GetAllList();
    }
}
