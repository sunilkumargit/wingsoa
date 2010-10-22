using System;
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
