using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Wing.Security;

namespace Wing.Mvc.Controllers
{
    public class LoginController : AbstractController
    {
        public ActionResult Index(String usr, String pwd, String token, String fwd)
        {
            return RedirectToAction("Logon", new { usr = usr, pwd = pwd, token = token, fwd = fwd });
        }

        private void DecodeToken(String token, ref string usr, ref string pwd, ref string fwd)
        {
            var dic = new Dictionary<String, String>();
            dic.SetToken(token);
            usr = dic["usr"];
            pwd = dic["pwd"];
            if (fwd.IsEmpty())
                dic.TryGetValue("fwd", out fwd);
        }

        public ActionResult Logon(String usr, String pwd, String token, String fwd)
        {
            if (!token.IsEmpty())
                DecodeToken(token, ref usr, ref pwd, ref fwd);
            else if (usr.IsEmpty())
                return RedirectToAction("LoginView", new { fwd = fwd });
            var tokenDic = new Dictionary<String, String>();
            tokenDic["usr"] = usr.AsString();
            tokenDic["pwd"] = pwd.AsString();
            tokenDic["fwd"] = fwd.AsString();
            token = tokenDic.GetToken();

            var authService = ServiceLocator.GetInstance<IAuthenticationService>();
            var msg = "";
            if (!authService.PerformLogin(usr, pwd, out msg))
                return RedirectToAction("LoginError", new { token = token, msg = msg });
            return RedirectToAction("LoginSuccess", new { token = token });
        }

        public ActionResult LoginView(String fwd, String msg, String token)
        {
            var usr = "";
            var pwd = "";
            if (token.HasValue())
                DecodeToken(token, ref usr, ref pwd, ref fwd);
            ViewData["usr"] = usr;
            ViewData["pwd"] = pwd;
            ViewData["fwd"] = fwd;
            ViewData["msg"] = msg;
            ViewData["token"] = token;
            return View("Login");
        }

        public ActionResult LoginError(String token, String msg)
        {
            return RedirectToAction("LoginView", new { token = token, msg = msg });
        }

        public ActionResult LoginSuccess(String token)
        {
            var dic = new Dictionary<String, String>();
            dic.SetToken(token);
            var fwd = "";
            dic.TryGetValue("fwd", out fwd);
            if (fwd.HasValue())
                return Redirect(fwd);
            return RedirectToAction("Index", "Shell");
        }

        public ActionResult Signout()
        {
            ServiceLocator.GetInstance<IAuthenticationService>().PerformLogout();
            return RedirectToAction("Index");
        }
    }
}
