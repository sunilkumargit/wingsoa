using System;

namespace Wing.Server.Core
{
    public class BootstrapSettings
    {
        public String ServerAssemblyStorePath { get; set; }
        public String ClientAssemblyStorePath { get; set; }
        public String ServerDataBasePath { get; set; }
        public Uri ServicesBaseUri { get; set; }
    }
}