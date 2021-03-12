using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.SafetyScore
{
    public class SafetyScoreAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SafetyScore";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SafetyScore_default",
                "SafetyScore/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}