using System.Web.Mvc;

namespace Wing.Server.Host.Controllers
{
    public class LoaderController : Controller
    {
        public ActionResult Index()
        {
            return View("WingLoader");
        }

        public ActionResult GetInfo()
        {
            return Content("teste de conteudo", "text/plain");
        }
    }
}
