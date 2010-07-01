using System;
using System.ServiceModel;
using System.Collections;
using System.Collections.Generic;

namespace Wing.Soa
{
    public interface ISoaServicesProvider
    {
        Uri BaseAdress { get; set; }
        SoaServiceHostInfo RegisterService<TContract, TService>(string serviceName, string baseAddress);
        SoaServiceHostInfo RegisterService<TContract, TService>();
        IEnumerable<SoaServiceHostInfo> GetRegisteredServices();
    }

    public class SoaServiceHostInfo
    {
        internal SoaServiceHostInfo()
        {
        }

        public ServiceHost Host { get; internal set; }
    }
}
