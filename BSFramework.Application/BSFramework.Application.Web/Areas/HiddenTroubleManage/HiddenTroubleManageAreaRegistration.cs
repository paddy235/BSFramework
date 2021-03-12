using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.HiddenTroubleManage
{
    public class HiddenTroubleManageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "HiddenTroubleManage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "HiddenTroubleManage_default",
                "HiddenTroubleManage/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
