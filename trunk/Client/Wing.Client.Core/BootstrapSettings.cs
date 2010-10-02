using System;
using System.Collections.Generic;
using System.Reflection;

namespace Wing.Client.Core
{
    public class BootstrapSettings
    {
        public Uri ServerBaseAddress { get; set; }
        public Uri SoaMetadataProviderUri { get; set; }
        public IRootVisualManager RootVisualManager { get; set; }
        public List<Assembly> Assemblies { get; set; }
        public ISplashUI Splash { get; set; }
    }
}
