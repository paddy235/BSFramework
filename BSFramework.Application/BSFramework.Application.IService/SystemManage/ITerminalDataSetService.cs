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
    public interface ITerminalDataSetService
    {
        List<TerminalDataSetEntity> GetPageList(Pagination pagination, string queryJson, string dataSetType);
        void SaveForm(string keyValue, TerminalDataSetEntity ds);
        TerminalDataSetEntity GetEntity(string keyValue);
        void RemoveForm(string keyValue);
        List<TerminalDataSetEntity> GetList();
    }
}
