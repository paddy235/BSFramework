using BSFramework.Application.Entity.PublicInfoManage.ViewMode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.PublicInfoManage
{
   public interface IAdminPrettyService
    {
        List<KeyValue> FindCount(string deptcode);
        List<KeyValue> FindCount1(string deptId,string TerminalType);
        List<KeyValue> FindBZAllCount(Dictionary<string, string> keyValuePairs, List<string> deptIds);
    }
}
