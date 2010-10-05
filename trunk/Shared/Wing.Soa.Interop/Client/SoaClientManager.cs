using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Wing.Soa.Interop.Client
{
    public static class SoaClientManager
    {
        private static ISoaClientServiceMetadataProvider _metadaProvider;
        private static Dictionary<Type, ChannelFactory> _channelFactories = new Dictionary<Type, ChannelFactory>();
        private static Object _lockObject = new Object();

        public static void SetMetadataProvider(ISoaClientServiceMetadataProvider metadataProvider)
        {
            _metadaProvider = metadataProvider;
        }

        public static TChannel CreateChannel<TChannel>()
        {
            lock (_lockObject)
            {
                if (!_channelFactories.ContainsKey(typeof(TChannel)))
                {
                    _metadaProvider.GetServiceConnectionInfoByContractType(typeof(TChannel), (connInfo) =>
                    {
                        if (connInfo == null)
                            throw new Exception("Service reference does not found. " + typeof(TChannel).FullName);
                        var binding = CreateBinding(connInfo);
                        var endPoint = CreateEndpoint(connInfo);
                        _channelFactories[typeof(TChannel)] = new ChannelFactory<TChannel>(binding, endPoint);
                    });
                }
            }
            var factory = (ChannelFactory<TChannel>)_channelFactories[typeof(TChannel)];
            return factory.CreateChannel();
        }

        private static Binding CreateBinding(SoaServiceConnectionInfo connInfo)
        {
            switch (connInfo.ServiceBindingMode)
            {
                case SoaServiceBindingMode.BasicHttp: return new BasicHttpBinding();
            }
            return null;
        }

        private static EndpointAddress CreateEndpoint(SoaServiceConnectionInfo connInfo)
        {
            return new EndpointAddress(connInfo.Address);
        }

        /*
        public static void CloseChannel(Object channel)
        {
            var commObj = channel as ICommunicationObject;

            if (commObj.State == CommunicationState.Faulted)
                commObj.Abort();
            else if (commObj.State != CommunicationState.Closed)
            {
                try
                {
                    commObj.Close();
                }
                catch (CommunicationException)
                {
                    commObj.Abort();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
         */
    }
}
