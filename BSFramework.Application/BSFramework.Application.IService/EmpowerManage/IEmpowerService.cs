using BSFramework.Application.Entity.Empower;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Data;

namespace BSFramework.Application.IService.EmpowerManage
{
    public interface IEmpowerService
    {
        IEnumerable<ModelEntity> GetModels();

        IEnumerable<EmpowerEntity> GetEmpowers(string deptid);

        IEnumerable<DepartEntity> GetDeparts();

        ModelEntity GetModelEntity(string id);

        EmpowerEntity GetEmpowerEntity(string id);

        DepartEntity GetDepartEntity(string id);

        void SaveModel(string id, ModelEntity entity);

        void SaveDepart(string id, DepartEntity entity);

        void SaveEmpower(string id, EmpowerEntity entity);

        void DeleteModel(string id);

        void DeleteDepart(string id);

        void DeleteEmpower(string id);
    }
}
