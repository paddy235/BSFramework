using BSFramework.Application.Entity.ProducingCheck;
using BSFramework.Busines.ProducingCheck;
using BSFrameWork.Application.AppInterface.Models;
using BSFrameWork.Application.AppInterface.Models.ProducingCheck;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class CheckTemplateController : BaseApiController
    {
        private CheckTemplateBLL checkTemplateBLL = new CheckTemplateBLL();

        [HttpPost]
        public ListBucket<CheckTemplateEntity> List(ParamBucket<CheckTemplateQueryModel> paramBucket)
        {
            var total = 0;
            var data = checkTemplateBLL.Get(null, paramBucket.Data.DistrictId, paramBucket.Data.Key, paramBucket.PageSize, paramBucket.PageIndex, out total);
            return new ListBucket<CheckTemplateEntity> { Success = true, Data = data, Total = total };
        }
    }
}