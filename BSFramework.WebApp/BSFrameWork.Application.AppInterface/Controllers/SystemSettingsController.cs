using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Entity.Activity;
using BSFramework.Busines.WorkMeeting;
using BSFrameWork.Application.AppInterface.Models;
using System.Configuration;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class SystemSettingsController : BaseApiController
    {

        public SystemSettingsController()
        {
        }

        [HttpPost]
        public ResultBucket GetSetting(ParamBucket<string> paramBucket)
        {
            var category = ConfigurationManager.AppSettings[paramBucket.Data].ToString();
            return new ModelBucket<string> { Success = true, Data = category };
        }
    }
}