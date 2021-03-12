using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.IService.SystemManage;
using BSFramework.Application.Service.SystemManage;
using BSFramework.Cache.Factory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSFramework.Application.Busines.SystemManage
{
    /// <summary>
    /// 描 述：区域责任人
    /// </summary>
    public class DistrictPersonBLL
    {
        private IDistrictPersonService districtPersonService = new DistrictPersonService();
        public void Save(List<DistrictPersonEntity> entities)
        {
            districtPersonService.Save(entities);
        }

        public List<DistrictPersonEntity> GetList(string districtCode, string districtName, string category, string person, int rows, int page, out int total)
        {
            return districtPersonService.GetList(districtCode, districtName, category, person, rows, page, out total);
        }

        public List<DistrictPersonEntity> GetList(string id)
        {
            return districtPersonService.GetList(id);
        }

        public void Remove(string id)
        {
            districtPersonService.Remove(id);
        }

        public List<UserEntity> GetDistrictPerson(string districtId)
        {
            return districtPersonService.GetDistrictPerson(districtId);
        }

        public List<DistrictPersonEntity> GetDistricts()
        {
            return districtPersonService.GetDistricts();
        }
    }
}
