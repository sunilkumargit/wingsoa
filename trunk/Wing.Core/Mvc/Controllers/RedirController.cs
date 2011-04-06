using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Wing.Mvc.Controllers
{
    public class RedirController : AbstractController
    {
        public ActionResult Index()
        {
            return RedirectToAction("Home");
        }

        public ActionResult Home()
        {
            return Redirect("/Shell");
        }
    }
}
