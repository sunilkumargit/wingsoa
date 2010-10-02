using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Wing.Soa.Interop
{
    public class SoaServiceConnectionInfo
    {
        public Uri Address { get; set; }
        public SoaServiceBindingMode ServiceBindingMode { get; set; }
    }
}
