using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.SetManage
{
    public class SetManageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SetManage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
              this.AreaName + "_Default",
              this.AreaName + "/{controller}/{action}/{id}",
              new { area = this.AreaName, controller = "Home", action = "Index", id = UrlParameter.Optional },
              new string[] { "BSFramework.Application.Web.Areas." + this.AreaName + ".Controllers" }
            );
        }
    }
}
