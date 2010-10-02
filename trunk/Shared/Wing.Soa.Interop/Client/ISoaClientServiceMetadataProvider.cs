using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Wing.Soa.Interop.Client
{
    public interface ISoaClientServiceMetadataProvider
    {
        void GetServiceConnectionInfoByContractType(Type contractType, Action<SoaServiceConnectionInfo> callback);
    }
}
