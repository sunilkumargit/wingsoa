using System;
using System.Web.Mvc;
using Wing.Server.Core;
using Wing.ServiceLocation;
using Wing.Utils;

namespace Wing.Server.Modules.WingClientSupport
{
    public class WingCltAppSupportController : Controller
    {
        public ActionResult GetAssembliesMetadata()
        {
            var info = ServiceLocator.Current.GetInstance<AssemblyInfoCollection>("Client");
            return Content(info.SerializeToXml(), "text/xml");
        }

        public ActionResult GetAssemblyData(String file)
        {
            var store = ServiceLocator.Current.GetInstance<IAssemblyStore>("Client");
            store.ConsolidateStore();
            var data = store.GetAssemblyData(file);
            return File(data, "application/binary");
        }
    }
}
