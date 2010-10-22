using System;

namespace Wing.Soa.Interop
{
    public class SoaServiceConnectionInfo
    {
        public String ServiceName { get; set; }
        public Uri Address { get; set; }
        public String ContractRef { get; set; }
        public SoaServiceBindingMode ServiceBindingMode { get; set; }
    }
}
