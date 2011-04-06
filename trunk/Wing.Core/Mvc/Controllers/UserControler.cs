using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Wing.Pipeline;
using System.Web.Script.Serialization;

namespace Wing.Mvc.Controllers
{
    public class UserController : AbstractController
    {
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SaveProfile(String login, String name, String email)
        {
            return Json(new { success = true });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ChangePassword(String login, String current, String pwd)
        {
            return Json(new { success = true });
        }
    }
}
