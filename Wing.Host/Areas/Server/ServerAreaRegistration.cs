using System.Web.Mvc;

namespace Wing.Host.Areas.Server
{
    public class ServerAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Server";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Server_default",
                "Server/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
