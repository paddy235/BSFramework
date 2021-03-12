using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.SystemManage;
using System.Collections.Generic;

namespace BSFramework.Application.IService.SystemManage
{
    /// <summary>
    /// 描 述：区域管理
    /// </summary>
    public interface IDistrictPersonService
    {
        void Save(List<DistrictPersonEntity> entities);
        List<DistrictPersonEntity> GetList(string districtCode, string districtName, string category, string person, int rows, int page, out int total);
        List<DistrictPersonEntity> GetList(string id);
        void Remove(string id);
        List<UserEntity> GetDistrictPerson(string districtId);
        List<DistrictPersonEntity> GetDistricts();
    }
}
