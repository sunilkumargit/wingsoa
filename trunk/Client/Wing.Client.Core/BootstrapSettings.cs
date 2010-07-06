using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Reflection;

namespace Wing.Client.Core
{
    public class BootstrapSettings
    {
        public String ServerBaseAddress { get; set; }
        public String SoaEndpointAddressServiceUri { get; set; }
        public List<Assembly> Assemblies { get; set; }
    }
}
