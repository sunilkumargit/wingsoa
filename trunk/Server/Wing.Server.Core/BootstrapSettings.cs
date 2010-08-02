using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Wing.Server.Core
{
    public class BootstrapSettings
    {
        public String ServerAssemblyStorePath { get; set; }
        public String ClientAssemblyStorePath { get; set; }
        public String ServerDataBasePath { get; set; }
    }
}