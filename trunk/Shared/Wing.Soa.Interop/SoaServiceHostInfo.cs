using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Wing.Soa.Interop
{
    [DataContract]
    public class SoaServiceHostInfo
    {
        [DataMember]
        public String ServiceName { get; set; }

        [DataMember]
        public SoaServiceState State { get; set; }

        [DataMember]
        public Uri DefaultAddress { get; set; }
    }
}
