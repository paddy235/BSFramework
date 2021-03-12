using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Util;
using BSFrameWork.Application.AppInterface.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class DistrictController : BaseApiController
    {
        private DistrictPersonBLL districtPersonBLL = new DistrictPersonBLL();

        [HttpPost]
        public ListBucket<DistrictModel> GetDistrict(ParamBucket<string> paramBucket)
        {
            //var baseUrl = Config.GetValue("ErchtmsApiUrl");
            //var client = new HttpClient();
            //var param = new { Data = new { companyId = paramBucket.Data } };
            //var requestContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(param));
            //requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            //var json = client.PostAsync(baseUrl + "District/GetDistrict", requestContent).Result.Content.ReadAsStringAsync().Result;
            //var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DistrictModel>>(json);
            //return new ListBucket<DistrictModel> { Success = true, Data = data };


            var data = districtPersonBLL.GetDistricts();
            return new ListBucket<DistrictModel> { Success = true, Data = data.Select(x => new DistrictModel { DistrictID = x.DistrictId, DistrictCode = x.DistrictCode, DistrictName = x.DistrictName, BelongCompany = x.CompanyName }).ToList() };
        }

        [HttpPost]
        public ListBucket<UserEntity> GetDistrictPerson(ParamBucket<string> paramBucket)
        {
            var persons = districtPersonBLL.GetDistrictPerson(paramBucket.Data);
            return new ListBucket<UserEntity> { Data = persons, Success = true };
        }
    }
}
