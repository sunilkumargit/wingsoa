using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Wing.Soa.Interop.Client
{
    public class SoaDefaultChannelFactoryFactory : IChannelFactoryFactory
    {
        public Dictionary<String, SoaServiceConnectionInfo> ConnectionInfo { get; private set; }

        public SoaDefaultChannelFactoryFactory()
        {
            ConnectionInfo = new Dictionary<string, SoaServiceConnectionInfo>();
        }

        public ChannelFactory<TChannel> CreateChannelFactory<TChannel>()
        {
            var typeName = typeof(TChannel).Name;
            var lastDotIndex = typeName.LastIndexOf(".");
            var contractRef = "";
            if (lastDotIndex == -1)
                contractRef = typeName;
            else
                contractRef = typeName.Substring(lastDotIndex + 1);
            if (ConnectionInfo.ContainsKey(contractRef))
            {
                var info = ConnectionInfo[contractRef];
                return new ChannelFactory<TChannel>(new BasicHttpBinding(), new EndpointAddress(info.Address));
            }
            return null;
        }
    }
}
