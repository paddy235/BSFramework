using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using BSFramework.Application.Web.Areas.Works.Views.EduTrain;
using BSFramework.Util;
using BSFramework.Application.Code;
using BSFramework.Application.Busines.BaseManage;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    public class EduTrainController : MvcControllerBase
    {
        public ActionResult Index(FormCollection fc)
        {
            var user = OperatorProvider.Provider.Current();
            var userAccount = (new UserBLL()).GetTrainUser(user.UserId);
            string newKey = "!2#3@1YV";
            string newIV = "A~we!S6d";
            Security sec = new Security(newKey, newIV);
            //string userAccount = Config.GetValue("userAccount");
            string valCode = sec.Encrypt(userAccount, newKey, newIV);
            ViewData["valCode"] = valCode;
            ViewData["EduPageUrl"] = Config.GetValue("EduPageUrl");
            return View();
        }

        public ActionResult NewIndex(FormCollection fc)
        {
            var user = OperatorProvider.Provider.Current();
            var userAccount = (new UserBLL()).GetTrainUser(user.UserId);

            string newKey = "!2#3@1YV";
            string newIV = "A~we!S6d";
            Security sec = new Security(newKey, newIV);
            //string userAccount = Config.GetValue("userAccount");
            string valCode = sec.Encrypt(userAccount, newKey, newIV);
            ViewData["valCode"] = valCode;
            ViewData["EduPageUrl"] = Config.GetValue("EduPageUrl");
            return View();
        }
    }
}
