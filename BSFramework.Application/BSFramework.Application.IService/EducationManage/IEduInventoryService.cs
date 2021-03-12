using BSFramework.Application.Entity.EducationManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BSFramework.Application.IService.EducationManage
{
    public interface IEduInventoryService
    {
        IEnumerable<EduInventoryEntity> GetList(string deptcode, string ids, string type, string name, int pageSize, int pageIndex, out int total);
        EduInventoryEntity GetEntity(string keyValue);
        void RemoveForm(string keyValue);
        void SaveForm(string keyValue, EduInventoryEntity entity);
        EduInventoryEntity GetRadEntity(string deptCode);
    }
}
