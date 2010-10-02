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
using Wing.Modularity;
using Wing.Soa.Interop.Client;
using Wing.Soa.Interop;
using System.ServiceModel;
using Wing.ServiceLocation;
using Wing.Client.Core;

namespace Wing.Client.Modules.SoaConnector
{
    public class SoaClientMetadataProvider : ISoaClientServiceMetadataProvider
    {
        private ChannelFactory<ISoaMetadataProviderService> _factory;

        private ISoaMetadataProviderService CreateServiceChannel()
        {
            if (_factory == null)
            {
                var settings = ServiceLocator.Current.GetInstance<BootstrapSettings>();
                _factory = new ChannelFactory<ISoaMetadataProviderService>(new BasicHttpBinding(), new EndpointAddress(settings.SoaMetadataProviderUri));
            }
            return _factory.CreateChannel();
        }


        public void GetServiceConnectionInfoByContractType(Type contractType, Action<SoaServiceConnectionInfo> callback)
        {
            var channel = CreateServiceChannel();
            try
            {
                var async = channel.BeginGetServiceConnectionInfoByContractRefTypeName(contractType.Name, (r) => callback(channel.EndGetServiceConnectionInfoByContractRefTypeName(r)), null);
            }
            finally
            {
                SoaClientManager.CloseChannel(channel);
            }
        }
    }
}
