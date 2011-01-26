using System.Web.Mvc;

namespace Wing.Server.Host.Controllers
{
    public class LoaderController : Controller
    {
        public ActionResult Index()
        {
            return View("WingLoader");
        }

        public ActionResult Test()
        {
            return View("ServerTest");
        }
    }
}
